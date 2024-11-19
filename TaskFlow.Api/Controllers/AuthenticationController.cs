using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Api.Classes;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.ViewModels;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class AuthenticationController : ControllerBase
    {
	    private readonly JwtSettings _jwtSettings;
	    private readonly UserManager<ApplicationUser> _userManager;
	    private readonly RoleManager<IdentityRole> _roleManager;
	    private readonly ApplicationDbContext _context;
	    private readonly TokenValidationParameters _tokenValidationParameters;
	    
	    public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext context, TokenValidationParameters tokenValidationParameters, IOptions<JwtSettings> jwtSettings)
	    {
		    _userManager = userManager;
		    _roleManager = roleManager;
		    _context = context;
		    _tokenValidationParameters = tokenValidationParameters;
		    _jwtSettings = jwtSettings.Value;
	    }
	    
	    [AllowAnonymous]
		[HttpPost("create-user")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel registerVM)
		{
			if (ModelState.IsValid)
			{
				var userExists = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
				if (userExists != null)
					return BadRequest(new { error =$"User {registerVM.EmailAddress} already exists"});

				var applicationUser = new ApplicationUser
				{
					Name = registerVM.FirstName,
					LastName = registerVM.LastName,
					Email = registerVM.EmailAddress,
					UserName = registerVM.EmailAddress,
					SecurityStamp = Guid.CreateVersion7().ToString()
				};

				var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);
				if (!result.Succeeded)
					return BadRequest(new { error = result.Errors });

				if (registerVM.Role == UserRoles.Admin || registerVM.Role == UserRoles.Client)
					await _userManager.AddToRoleAsync(applicationUser, registerVM.Role);

				return Ok(new {data = $"{applicationUser.Name} {applicationUser.LastName}"});
			}

			return BadRequest("Please provide all required fields");
		}

		[AllowAnonymous]
		[HttpPost("user-signIn")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel loginVM)
		{
			if (!ModelState.IsValid)
				return BadRequest("Please provide all required fields");

			var userExists = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
			if (userExists == null || !await _userManager.CheckPasswordAsync(userExists, loginVM.Password))
				return Unauthorized();

			var tokenValue = await GenerateJWTTokenAsync(userExists);
			return Ok( tokenValue );
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] TokenRequestViewModel tokenRequestVM)
		{
			if (!ModelState.IsValid)
				return BadRequest("Please provide all required fields");

			var result = await VerifyAndGenerateToken(tokenRequestVM);
			return Ok(result);
		}

		private async Task<AuthResultViewModel> VerifyAndGenerateToken(TokenRequestViewModel tokenRequestVM)
		{
			var jwtTokenHandler = new JwtSecurityTokenHandler();
			var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequestVM.RefreshToken);
			var dbUser = await _userManager.FindByIdAsync(storedToken?.UserId);

			if (storedToken == null || dbUser == null)
				return new AuthResultViewModel { Token = null, RefreshToken = null, ExpiresAt = DateTime.MinValue };

			try
			{
				jwtTokenHandler.ValidateToken(tokenRequestVM.Token, _tokenValidationParameters, out var validatedToken);
				return await GenerateJWTTokenAsync(dbUser, storedToken);
			}
			catch (SecurityTokenExpiredException)
			{
				if (storedToken.DateExpire >= DateTime.UtcNow)
					return await GenerateJWTTokenAsync(dbUser, storedToken);

				return await GenerateJWTTokenAsync(dbUser, null);
			}
		}

		private async Task<AuthResultViewModel> GenerateJWTTokenAsync(ApplicationUser user, RefreshToken rToken = null)
		{
			var authClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Surname, user.LastName),
				new Claim("Uid", user.Id),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString())
			};

			var userRoles = await _userManager.GetRolesAsync(user);
			foreach (var role in userRoles)
				authClaims.Add(new Claim(ClaimTypes.Role, role));

			var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret));

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
			);

			var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

			if (rToken != null)
				return new AuthResultViewModel { Token = jwtToken, RefreshToken = rToken.Token, ExpiresAt = token.ValidTo };

			var refreshToken = new RefreshToken
			{
				JwtId = token.Id,
				IsRevoked = false,
				UserId = user.Id,
				DateAdded = DateTime.UtcNow,
				DateExpire = DateTime.UtcNow.AddHours(3),
				Token = $"{Guid.CreateVersion7()}-{Guid.CreateVersion7()}"
			};

			await _context.RefreshTokens.AddAsync(refreshToken);
			await _context.SaveChangesAsync();

			return new AuthResultViewModel { Token = jwtToken, RefreshToken = refreshToken.Token, ExpiresAt = token.ValidTo };
		}
	}
}