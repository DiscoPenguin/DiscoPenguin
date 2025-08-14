using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Net; // For HttpStatusCode

using StarWarsTcg.Security;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.DTOs.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


namespace StarWarsTcgApi.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration
        )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserProfileResponse))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                NormalizedUserName = model.Email.ToUpperInvariant(),
                NormalizedEmail = model.Email.ToUpperInvariant(),
                EmailConfirmed = true,
                FirstName = model.FirstName,
                LastName = model.LastName,
                AvatarId = model.AvatarId
            };
            //TODO: EmailConfirmed should be false until email confirmation is implemented
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok(user);
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        ///  ./StarWarsTcg/StarWarsTcgApi/StarWarsTcgApi.Application/Services/AuthService.cs
        /// </summary>
        /// <param name="loginDto">User login credentials.</param>
        /// <returns>Auth response with JWT token on success, or unauthorized if credentials are invalid.</returns>
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserProfileResponse))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var token = GenerateJwtToken(user);
                return Ok(new UserProfileResponse
                {
                    Id = user?.Id,
                    UserName = user?.UserName,
                    Token = token,
                    Email = user?.Email,
                    AvatarId = (int)(user?.AvatarId),
                //  AvatarUrl = user?.AvatarUrl,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                    Roles = new List<string>()
                    //Roles = user?.Roles.Select(r => r.Name).ToList() ?? new List<string>()
                });
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured."));

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user?.NormalizedUserName)
        };

            /* Add roles as claims
            foreach (var role in user?.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            */

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public class RegisterModel
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int AvatarId { get; set; } = 0;
        }

        public class LoginModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}