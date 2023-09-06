using LegalGen.Domain.Helper;
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
        Task<LegalGenUser> GetUserByEmailAsync(string email);
        Task<(bool success, IEnumerable<string> errors)> CreateUserAsync(LegalGenUser user, string password);
        Task<SignInResult> CheckPasswordAsync(LegalGenUser user, string password);
        Task SignOutAsync();
        string GenerateJwtToken(LegalGenUser user);
    }
}
