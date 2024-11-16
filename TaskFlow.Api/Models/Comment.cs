using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Api.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public ProjectTask ProjectTask { get; set; }
    
    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }
}