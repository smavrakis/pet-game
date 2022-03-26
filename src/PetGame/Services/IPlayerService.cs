using PetGame.Models;
using PetGame.Persistence.Models;

namespace PetGame.Services
{
    public interface IPlayerService
    {
        Task<PostResponse> CreatePlayerAsync(PostPlayerRequest playerRequest, CancellationToken cancellationToken = default);
        Task<Player> GetPlayerAsync(int id, CancellationToken cancellationToken = default);
    }
}
