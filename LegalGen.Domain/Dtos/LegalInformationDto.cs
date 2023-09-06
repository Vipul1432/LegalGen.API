using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.DTOs
{
    public class LegalInformationDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Document { get; set; }
        public DateTime DateAdded { get; set; }
        public int ResearchBookId { get; set; } 
    }
}
