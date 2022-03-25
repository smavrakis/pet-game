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

        public PlayerService(ILogger logger, GameContext databaseContext)
        {
            _logger = logger.ForContext<PlayerService>();
            _databaseContext = databaseContext;
        }

        public async Task<PostPlayerResponse> CreatePlayerAsync(PostPlayerRequest playerRequest, CancellationToken cancellationToken = default)
        {
            if (playerRequest == null)
            {
                throw new ArgumentNullException(nameof(playerRequest));
            }

            _logger.Information("Creating new player");

            var player = new Player
            {
                FirstName = playerRequest.FirstName,
                MiddleName = playerRequest.MiddleName,
                LastName = playerRequest.LastName,
                UserName = playerRequest.UserName,
                Email = playerRequest.Email,
                RegistrationDate = DateTimeOffset.UtcNow
            };

            using (_logger.TimeOperation("Saving new player to database"))
            {
                _databaseContext.Players.Add(player);
                await _databaseContext.SaveChangesAsync(cancellationToken);
            }

            return new PostPlayerResponse { ID = player.ID };
        }
    }
}
