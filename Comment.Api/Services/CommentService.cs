using Comment.Api.Context;
using Comment.Api.Models;
using Comment.Api.Models.Entities;
using Comment.Api.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Comment.Api.Services
{
    public class CommentService : BaseService
    {
        private static ConcurrentDictionary<Guid, DataRequest> _pendingRequests = new ConcurrentDictionary<Guid, DataRequest>();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CommentService(CommentContext context, IServiceScopeFactory serviceScopeFactory) : base(context)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task<Guid> CreateCommentAsync(string comment)
        {
            var requestId = Guid.NewGuid();
            var delayInSeconds = new Random().Next(10, 16);
            _pendingRequests.TryAdd(requestId, new DataRequest());

            Task.Run(async () =>
            {                
                await Task.Delay(delayInSeconds * 1000);

                var NewComment = new CommentEntity { Content = comment };
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<CommentContext>();                
                context!.Add(NewComment);
                await context.SaveChangesAsync();
                _pendingRequests[requestId].IsSaved = true;
                _pendingRequests[requestId].Id = NewComment.Id;

                await Task.Delay(120 * 1000);

                _pendingRequests.TryRemove(requestId, out _);
            });

            return Task.FromResult(requestId);
        }

        public Task<int> GetIdCommentAsync(string id)
        {
            int result = 0;
            DataRequest data = new();

            if (_pendingRequests.TryGetValue(Guid.Parse(id), out data!))
            {
                result = data.Id;
            }
            else
            {
                throw new ArgumentException(nameof(GetIdCommentAsync));
            }

            return Task.FromResult(result);
        }

        public async Task<string> GetComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null)
            {
                throw new ArgumentException(nameof(GetComment));
            }

            return comment.Content!;
        }
    }
}
