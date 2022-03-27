using Microsoft.Extensions.Options;
using PetGame.Common.Constants;
using PetGame.Configuration;
using PetGame.Persistence.Models;
using ILogger = Serilog.ILogger;

namespace PetGame.Services
{
    public class PetSatisfactionService : IPetSatisfactionService
    {
        private readonly ILogger _logger;
        private readonly PetSatisfactionRatesSettings _settings;

        public PetSatisfactionService(ILogger logger, IOptions<PetSatisfactionRatesSettings> settings)
        {
            _logger = logger.ForContext<PetSatisfactionService>();
            _settings = settings.Value;
        }

        public void UpdatePetSatisfactionStats(Pet pet)
        {
            if (pet == null)
            {
                throw new ArgumentNullException(nameof(pet));
            }

            _logger.Information("Updating pet satisfaction stats");

            var petType = pet.Type.ToString();
            if (!_settings.Happiness.ContainsKey(petType) || !_settings.Hunger.ContainsKey(petType))
            {
                throw new InvalidOperationException($"Could not find satisfaction rates for pet type [{petType}]");
            }

            if (pet.LastPetted == null || pet.LastFed == null)
            {
                throw new InvalidOperationException($"Missing pet satisfaction stats for pet with ID [{pet.ID}]");
            }

            var now = DateTimeOffset.UtcNow;
            pet.Happiness -= (int)(now - pet.LastPetted.Value).Divide(_settings.Happiness[petType]);

            if (pet.Happiness < PetSatisfactionStats.MinValue)
            {
                pet.Happiness = PetSatisfactionStats.MinValue;
            }

            pet.Hunger += (int)(now - pet.LastFed.Value).Divide(_settings.Hunger[petType]);

            if (pet.Hunger > PetSatisfactionStats.MaxValue)
            {
                pet.Hunger = PetSatisfactionStats.MaxValue;
            }
        }
    }
}
