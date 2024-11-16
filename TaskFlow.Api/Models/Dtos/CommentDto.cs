namespace TaskFlow.Api.Models.Dtos;

public record CommentDto()
{
    public int Id { get; init; }
    public int TaskId { get; init; }
    public string Content { get; init; }
    public DateTime CreatedAt { get; init; }
    public string UserId { get; init; }
}