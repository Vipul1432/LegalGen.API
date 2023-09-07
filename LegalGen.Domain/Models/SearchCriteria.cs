using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Models
{
    public class SearchCriteria
    {
        public string? DocumentType { get; set; }
        public string? Title { get; set; }
        public DateTime? Date { get; set; }
    }
}
