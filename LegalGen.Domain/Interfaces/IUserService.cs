using LegalGen.Domain.Helper;
using LegalGen.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation and returns the user.</returns>
        Task<LegalGenUser> GetUserByEmailAsync(string email);

        /// <summary>
        /// Creates a new user with the provided information and password.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>
        /// A tuple indicating success status and a collection of error messages if any.
        /// </returns>
        Task<(bool success, IEnumerable<string> errors)> CreateUserAsync(LegalGenUser user, string password);

        /// <summary>
        /// Checks if the provided password matches the user's password.
        /// </summary>
        /// <param name="user">The user to validate.</param>
        /// <param name="password">The password to check.</param>
        /// <returns>The result of the password check as a task.</returns>
        Task<SignInResult> CheckPasswordAsync(LegalGenUser user, string password);

        /// <summary>
        /// Signs the user out of the application.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SignOutAsync();

        /// <summary>
        /// Generates a JWT (JSON Web Token) for the provided user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>The JWT token as a string.</returns>
        string GenerateJwtToken(LegalGenUser user);

        /// <summary>
        /// Generates a password reset token for the user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>A task that represents the asynchronous operation and returns the reset token.</returns>
        Task<string> GetPasswordResetTokenAsync(LegalGenUser user);

        /// <summary>
        /// Creates a new password for the user using a password reset token.
        /// </summary>
        /// <param name="user">The user for whom the password is reset.</param>
        /// <param name="token">The password reset token.</param>
        /// <param name="password">The new password.</param>
        /// <returns>
        /// A tuple indicating success status and a collection of error messages if any.
        /// </returns>
        Task<(bool success, IEnumerable<string> errors)> CreateResetPasswordAsync(LegalGenUser user, string token, string password);

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="oldPassword">The user's current password.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the password is changed successfully.</returns>
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);

        /// <summary>
        /// Retrieves a user's profile based on their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation and returns the user's profile.</returns>
        Task<UserProfile> GetUserProfileAsync(string userId);

        /// <summary>
        /// Updates a user's profile information based on the provided data.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="model">The updated profile information.</param>
        /// <returns>A task that represents the asynchronous operation and returns true if the profile is updated successfully.</returns>
        Task<bool> UpdateUserProfileAsync(string userId, UserProfile model);
    }
}
