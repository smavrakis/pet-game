using PetGame.Services;

namespace PetGame.Configuration
{
    public static class ApiServices
    {
        public static void AddApiServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var petSatisfactionSection = configuration.GetSection("PetSatisfactionRates");
            serviceCollection.Configure<PetSatisfactionRatesSettings>(petSatisfactionSection);

            var petSatisfactionSettings = petSatisfactionSection.Get<PetSatisfactionRatesSettings>();
            petSatisfactionSettings.ValidateAndThrow();

            serviceCollection.AddScoped<IPlayerService, PlayerService>();
            serviceCollection.AddScoped<IPetService, PetService>();
            serviceCollection.AddScoped<IPetSatisfactionService, PetSatisfactionService>();
        }
    }
}
