using PetGame.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PetGame.Models
{
    public class PutPetRequest
    {
        [StringLength(Validation.StringMaxLength)]
        public string? Name { get; set; }
        [Range(PetSatisfactionStats.MinValue, PetSatisfactionStats.MaxValue)]
        public int? FeedingPoints { get; set; }
        [Range(PetSatisfactionStats.MinValue, PetSatisfactionStats.MaxValue)]
        public int? PettingPoints { get; set; }
    }
}
