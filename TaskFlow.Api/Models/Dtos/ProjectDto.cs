namespace TaskFlow.Api.Models.Dtos;

public record ProjectDto()
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string OwnerId { get; init; }
}