using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Api.Models;

public class Notification
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Type { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }
}