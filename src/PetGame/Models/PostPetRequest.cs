using PetGame.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PetGame.Models
{
    public class PostPetRequest
    {
        [Required]
        public int PlayerId { get; set; }
        [Required]
        [StringLength(Validation.StringMaxLength)]
        public string? Name { get; set; }
        [Required]
        public PetType Type { get; set; }
    }
}
