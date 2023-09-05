using LegalGen.Domain.Helper;
using LegalGen.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
