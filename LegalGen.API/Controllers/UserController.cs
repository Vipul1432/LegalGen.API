using JwtAuthentication.Domain.Models;
using LegalGen.Domain.Dtos;
using LegalGen.Domain.Helper;
using LegalGen.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LegalGen.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The repository for user-related operations.</param>
        /// <param name="configuration">The configuration object used for accessing application settings.</param>
        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user in the application.
        /// </summary>
        /// <remarks>
        /// This method validates the user's details, checks if the user already exists, and creates a new user account.
        /// It returns a success message upon successful registration or an error message if registration fails.
        /// </remarks>
        /// <param name="model">The register dto containing user details.</param>
        /// <returns>
        /// An HTTP response indicating success or failure along with a message.
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "Invalid user details." });
            }

            // Check If User exists
            var userExists = await _userService.GetUserByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User already exists!" });

            LegalGenUser user = new LegalGenUser()
            {
                UserName = model.Email,
                Email = model.Email,
                PasswordHash = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Organization = model.Organization,
                PhoneNumber = model.ContactDetails,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userService.CreateUserAsync(user, model.Password);
            if (!result.success)
            {
                var errorMessage = "User creation failed! Please check user details and try again.";

                foreach (var error in result.errors)
                {
                    errorMessage += $" {error}";
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = errorMessage });
            }

            return Ok(new Response { Status = "Success", Message = "User registered successfully!" });
        }


        /// <summary>
        /// Logs in a user and generates a JWT token upon successful authentication.
        /// </summary>
        /// <param name="model">The login dto containing user credentials.</param>
        /// <returns>
        /// An HTTP response containing the JWT token and its expiration time if login is successful,
        /// or an error message if authentication fails.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "Invalid user credentials." });
            }

            //Check User
            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "User not found!" });
            }
            //Check Password
            var result = await _userService.CheckPasswordAsync(user, model.Password);

            if (result.Succeeded)
            {
                var jwtToken = _userService.GenerateJwtToken(user);

                // Returning the token along with other information
                return Ok(new
                {
                    jwtToken,
                    expiration = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JWT:LifetimeInMinutes"]))
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized,
                     new Response { Status = "Error", Message = "Invalid password." });
            }
        }


        /// <summary>
        /// Signs the user out of the application.
        /// </summary>
        /// <remarks>
        /// This method clears the user's authentication status, effectively logging them out.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.SignOutAsync();
            return Ok(new Response { Status = "Success", Message = "Logged out successfully!" });
        }
    }
}
