using Bogus;
using Comment.Console.Models;
using System.Net;

internal class Program
{
    private static readonly HttpClient _client = new HttpClient();

    private static async Task Main(string[] args)
    {
        _client.BaseAddress = new Uri("http://localhost:5115");

        var comments = new Faker<CommentFake>().RuleFor(x => x.Content, f => f.Lorem.Sentence()).Generate(10);

        foreach (var comment in comments)
        {
            var response = await _client.PostAsync($"/api/comments?comment={comment.Content}", null);

            if (response.IsSuccessStatusCode)
            {
                comment.RequestId = (await response.Content.ReadAsStringAsync()).Trim('"');
            }
        }

        var count = comments.Count();

        while (count > 0)
        {
            foreach (var comment in comments)
            {
                if (comment.Id == 0)
                {
                    var response = await _client.GetAsync($"/api/comments/getId/{comment.RequestId}");

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            comment.Id = int.Parse(await response.Content.ReadAsStringAsync());
                            comment.ResponseTime = DateTime.Now;
                            count --;
                        }                        
                    }

                    await Task.Delay(500);
                }
            }
        }

        foreach (var comment in comments)
        {
            Console.WriteLine($"Id: {comment.Id}, Time: {comment.ResponseTime:HH:mm:ss}, Comment: {comment.Content}");
        }

        Console.ReadLine();
    }
}