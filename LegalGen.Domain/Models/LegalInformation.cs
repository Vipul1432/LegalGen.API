using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Models
{
    public class LegalInformation
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Document { get; set; }
        public DateTime DateAdded { get; set; }

        // Foreign key
        public int ResearchBookId { get; set; }
        // Navigation property for one-to-many relationship
        public ResearchBook ResearchBook { get; set; }

    }
}
