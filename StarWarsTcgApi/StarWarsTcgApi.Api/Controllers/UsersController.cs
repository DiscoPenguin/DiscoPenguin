using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces; // Assuming you'll create an IUserService

namespace StarWarsTcgApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UsersController(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IUserService userService,
            IAuthService authService
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userService = userService;
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="request">User registration details</param>
        /// <returns>Auth response with JWT token on success; Bad Request if username pre-exists</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
        {
            //NOTE: add more robust validation and error handling to User Registration
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userResponse = await _userService.RegisterUserAsync(request);
                if (userResponse == null)
                {
                    return BadRequest("Failed to register user. Username or email might be taken.");
                }
                return CreatedAtAction(nameof(GetUserById), new { id = userResponse.Id }, userResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred during user registration.");
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">User login credentials.</param>
        /// <returns>Auth response with JWT token on success, or unauthorized if credentials are invalid.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType((int)StatusCodes.Status200OK, Type = typeof(UserProfileResponse))]
        [ProducesResponseType((int)StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authResponse = await _authService.LoginAsync(loginDto);

            if (authResponse == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(authResponse);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        
        [HttpGet("id/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("name/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}