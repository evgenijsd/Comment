using Comment.Api.Context;

namespace Comment.Api.Services.Abstract
{
    public class BaseService
    {
        private protected readonly CommentContext _context;

        public BaseService(CommentContext context)
        {
            _context = context;
        }
    }
}
