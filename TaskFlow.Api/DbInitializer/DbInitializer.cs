using Microsoft.AspNetCore.Identity;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskFlow.Api.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    
    public async Task Initialize()
    {
        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

        if (!await _roleManager.RoleExistsAsync(UserRoles.Client))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Client));
    }
}