/*
API Wrapper for the Python API, allowing use of tools such as langchain and FAISS.
*/

using System.Text;

public class MyFlaskApiWrapper
{
    private readonly string baseUrl;

    public MyFlaskApiWrapper(string baseUrl)
    {
        this.baseUrl = baseUrl;
    }

    public async Task<string> PostApiRequest(string endpoint, string jsonData)
    {
        using (var httpClient = new HttpClient())
        {
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{baseUrl}/{endpoint}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}
