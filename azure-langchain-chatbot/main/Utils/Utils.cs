using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Utils
{
    static public class CallbackFunction
    {
        static public void OutputCallback(string output)
        {
            if (output == null)
            {
                // Handle the null value
                return;
            }

            // Print word by word
            string[] words = output.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                Console.Write(words[i]);
                
                // Add space after each word except the last one
                if (i < words.Length - 1)
                {
                    Console.Write(" ");
                }
                
                Thread.Sleep(10);
            }
        }
    }

    static public class WrapperUtils
    {
        static async public Task<string> IndexDocuments(bool debug = false)
        {
            string endpoint = "indexer";
            string apiBaseUrl = Config.Wrapper.FlaskApiBaseUrl;
            var apiWrapper = new MyFlaskApiWrapper(apiBaseUrl);

            if (debug)
                Console.WriteLine("Indexing documents...");
            await apiWrapper.PostApiRequest(endpoint, "");
            return "Indexing completed";
        }

        static async public Task<string> SearchIndex(string query, bool debug = false)
        {
            string endpoint = "search_index";
            string apiBaseUrl = Config.Wrapper.FlaskApiBaseUrl;
            var apiWrapper = new MyFlaskApiWrapper(apiBaseUrl);

            var queryData = new
            {
                query
            };

            string jsonData = JsonSerializer.Serialize(queryData);

            if (debug)
                Console.WriteLine("Searching index...");
            string responseContent = await apiWrapper.PostApiRequest(endpoint, jsonData);

            return responseContent;
        }

        static public string Parse(string jsonString)
        {
            JObject jsonObject = JObject.Parse(jsonString);
            string message = jsonObject["message"].ToString();

            return message;
        }
    }
}