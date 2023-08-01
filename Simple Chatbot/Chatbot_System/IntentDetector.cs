/*
IntentDetector is a system that detects the user's intent and matches that intent to a tool in the toolbox.
*/

using Azure.AI.OpenAI;
using Project_ToolBox;

namespace Chatbot_System
{
    public class IntentDetector : Core
    {
        public IntentDetector(string? systemPrompt = null, int maxTokens = 200, float temperature = 0)
            : base(systemPrompt ?? Config.IntentDetector.defaultSystemPrompt!, maxTokens, temperature) {}
        
        public async Task<Tuple<bool, string, string>> Run(
            string query,
            int timeoutMilliseconds = 5000,
            int retryDelayMilliseconds = 100,
            int maxRetries = 3,
            Action<string>? callbackFunction = null
        )
        {
            string intent = "unknown_intent";
            string returnValue = "";

            // Determine user intent
            intent = await _Run(query, timeoutMilliseconds, retryDelayMilliseconds, maxRetries, null);

            // Clear memory and re-add system prompt (this is to prevent the next query from being affected by the previous query)
            // The reason memory was added in the first place is because the user query is required to generate a response.
            _options.Messages.Clear();
            _options.Messages.Add(new ChatMessage(ChatRole.System, _systemPrompt));

            // Match intent to tool
            returnValue = "";
            foreach (Tool tool in ToolBox.tools)
            {
                if (tool.name != null && intent.Contains(tool.name))
                {
                    // Match found
                    returnValue = tool.func?.Invoke()!;
                    if (tool.allowCallback) callbackFunction?.Invoke(returnValue);
                    return new Tuple<bool, string, string>(true, intent, returnValue);
                }
            }

            // No match found
            return new Tuple<bool, string, string>(false, intent, returnValue);
        }
    }
}