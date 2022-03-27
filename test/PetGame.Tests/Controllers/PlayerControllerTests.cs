using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetGame.Controllers;
using PetGame.Models;
using PetGame.Persistence.Models;
using PetGame.Services;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PetGame.Tests.Controllers
{
    public class PlayerControllerTests
    {
        private readonly Mock<IPlayerService> _playerService;
        private readonly PlayerController _sut;

        public PlayerControllerTests()
        {
            _playerService = new Mock<IPlayerService>(MockBehavior.Strict);
            _sut = new PlayerController(_playerService.Object);
        }

        [Theory, AutoData]
        public async Task Given_invalid_request_When_calling_post_Then_should_return_400(PostPlayerRequest request)
        {
            // Given
            _sut.ModelState.AddModelError("key", "error message");

            // When
            var result = await _sut.PostAsync(request);

            // Then
            result.Should().BeOfType<BadRequestObjectResult>();
            result.As<BadRequestObjectResult>().StatusCode.Should().Be(400);
            VerifyMocks();
        }

        [Theory, AutoData]
        public async Task Given_valid_request_When_calling_post_Then_should_return_201(PostPlayerRequest request, PostResponse response, CancellationToken cancellationToken)
        {
            // Given
            _playerService.Setup(s => s.CreatePlayerAsync(request, cancellationToken)).ReturnsAsync(response);

            // When
            var result = await _sut.PostAsync(request, cancellationToken);

            // Then
            result.Should().BeOfType<CreatedAtActionResult>();
            result.As<CreatedAtActionResult>().StatusCode.Should().Be(201);
            result.As<CreatedAtActionResult>().Value.Should().Be(response);
            VerifyMocks();
        }

        [Theory, AutoData]
        public async Task Given_valid_request_When_calling_get_Then_should_return_200(int id, Player response, CancellationToken cancellationToken)
        {
            // Given
            _playerService.Setup(s => s.GetPlayerAsync(id, cancellationToken)).ReturnsAsync(response);

            // When
            var result = await _sut.GetAsync(id, cancellationToken);

            // Then            
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().StatusCode.Should().Be(200);
            result.As<OkObjectResult>().Value.Should().Be(response);
            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _playerService.VerifyAll();
        }
    }
}
