using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetGame.Common.Constants;
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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace PetGame.IntegrationTests
{
    public class PetGamePetTests : PetGameTestsBase
    {
        private readonly PetGameWebApplicationFactory<Program> _factory;

        public PetGamePetTests(PetGameWebApplicationFactory<Program> factory) : base(factory)
        {
            _factory = factory;
        }

        [Theory, AutoData]
        public async Task Given_new_pet_request_When_calling_post_Then_should_persist_pet(PostPetRequest request, Player player)
        {
            // Given
            player.ID = 0;
            player.Pets = new List<Pet>();

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<GameContext>();
            dbContext.Players.Add(player);
            await dbContext.SaveChangesAsync();

            request.PlayerId = player.ID;
            var httpMessage = new HttpRequestMessage(HttpMethod.Post, "/pet");
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

            var pet = await dbContext.FindAsync<Pet>(response.Id);
            pet.Should().NotBeNull();
            pet.Name.Should().Be(request.Name);
            pet.Type.Should().Be(request.Type);
            pet.Happiness.Should().Be(PetSatisfactionStats.NeutralValue);
            pet.Hunger.Should().Be(PetSatisfactionStats.NeutralValue);
            pet.AdoptionDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
            pet.LastPetted.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
            pet.LastFed.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));

            player.Pets.Should().HaveCount(1);
        }

        [Theory, AutoData]
        public async Task Given_existing_pet_When_calling_get_Then_should_return_pet_with_adjusted_stats(Player player, Pet pet)
        {
            // Given
            player.ID = 0;
            player.Pets = new List<Pet>();

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<GameContext>();

                dbContext.Players.Add(player);
                await dbContext.SaveChangesAsync();

                pet.ID = 0;
                pet.Happiness = PetSatisfactionStats.NeutralValue;
                pet.Hunger = PetSatisfactionStats.NeutralValue;
                pet.AdoptionDate = DateTimeOffset.UtcNow;
                pet.LastPetted = DateTimeOffset.UtcNow;
                pet.LastFed = DateTimeOffset.UtcNow;

                player.Pets.Add(pet);
                await dbContext.SaveChangesAsync();
            }

            var httpMessage = new HttpRequestMessage(HttpMethod.Get, $"/pet/{pet.ID}");

            // When
            var result = await HttpClient.SendAsync(httpMessage);

            // Then
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await result.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new JsonStringEnumConverter());
            var response = JsonSerializer.Deserialize<Pet>(content, options);

            response.Should().BeEquivalentTo(pet, options => options.Excluding(p => p.Happiness).Excluding(p => p.Hunger));
            response.Happiness.Should().BeLessThan(pet.Happiness);
            response.Hunger.Should().BeGreaterThan(pet.Hunger);
        }

        [Theory, AutoData]
        public async Task Given_existing_pet_When_calling_put_Then_should_update_pet(Player player, Pet pet)
        {
            // Given
            player.ID = 0;
            player.Pets = new List<Pet>();

            using var scope = _factory.Services.CreateScope();
            {
                var dbContext = scope.ServiceProvider.GetService<GameContext>();

                dbContext.Players.Add(player);
                await dbContext.SaveChangesAsync();

                pet.ID = 0;
                pet.Happiness = PetSatisfactionStats.NeutralValue;
                pet.Hunger = PetSatisfactionStats.NeutralValue;
                pet.AdoptionDate = DateTimeOffset.UtcNow;
                pet.LastPetted = DateTimeOffset.UtcNow;
                pet.LastFed = DateTimeOffset.UtcNow;

                player.Pets.Add(pet);
                await dbContext.SaveChangesAsync();
            }

            var request = new PutPetRequest { Name = "new", FeedingPoints = 100, PettingPoints = 100 };
            var httpMessage = new HttpRequestMessage(HttpMethod.Put, $"/pet/{pet.ID}");
            httpMessage.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            // When
            var result = await HttpClient.SendAsync(httpMessage);

            // Then
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var newScope = _factory.Services.CreateScope();
            {
                var dbContext = newScope.ServiceProvider.GetService<GameContext>();
                var updatedPet = await dbContext.FindAsync<Pet>(pet.ID);
                updatedPet.Name.Should().Be(request.Name);
                updatedPet.Happiness.Should().BeGreaterThan(pet.Happiness);
                updatedPet.Hunger.Should().BeLessThan(pet.Hunger);
            }
        }
    }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
