using PetGame.Common.Constants;

namespace PetGame.Persistence.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public PetType Type { get; set; }
        public int Happiness { get; set; }
        public int Hunger { get; set; }
        public DateTimeOffset? AdoptionDate { get; set; }
        public DateTimeOffset? LastPetted { get; set; }
        public DateTimeOffset? LastFed { get; set; }
    }
}
