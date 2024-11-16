namespace TaskFlow.Api.Models.Dtos;

public record ProjectTaskDto()
{
    public int Id { get; init; }
    public int ProjectId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Status { get; init; }
    public string? AssignedUserId { get; init; }
    public int Priority { get; init; }
    public DateTime? DueDate { get; init; }
}