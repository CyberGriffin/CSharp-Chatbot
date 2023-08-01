/*
Core is the base class for all chatbot related systems. It contains the core variables and methods that are used by all chatbot systems.
*/

using Azure;
using Azure.AI.OpenAI;

namespace Chatbot_System
{
    public class Core
    {
        // Core variables
        protected string _systemPrompt;
        protected readonly OpenAIClient _client;
        protected ChatCompletionsOptions _options { get; }

        // Constructor
        protected Core(string systemPrompt, int maxTokens, float temperature)
        {
            _systemPrompt = systemPrompt;

            _client = new OpenAIClient(
                new Uri(Config.AzureOpenAI.BaseUrl!),
                new AzureKeyCredential(Config.AzureOpenAI.ApiKey!)
            );

            _options = new ChatCompletionsOptions()
            {
                Messages = { new ChatMessage(ChatRole.System, _systemPrompt)},
                MaxTokens = maxTokens,
                Temperature = temperature,
            };
        }

        //==========================//
        // Core methods (protected) //
        //==========================//
        protected async Task<string> _Run(
            string query,
            int timeoutMilliseconds,
            int retryDelayMilliseconds,
            int maxRetries,
            Action<string>? callbackFunction = null
        )
        {
            // Handle invalid query
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));
            }

            // Track number of retries
            int currentRetry = 0;

            // Save query to memory (required to generate response)
            _options.Messages.Add(new ChatMessage(ChatRole.User, query));

            // Start retry mechanism
            while (currentRetry <= maxRetries)
            {
                try
                {
                    // Create response task
                    var responseTask = _client.GetChatCompletionsStreamingAsync(
                        Config.AzureOpenAI.DeploymentName!,
                        _options
                    );

                    // Create a CancellationTokenSource to enforce timeout
                    using (var cts = new CancellationTokenSource())
                    {
                        // Combine the original task with the timeout task
                        var completedTask = await Task.WhenAny(responseTask, Task.Delay(timeoutMilliseconds, cts.Token));

                        if (completedTask == responseTask)
                        {
                            // The response task was successful
                            var response = responseTask.Result;
                            string fullResponse = "";

                            // Stream response to callback function if provided
                            await foreach (StreamingChatChoice choice in response.Value.GetChoicesStreaming())
                            {
                                await foreach (ChatMessage message in choice.GetMessageStreaming())
                                {
                                    fullResponse += message.Content;
                                    callbackFunction?.Invoke(message.Content);
                                }
                            }

                            // Add full response to memory and return it
                            _options.Messages.Add(new ChatMessage(ChatRole.Assistant, fullResponse));
                            return fullResponse;
                        }
                        else
                        {
                            // The response task timed out, resend query after backoff
                            currentRetry++;
                            _backoff(retryDelayMilliseconds, currentRetry);
                        }
                    }
                }
                catch (TaskCanceledException ex) when (ex.CancellationToken.IsCancellationRequested)
                {
                    // The response task timed out and raised an exception, resend query after backoff
                    currentRetry++;
                    _backoff(retryDelayMilliseconds, currentRetry);
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    _options.Messages.Add(new ChatMessage(ChatRole.Assistant, ex.Message));
                    callbackFunction?.Invoke(ex.Message);
                    return ex.Message;
                }
            }

            // Operation has timed out and failed all retries
            _options.Messages.Add(new ChatMessage(ChatRole.Assistant, "Operation timed out."));
            callbackFunction?.Invoke("Operation timed out.");
            return "Operation timed out.";
        }

        //==========================//
        //  Core methods (private)  //
        //==========================//
        protected private async void _backoff(int retryDelayMilliseconds, int currentRetry)
        {
            int backoff = (int)((Math.Pow(2, currentRetry) - 1) * retryDelayMilliseconds);
            await Task.Delay(backoff);
        }
    }
}