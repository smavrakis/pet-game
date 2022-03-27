using System.Net.Http;
using Xunit;

namespace PetGame.IntegrationTests.Helpers
{
    public class PetGameTestsBase : IClassFixture<PetGameWebApplicationFactory<Program>>
    {
        protected PetGameTestsBase(PetGameWebApplicationFactory<Program> factory)
        {
            HttpClient = factory.CreateClient();
        }

        protected HttpClient HttpClient { get; set; }
    }
}
