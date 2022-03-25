using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PetGame.Persistence.Configuration
{
    public static class PersistenceServices
    {
        public static void AddPersistenceServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var persistenceSection = configuration.GetSection("Persistence");
            serviceCollection.Configure<PersistenceSettings>(persistenceSection);

            var persistenceSettings = persistenceSection.Get<PersistenceSettings>();
            serviceCollection.AddDbContext<GameContext>(options => options.UseSqlServer(persistenceSettings.ConnectionString));            
        }
    }
}
