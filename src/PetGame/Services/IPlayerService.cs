using PetGame.Models;

namespace PetGame.Services
{
    public interface IPlayerService
    {
        Task<PostPlayerResponse> CreatePlayerAsync(PostPlayerRequest playerRequest, CancellationToken cancellationToken = default);
    }
}
