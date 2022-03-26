using PetGame.Models;
using PetGame.Persistence.Models;

namespace PetGame.Services
{
    public interface IPetService
    {
        Task<PostResponse> AdoptPetAsync(PostPetRequest request, CancellationToken cancellationToken = default);
        Task<Pet> GetPetAsync(int id);
    }
}
