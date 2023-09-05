using System.ComponentModel.DataAnnotations;

namespace LegalGen.Domain.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Organization { get; set; }
        [Required]
        public string ContactDetails { get; set; }
    }
}
