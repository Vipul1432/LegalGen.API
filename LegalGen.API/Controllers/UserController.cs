using LegalGen.Domain.Dtos;
using LegalGen.Domain.Helper;
using LegalGen.Domain.Interfaces;
using LegalGen.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace LegalGen.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterDto> _logger;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The repository for user-related operations.</param>
        /// <param name="configuration">The configuration object used for accessing application settings.</param>
        public UserController(IUserService userService, IConfiguration configuration, ILogger<RegisterDto> logger, IEmailService emailService)
        {
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
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
            {
                _logger.LogInformation("User Already exist!");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User already exists!" });
            }

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
            _logger.LogInformation("User registered successfully!");
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
                _logger.LogInformation("Invalid user credentials!");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "Invalid user credentials." });
            }

            //Check User
            var user = await _userService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogInformation("User not found!");
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
                _logger.LogInformation("Invalid password!");
                return StatusCode(StatusCodes.Status401Unauthorized,
                     new Response { Status = "Error", Message = "Invalid password." });
            }
        }


        /// <summary>
        /// Initiates the process of resetting a user's password by sending a password reset link via email.
        /// </summary>
        /// <param name="email">The email address of the user requesting a password reset.</param>
        /// <returns>
        /// - 200 OK: If the request to send the password reset link is successful.
        /// - 400 Bad Request: If the request is invalid or cannot be processed.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("forget-password/{email}")]
        public async Task<IActionResult> ForgetPassword([FromRoute] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user != null)
            {
                var token = await _userService.GetPasswordResetTokenAsync(user);
                var encodedToken = Encoding.UTF8.GetBytes(token);
                var validToken = WebEncoders.Base64UrlEncode(encodedToken);

                var forgetPasswordLink = $"{Request.Scheme}://{Request.Host}/api/user/ResetPassword?token={validToken}&email={user.Email}";
                var message = new EmailMessage(new string[] { user.Email }, "Forget password link", "<h1>To reset your password.</h1><p><a href='" + forgetPasswordLink! + "'>Click Here</a></p>");
                _emailService.SendEmail(message);

                _logger.LogInformation("Password change request is sent on Email");
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = $"Password change request is sent on Email {user.Email}. Please open your email $ click the link." });
            }
            _logger.LogError("Could not send link, please try again.");
            return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "Could not send link, please try again." });
        }


        /// <summary>
        /// Resets the user's password with a provided token and new password.
        /// </summary>
        /// <param name="resetPassword">A model containing email, token, password, and confirmation password for password reset.</param>
        /// <returns>
        /// - 200 OK: If the password is successfully reset.
        /// - 400 Bad Request: If the request is invalid or cannot be processed due to various reasons.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPassword resetPassword)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Some model's properties are not valid.");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "Some properties are not valid." });
            }
            var user = await _userService.GetUserByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                if (resetPassword.Password != resetPassword.ConfirmPassword)
                {
                    _logger.LogError("Password & Confirm Password doesn't match.");
                    return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "Password & Confirm Password doesn't match." });
                }
                var decodedToken = WebEncoders.Base64UrlDecode(resetPassword.Token);
                string normalToken = Encoding.UTF8.GetString(decodedToken);

                var resetPasswordResult = await _userService.CreateResetPasswordAsync(user, normalToken, resetPassword.Password);
                if (!resetPasswordResult.success)
                {
                    var errorMessage = "Cann't reset password. ";

                    foreach (var error in resetPasswordResult.errors)
                    {
                        errorMessage += $" {error}";
                    }
                    _logger.LogError(errorMessage);
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = "Error", Message = errorMessage });
                }

                var message = new EmailMessage(new string[] { user.Email }, "Password Reset", "Your password has been changed.");
                _emailService.SendEmail(message);

                _logger.LogInformation("Password has been changed.");
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = "Password has been changed." });
            }

            _logger.LogError("Could not find email, please try again.");
            return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "Could not find email, please try again." });
        }


        /// <summary>
        /// Changes the password for the currently authenticated user.
        /// </summary>
        /// <param name="model">A model containing the current password and the new password.</param>
        /// <returns>
        /// - 200 OK: If the password change is successful.
        /// - 400 Bad Request: If the password change fails or the request is invalid.
        /// - 401 Unauthorized: If the user is not authenticated.
        /// </returns>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                _logger.LogError("User is not authenticated");
                return StatusCode(StatusCodes.Status401Unauthorized,
                        new Response { Status = "Error", Message = "User is not authenticated" });
            }
            var userId = userIdClaim.Value;
            var isPasswordChanged = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (isPasswordChanged)
            {
                await _userService.SignOutAsync();
                _logger.LogInformation("Password changed successfully.");
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = "Password changed successfully" });
            }
            else
            {
                _logger.LogError("Password change failed");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "Password change failed" });
            }
        }


        /// <summary>
        /// Retrieves the user's profile information based on the authentication token.
        /// </summary>
        /// <returns>
        /// - 200 OK with the user's profile details if authenticated and profile found.
        /// - 401 Unauthorized with an error message if the user is not authenticated.
        /// - 404 Not Found with an error message if the user's profile is not found.
        /// </returns>
        /// <remarks>
        /// This endpoint is protected by the [Authorize] attribute, ensuring that only authenticated
        /// users can access it. It retrieves the user's ID from the authentication token and uses it
        /// to fetch the user's profile information via the <see cref="_userService"/>.
        /// Logging is performed to record successful and error cases.
        /// </remarks>
        [Authorize]
        [HttpGet("profile-details")]
        public async Task<IActionResult> GetUserProfile()
        {
            // Get the user's ID from the authentication token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _logger.LogError("User is not authenticated");
                return StatusCode(StatusCodes.Status401Unauthorized,
                        new Response { Status = "Error", Message = "User is not authenticated" });
            }

            // Retrieve the user's profile information (you can customize this based on your data model)
            var userProfile = await _userService.GetUserProfileAsync(userId);

            if (userProfile == null)
            {
                _logger.LogError("User profile not found");
                return StatusCode(StatusCodes.Status404NotFound,
                        new Response { Status = "Error", Message = "User profile not found" });
            }
            _logger.LogInformation("User retrived successfully");
            return Ok(userProfile);
        }


        /// <summary>
        /// Updates the user's profile based on the provided data.
        /// </summary>
        /// <param name="model">The <see cref="UserProfile"/> containing the updated profile information.</param>
        /// <returns>
        /// - 200 OK with a success message if the profile is updated successfully.
        /// - 400 Bad Request with an error message if the profile update fails.
        /// - 401 Unauthorized with an error message if the user is not authenticated.
        /// </returns>
        /// <remarks>
        /// This endpoint is protected by the [Authorize] attribute, ensuring that only authenticated
        /// users can access it. It retrieves the user's ID from the authentication token and uses it
        /// to call the <see cref="_userService"/> for profile update. Logging is performed to record
        /// successful and error cases.
        /// </remarks>
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfile model)
        {
            // Get the user's ID from the authentication token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _logger.LogError("User is not authenticated");
                return StatusCode(StatusCodes.Status401Unauthorized,
                        new Response { Status = "Error", Message = "User is not authenticated" });
            }

            // Call the service to update the user's profile
            var updated = await _userService.UpdateUserProfileAsync(userId, model);

            if (updated)
            {
                _logger.LogInformation("Profile updated successfully");
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = "Profile updated successfully" });
            }
            else
            {
                _logger.LogError("Profile update failed");
                return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = "Error", Message = "Profile update failed" });
            }
        }

        /// <summary>
        /// Retrieves the user's ID from the JWT token's claims and returns it as a response.
        /// </summary>
        /// <returns>
        /// If the user's ID is found in the token's claims, it returns a 200 OK response with the user's ID.
        /// If the user's ID is not found, it returns a 404 NotFound response.
        /// </returns>
        [HttpGet("UserId")]
        public IActionResult GetUserId()
        {
            // Retrieve the user's claims from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
                // Get the UserId from the claim
                var userId = userIdClaim.Value;
                return Ok(userId);
            }
            else
            {
                return NotFound("UserId not found");
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
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.SignOutAsync();
            _logger.LogInformation("Logged out successfully!");
            return Ok(new Response { Status = "Success", Message = "Logged out successfully!" });
        }

    }
}
