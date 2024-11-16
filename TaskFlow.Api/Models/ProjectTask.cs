using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Api.Models;

public class ProjectTask
{
    [Key]
    public int Id { get; set; }
    
    public int ProjectId { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Status { get; set; }
    public string? AssignedUserId { get; set; }
    
    public int Priority { get; set; }
    
    public DateTime? DueDate { get; set; }

    // Navigation Properties
    public Project Project { get; set; }
    
    [ForeignKey(nameof(AssignedUserId))]
    public ApplicationUser AssignedUser { get; set; }
    
    public ICollection<Comment> Comments { get; set; }
}