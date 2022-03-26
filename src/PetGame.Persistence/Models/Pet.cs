using PetGame.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PetGame.Persistence.Models
{
    public class Pet
    {
        public int ID { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public PetType Type { get; set; }
        [Required]
        public int Happiness { get; set; }
        [Required]
        public int Hunger { get; set; }
        [Required]
        public DateTimeOffset? AdoptionDate { get; set; }
        [Required]
        public DateTimeOffset? LastPetted { get; set; }
        [Required]
        public DateTimeOffset? LastFed { get; set; }
    }
}
