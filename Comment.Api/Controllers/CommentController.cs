using Comment.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Comment.Api.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(string comment)
        {
            return Accepted(await _commentService.CreateCommentAsync(comment));
        }

        [HttpGet("getId/{requestId}")]
        public async Task<IActionResult> GetCommentStatus(string requestId)
        {
            return Ok(await _commentService.GetIdCommentAsync(requestId));
        }

        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetComment(int commentId)
        {
            return Ok(await _commentService.GetComment(commentId));
        }
    }
}
