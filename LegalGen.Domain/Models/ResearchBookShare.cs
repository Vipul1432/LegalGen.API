using LegalGen.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Models
{
    public class ResearchBookShare
    {
        public int Id { get; set; } 

        public string UserId { get; set; }
        public LegalGenUser User { get; set; }

        public int ResearchBookId { get; set; }
        public ResearchBook ResearchBook { get; set; }
    }
}
