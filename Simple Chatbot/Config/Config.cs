/*
Config contains static classes that are used to store configuration data such as API keys and default values.
*/

using Project_ToolBox;

namespace Config
{
    static class AzureOpenAI
    {
        public static readonly string? ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        public static readonly string? BaseUrl = Environment.GetEnvironmentVariable("OPENAI_API_BASE");
        public static readonly string? DeploymentName = Environment.GetEnvironmentVariable("OPENAI_DEPLOYMENT_NAME");

        static AzureOpenAI()
        {
            if (string.IsNullOrEmpty(ApiKey)) throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
            if (string.IsNullOrEmpty(BaseUrl)) throw new InvalidOperationException("OPENAI_API_BASE environment variable is not set.");
            if (string.IsNullOrEmpty(DeploymentName)) throw new InvalidOperationException("OPENAI_DEPLOYMENT_NAME environment variable is not set.");
        }
    }

    static class IntentDetector
    {
        public static readonly string? defaultSystemPrompt = (
            "Determine if the intent requires one of the following functions:\n" +
            string.Join("\n", ToolBox.tools.Select(tool => $"- {tool.name} : {tool.desc}")) +
            "\nYou must respond only with the name value of the intent and nothing else." +
            "\nIf you detect an intent not listed above, please respond \"unknown_intent\"."
        );
    }

    static class Chatbot
    {
        public static readonly string? defaultSystemPrompt = (
            "Assistant is a chatbot that can help you with your tasks."
        );
    }
}