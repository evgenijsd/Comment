using Microsoft.EntityFrameworkCore;
using Comment.Api.Models.Entities;

namespace Comment.Api.Context
{
    public class CommentContext : DbContext
    {
        public CommentContext(DbContextOptions<CommentContext> options) : base(options)
        {
        }

        protected CommentContext()
        {
        }

        public DbSet<CommentEntity> Comments { get; set; }
    }
}
