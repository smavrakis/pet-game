using PetGame.Common.Constants;
using PetGame.Exceptions;
using PetGame.Models;
using PetGame.Persistence;
using PetGame.Persistence.Models;
using SerilogTimings.Extensions;
using ILogger = Serilog.ILogger;

namespace PetGame.Services
{
    public class PetService : IPetService
    {
        private readonly ILogger _logger;
        private readonly GameContext _databaseContext;
        private readonly IPetSatisfactionService _petSatisfactionService;

        public PetService(ILogger logger, GameContext databaseContext, IPetSatisfactionService petSatisfactionService)
        {
            _logger = logger.ForContext<PetService>();
            _databaseContext = databaseContext;
            _petSatisfactionService = petSatisfactionService;
        }

        public async Task<PostResponse> AdoptPetAsync(PostPetRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (_logger.TimeOperation("Adopting new pet"))
            {
                var player = _databaseContext.Players.FirstOrDefault(p => p.ID == request.PlayerId);

                if (player == null)
                {
                    throw new ResourceNotFoundException("Player ID not found");
                }

                var now = DateTimeOffset.UtcNow;
                var pet = new Pet
                {
                    Name = request.Name,
                    Type = request.Type,
                    Happiness = PetSatisfactionStats.NeutralValue,
                    Hunger = PetSatisfactionStats.NeutralValue,
                    AdoptionDate = now,
                    LastPetted = now,
                    LastFed = now
                };

                player.Pets.Add(pet);
                await _databaseContext.SaveChangesAsync(cancellationToken);

                return new PostResponse { ID = pet.ID };
            }
        }

        public async Task<Pet> GetPetAsync(int id)
        {
            using (_logger.TimeOperation("Getting pet"))
            {
                var pet = await _databaseContext.FindAsync<Pet>(id);

                if (pet == null)
                {
                    throw new ResourceNotFoundException("Pet ID not found");
                }

                _petSatisfactionService.UpdatePetSatisfactionStats(pet);

                return pet;
            }
        }
    }
}
