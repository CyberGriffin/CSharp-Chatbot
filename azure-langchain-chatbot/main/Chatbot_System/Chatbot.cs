/*
Chatbot is a system that can be used to chat with the user.
*/

using Azure.AI.OpenAI;
using Utils;

namespace Chatbot_System
{
    public class Chatbot : Core
    {
        public Chatbot(string? systemPrompt = null, int maxTokens = 800, float temperature = 0.5f)
            : base(systemPrompt ?? Config.Chatbot.defaultSystemPrompt!, maxTokens, temperature) {}

         public async Task<string> Run(
            string query,
            int timeoutMilliseconds = 10000,
            int retryDelayMilliseconds = 100,
            int maxRetries = 3,
            Action<string>? callbackFunction = null
        )
        {
            // Check FAISS index
            string jsonResponse = await WrapperUtils.SearchIndex(query);
            string ParsedResponse = WrapperUtils.Parse(jsonResponse);

            query = $"QUESTION: {query}" + "\n\n" + $"INFO: {ParsedResponse}";

            // Get response from chatbot
            string response = await _Run(query, timeoutMilliseconds, retryDelayMilliseconds, maxRetries, callbackFunction);

            // Clear memory and update system prompt (this is to prevent the LLM from deviating from the system prompt)
            _options.Messages.Clear();
            _systemPrompt += (
                $"\nUser: {query}" +
                $"\nAssistant: {response}"
            );
            _options.Messages.Add(new ChatMessage(ChatRole.System, _systemPrompt));

            // Return response
            return response;
        }
    }
}