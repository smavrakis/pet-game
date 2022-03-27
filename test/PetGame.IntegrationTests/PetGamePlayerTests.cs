using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetGame.IntegrationTests.Helpers;
using PetGame.Models;
using PetGame.Persistence;
using PetGame.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace PetGame.IntegrationTests
{
    public class PetGamePlayerTests : PetGameTestsBase
    {
        private readonly PetGameWebApplicationFactory<Program> _factory;

        public PetGamePlayerTests(PetGameWebApplicationFactory<Program> factory) : base(factory)
        {
            _factory = factory;
        }

        [Theory, AutoData]
        public async Task Given_new_player_request_When_calling_post_Then_should_persist_player(PostPlayerRequest request)
        {
            // Given
            request.Email = "test@test.com";
            var httpMessage = new HttpRequestMessage(HttpMethod.Post, "/player");
            httpMessage.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            // When
            var result = await HttpClient.SendAsync(httpMessage);

            // Then
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<PostResponse>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<GameContext>();
                var player = await dbContext.FindAsync<Player>(response.Id);
                player.Should().NotBeNull();
                player.FirstName.Should().Be(request.FirstName);
                player.MiddleName.Should().Be(request.MiddleName);
                player.LastName.Should().Be(request.LastName);
                player.UserName.Should().Be(request.UserName);
                player.Email.Should().Be(request.Email);
                player.RegistrationDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
                player.Pets.Should().BeEmpty();
            }
        }

        [Theory, AutoData]
        public async Task Given_existing_player_When_calling_get_Then_should_return_player(Player player)
        {
            // Given
            player.ID = 0;
            player.Pets = new List<Pet>();

            using (var scope = _factory.Services.CreateScope())
            {                
                var dbContext = scope.ServiceProvider.GetService<GameContext>();
                dbContext.Players.Add(player);
                await dbContext.SaveChangesAsync();                
            }
            
            var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/player/{player.ID}");

            // When
            var result = await HttpClient.SendAsync(httpMessage);

            // Then
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<Player>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            response.Should().BeEquivalentTo(player);
        }
    }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.