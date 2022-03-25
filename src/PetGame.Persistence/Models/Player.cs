using System.ComponentModel.DataAnnotations;

namespace PetGame.Persistence.Models
{
    public class Player
    {        
        public int ID { get; set; }
        [Required]
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public DateTimeOffset? RegistrationDate { get; set; }

        public List<Pet> Pets { get; set; } = new List<Pet>();
    }
}
