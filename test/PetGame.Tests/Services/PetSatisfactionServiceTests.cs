using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using PetGame.Common.Constants;
using PetGame.Configuration;
using PetGame.Persistence.Models;
using PetGame.Services;
using Serilog;
using System;
using System.Collections.Generic;
using Xunit;

namespace PetGame.Tests.Services
{
    public class PetSatisfactionServiceTests
    {
        private readonly Mock<ILogger> _logger;
        private readonly Mock<IOptions<PetSatisfactionRatesSettings>> _settings;
        private readonly IPetSatisfactionService _sut;

        private readonly Dictionary<string, TimeSpan> _exampleRates;

        public PetSatisfactionServiceTests()
        {
            _logger = new Mock<ILogger>(MockBehavior.Loose);
            _logger.Setup(l => l.ForContext<PetSatisfactionService>()).Returns(_logger.Object);

            var timeSpan = TimeSpan.FromSeconds(1);
            _exampleRates = new Dictionary<string, TimeSpan> { { "Dog", timeSpan }, { "Cat", timeSpan }, { "Parrot", timeSpan } };
            _settings = new Mock<IOptions<PetSatisfactionRatesSettings>>(MockBehavior.Strict);
            _settings.Setup(s => s.Value).Returns(new PetSatisfactionRatesSettings { Happiness = _exampleRates, Hunger = _exampleRates });

            _sut = new PetSatisfactionService(_logger.Object, _settings.Object);
        }

        [Fact]
        public void Given_null_pet_When_updating_stats_Then_should_throw()
        {
            // Given
            // When
            var action = () => _sut.UpdatePetSatisfactionStats(null!);

            // Then
            action.Should().ThrowExactly<ArgumentNullException>();
            VerifyMocks();
        }

        [Theory, AutoData]
        public void Given_missing_happiness_rates_When_updating_stats_Then_should_throw(Pet pet)
        {
            // Given
            _settings.Setup(s => s.Value).Returns(new PetSatisfactionRatesSettings { Hunger = _exampleRates });
            var sut = new PetSatisfactionService(_logger.Object, _settings.Object);

            // When
            var action = () => sut.UpdatePetSatisfactionStats(pet);

            // Then
            action.Should().ThrowExactly<InvalidOperationException>();
            VerifyMocks();
        }

        [Theory, AutoData]
        public void Given_missing_hunger_rates_When_updating_stats_Then_should_throw(Pet pet)
        {
            // Given
            _settings.Setup(s => s.Value).Returns(new PetSatisfactionRatesSettings { Happiness = _exampleRates });
            var sut = new PetSatisfactionService(_logger.Object, _settings.Object);

            // When
            var action = () => sut.UpdatePetSatisfactionStats(pet);

            // Then
            action.Should().ThrowExactly<InvalidOperationException>();
            VerifyMocks();
        }

        [Theory, AutoData]
        public void Given_missing_last_petted_date_When_updating_stats_Then_should_throw(Pet pet)
        {
            // Given
            pet.LastPetted = null;

            // When
            var action = () => _sut.UpdatePetSatisfactionStats(pet);

            // Then
            action.Should().ThrowExactly<InvalidOperationException>();
            VerifyMocks();
        }

        [Theory, AutoData]
        public void Given_missing_last_fed_date_When_updating_stats_Then_should_throw(Pet pet)
        {
            // Given
            pet.LastFed = null;

            // When
            var action = () => _sut.UpdatePetSatisfactionStats(pet);

            // Then
            action.Should().ThrowExactly<InvalidOperationException>();
            VerifyMocks();
        }

        [Theory, AutoData]
        public void Given_pet_When_updating_stats_Then_should_update_stats_correctly(Pet pet)
        {
            // Given
            var currentHappiness = PetSatisfactionStats.MaxValue;
            var currentHunger = PetSatisfactionStats.MinValue;
            pet.Happiness = currentHappiness;
            pet.Hunger = currentHunger;
            pet.LastPetted = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(5);
            pet.LastFed = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(10);

            // When
            _sut.UpdatePetSatisfactionStats(pet);

            // Then
            pet.Happiness.Should().Be(currentHappiness - 5);
            pet.Hunger.Should().Be(currentHunger + 10);
            VerifyMocks();
        }

        [Theory, AutoData]
        public void Given_pet_with_low_stats_When_updating_stats_Then_should_not_go_over_the_limits(Pet pet)
        {
            // Given
            var currentHappiness = PetSatisfactionStats.MinValue;
            var currentHunger = PetSatisfactionStats.MaxValue;
            pet.Happiness = currentHappiness;
            pet.Hunger = currentHunger;
            pet.LastPetted = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(5);
            pet.LastFed = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(10);

            // When
            _sut.UpdatePetSatisfactionStats(pet);

            // Then
            pet.Happiness.Should().Be(PetSatisfactionStats.MinValue);
            pet.Hunger.Should().Be(PetSatisfactionStats.MaxValue);
            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _settings.VerifyAll();
        }
    }
}
