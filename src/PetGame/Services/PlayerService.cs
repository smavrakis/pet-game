using Microsoft.EntityFrameworkCore;
using PetGame.Exceptions;
using PetGame.Models;
using PetGame.Persistence;
using PetGame.Persistence.Models;
using SerilogTimings.Extensions;
using ILogger = Serilog.ILogger;

namespace PetGame.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly ILogger _logger;
        private readonly GameContext _databaseContext;
        private readonly IPetSatisfactionService _petSatisfactionService;

        public PlayerService(ILogger logger, GameContext databaseContext, IPetSatisfactionService petSatisfactionService)
        {
            _logger = logger.ForContext<PlayerService>();
            _databaseContext = databaseContext;
            _petSatisfactionService = petSatisfactionService;
        }

        public async Task<PostResponse> CreatePlayerAsync(PostPlayerRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (_logger.TimeOperation("Creating new player"))
            {
                var player = new Player
                {
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    Email = request.Email,
                    RegistrationDate = DateTimeOffset.UtcNow
                };

                _databaseContext.Players.Add(player);
                await _databaseContext.SaveChangesAsync(cancellationToken);

                return new PostResponse { ID = player.ID };
            }
        }

        public async Task<Player> GetPlayerAsync(int id, CancellationToken cancellationToken = default)
        {
            using (_logger.TimeOperation("Getting player"))
            {
                var player = await _databaseContext.Players.Include(p => p.Pets).FirstOrDefaultAsync(p => p.ID == id, cancellationToken);

                if (player == null)
                {
                    throw new ResourceNotFoundException("Player ID not found");
                }

                foreach (var pet in player.Pets)
                {
                    _petSatisfactionService.UpdatePetSatisfactionStats(pet);
                }

                return player;
            }
        }
    }
}
