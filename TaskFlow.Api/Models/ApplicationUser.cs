using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Api.Models;

public class ApplicationUser : IdentityUser
{
    public required string  Name { get; set; } 
    public required string LastName { get; set; }
}