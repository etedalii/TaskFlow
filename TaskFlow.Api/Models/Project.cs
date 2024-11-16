using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Api.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public string OwnerId { get; set; }
    
    [ForeignKey(nameof(OwnerId))]
    public ApplicationUser Owner { get; set; }
    
    public ICollection<ProjectTask> ProjectTasks { get; set; }
}