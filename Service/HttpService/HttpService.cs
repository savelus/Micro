namespace Rabbit.HttpService;

public class HttpService : IHttpService
{
    public async Task<string> GetStatusCodeAsync(string url)
    {

        try {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            using var response = await client.GetAsync(url);

            return response.StatusCode.ToString();
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return "";
        }
    }
}