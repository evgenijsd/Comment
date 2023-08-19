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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ActiveCommentsService _activeCommentsService;

        public CommentService(
            CommentContext context, 
            IServiceScopeFactory serviceScopeFactory,
            ActiveCommentsService activeCommentsService) : base(context)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _activeCommentsService = activeCommentsService;
        }

        public Task<Guid> CreateCommentAsync(string comment)
        {
            if (string.IsNullOrEmpty(comment)) 
            {
                throw new ArgumentException(nameof(CreateCommentAsync));
            }

            var requestId = Guid.NewGuid();
            var delayInSeconds = new Random().Next(10, 16);
            _activeCommentsService.Add(requestId, new DataRequest());

            Task.Run(async () =>
            {                
                await Task.Delay(delayInSeconds * 1000);

                var NewComment = new CommentEntity { Content = comment };
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<CommentContext>();                
                context!.Add(NewComment);
                await context.SaveChangesAsync();
                var data = new DataRequest { Id = NewComment.Id, IsSaved = true };
                _activeCommentsService.UpdateActivity(requestId, data);

                await Task.Delay(120 * 1000);

                _activeCommentsService.Remove(requestId);
            });

            return Task.FromResult(requestId);
        }

        public Task<int> GetIdCommentAsync(Guid id)
        {
            int result = _activeCommentsService.GetId(id);

            if (result == 0)
            {
                throw new ArgumentException(nameof(GetIdCommentAsync));
            }

            return Task.FromResult(result);
        }

        public async Task<string> GetCommentAsync(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null)
            {
                throw new ArgumentException(nameof(GetCommentAsync));
            }

            return comment.Content!;
        }
    }
}
