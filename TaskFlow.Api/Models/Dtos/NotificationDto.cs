namespace TaskFlow.Api.Models.Dtos;

public record NotificationDto()
{
    public int Id { get; init; }
    public string UserId { get; init; }
    public string Message { get; init; }
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Type { get; init; }
}