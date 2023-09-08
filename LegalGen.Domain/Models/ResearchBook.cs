using LegalGen.Domain.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Models
{
    public class ResearchBook
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }

        // Navigation properties
        public string UserId { get; set; }
        public LegalGenUser User { get; set; }
        public ICollection<LegalInformation> LegalInformation { get; set; }

        public ICollection<ResearchBookShare> UserAssignments { get; set; }

    }
}
