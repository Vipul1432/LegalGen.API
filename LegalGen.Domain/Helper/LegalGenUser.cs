using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Helper
{
    public class LegalGenUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
    }
}
