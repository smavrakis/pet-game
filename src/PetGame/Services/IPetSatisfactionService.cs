using PetGame.Persistence.Models;

namespace PetGame.Services
{
    public interface IPetSatisfactionService
    {
        void UpdatePetSatisfactionStats(Pet pet);
    }
}
