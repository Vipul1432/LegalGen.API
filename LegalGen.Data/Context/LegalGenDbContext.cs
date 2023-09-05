using LegalGen.Domain.Helper;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Data.Context
{
    public class LegalGenDbContext : IdentityDbContext<LegalGenUser>
    {
        public LegalGenDbContext(DbContextOptions<LegalGenDbContext> options) : base(options) { }
    }
}
