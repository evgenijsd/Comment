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
                Console.WriteLine($"RequestId: {comment.RequestId}, Time: {DateTime.Now:HH:mm:ss}, Comment: {comment.Content}");
            }
        }

        var count = comments.Count();
        Console.WriteLine();

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
                            comment.ReceivedTime = DateTime.Now;

                            var responseComment = await _client.GetAsync($"/api/comments/{comment.Id}");

                            if (responseComment.IsSuccessStatusCode)
                            {
                                var receivedComment = await responseComment.Content.ReadAsStringAsync();
                                Console.WriteLine($"Id: {comment.Id, 5}, Time: {comment.ReceivedTime:HH:mm:ss}, Comment: {receivedComment}");
                            }
                            
                            count --;
                        }                        
                    }

                    await Task.Delay(500);
                }
            }
        }

        Console.ReadLine();
    }
}