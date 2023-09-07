using LegalGen.Domain.Models;
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

        // Navigation properties
        public ICollection<ResearchBook> ResearchBooks { get; set; }
        public ICollection<AiChat> AiChats { get; set; }

    }
}
