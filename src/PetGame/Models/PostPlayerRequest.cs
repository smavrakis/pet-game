using PetGame.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PetGame.Models
{
    public class PostPlayerRequest
    {
        [Required]
        [StringLength(Validation.StringMaxLength)]
        public string? FirstName { get; set; }
        [StringLength(Validation.StringMaxLength)]
        public string? MiddleName { get; set; }
        [Required]
        [StringLength(Validation.StringMaxLength)]
        public string? LastName { get; set; }
        [Required]
        [StringLength(Validation.StringMaxLength)]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
