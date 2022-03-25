using PetGame.Services;

namespace PetGame.Configuration
{
    public static class ApiServices
    {
        public static void AddApiServices(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddScoped<IPlayerService, PlayerService>();
        }
    }
}
