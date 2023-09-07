using LegalGen.Domain.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Models
{
    public class AiChat
    {
        [Key]
        public int Id { get; set; } 
        public string UserId { get; set; } 
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        // Navigation properties
        public LegalGenUser User { get; set; }
    }
}
