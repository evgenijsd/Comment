namespace Comment.Api.Models.Entities
{
    public class CommentEntity
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
