﻿using LegalGen.Domain.Helper;
using LegalGen.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Data.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<LegalGenUser> _userManager;
        private readonly SignInManager<LegalGenUser> _signInManager;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<LegalGenUser> userManager,
            SignInManager<LegalGenUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation that returns the user with the specified email address,
        /// or <c>null</c> if no user is found with the provided email address.
        /// </returns>
        public async Task<LegalGenUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }


        /// <summary>
        /// Creates a new user account asynchronously with the specified user details and password.
        /// </summary>
        /// <param name="user">The user details for the new account.</param>
        /// <param name="password">The password for the new account.</param>
        /// <returns>
        /// A <see cref="ValueTuple"/> containing a <see cref="bool"/> indicating the success of user creation,
        /// and an <see cref="IEnumerable{T}"/> of error messages if the creation fails.
        /// </returns>
        public async Task<(bool success, IEnumerable<string> errors)> CreateUserAsync(LegalGenUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return (true, Enumerable.Empty<string>());
            }
            else
            {
                var errorMessages = result.Errors.Select(error => error.Description);
                return (false, errorMessages);
            }
        }


        /// <summary>
        /// Checks if the provided password matches the password associated with the user's account.
        /// </summary>
        /// <param name="user">The user whose password is being checked.</param>
        /// <param name="password">The password to be verified.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation that returns a <see cref="SignInResult"/>.
        /// The result indicates whether the password verification succeeded, whether the account is locked, and other authentication-related information.
        /// </returns>
        public async Task<SignInResult> CheckPasswordAsync(LegalGenUser user, string password)
        {
            return await _signInManager.PasswordSignInAsync(user, password, false, false);
        }


        /// <summary>
        /// Signs the user out of the application asynchronously.
        /// </summary>
        /// <remarks>
        /// This method clears the user's authentication status, effectively logging them out.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        /// <summary>
        /// Generates a JWT token for the specified user with the configured claims and settings.
        /// </summary>
        /// <param name="user">The user for whom the JWT token is generated.</param>
        /// <returns>The JWT token as a string.</returns>
        public string GenerateJwtToken(LegalGenUser user)
        {
            // Create a list of authentication claims for the user
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Get the JWT secret key from configuration and convert it to a byte array
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            // Get the token's lifetime in minutes from configuration
            var lifetimeInMinutes = Convert.ToInt32(_configuration["JWT:LifetimeInMinutes"]);

            var jwtToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(lifetimeInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            // Serialize the JWT token to a string representation
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
