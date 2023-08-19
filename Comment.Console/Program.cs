internal class Program
{
    private static async Task Main(string[] args)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5115");

        var response = await client.GetAsync($"/api/comments/1");

        Console.WriteLine(await response.Content.ReadAsStringAsync());

        Console.ReadLine();
    }
}