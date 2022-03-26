using System.ComponentModel.DataAnnotations;

namespace PetGame.Configuration
{
    public class PetSatisfactionRatesSettings
    {
        public Dictionary<string, TimeSpan> Happiness { get; set; } = new Dictionary<string, TimeSpan>();
        public Dictionary<string, TimeSpan> Hunger { get; set; } = new Dictionary<string, TimeSpan>();

        public void ValidateAndThrow()
        {
            foreach (var rate in Happiness)
            {
                if (rate.Value <= TimeSpan.FromSeconds(1))
                {
                    throw new ValidationException($"Rates need to have a minimum value of 1 second");
                }
            }

            foreach (var rate in Hunger)
            {
                if (rate.Value <= TimeSpan.FromSeconds(1))
                {
                    throw new ValidationException($"Rates need to have a minimum value of 1 second");
                }
            }
        }
    }
}
