using LegalGen.Domain.Helper;
using LegalGen.Domain.Models;
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

        // DbSet for ResearchBook entity
        public DbSet<ResearchBook> ResearchBooks { get; set; }
        public DbSet<AiChat> AiChats { get; set; }
        public DbSet<ResearchBookShare> ResearchBookShares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ResearchBookShare>()
                .HasKey(ura => ura.Id);

            modelBuilder.Entity<ResearchBookShare>()
                .HasOne(ura => ura.User)
                .WithMany(u => u.BookAssignments)
                .HasForeignKey(ura => ura.UserId);

            modelBuilder.Entity<ResearchBookShare>()
                .HasOne(ura => ura.ResearchBook)
                .WithMany(b => b.UserAssignments)
                .HasForeignKey(ura => ura.ResearchBookId);
        }

    }
}
