/*
The starting point of the program.
*/

using Chatbot_System;
using Utils;

namespace Project
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Index documents located in the data folder.
            await WrapperUtils.IndexDocuments(true);

            // Create instance of IntentDetector and Chatbot
            IntentDetector intentDetector = new IntentDetector();
            Chatbot chatbot = new Chatbot();

            // Start chatbot
            while (true) {
                // Get user query
                Console.Write("You: ");
                string query = Console.ReadLine()!;

                // Check for exit conditions
                if (query.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                // Check for invalid input
                if (query.Trim() == "")
                    continue;

                // Detect intent
                Console.Write($"AI: ");
                var intent = await intentDetector.Run(query, timeoutMilliseconds: 1000, callbackFunction: CallbackFunction.OutputCallback);
                if (!intent.Item1) {
                    // Get response from chatbot
                    await chatbot.Run(query, timeoutMilliseconds: 2500, callbackFunction: CallbackFunction.OutputCallback);
                }
                Console.Write("\n");
            }
        }
    }
}