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
    public class PetControllerTests
    {
        private readonly Mock<IPetService> _petService;
        private readonly PetController _sut;

        public PetControllerTests()
        {
            _petService = new Mock<IPetService>(MockBehavior.Strict);
            _sut = new PetController(_petService.Object);
        }

        [Theory, AutoData]
        public async Task Given_invalid_request_When_calling_post_Then_should_return_400(PostPetRequest request)
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
        public async Task Given_valid_request_When_calling_post_Then_should_return_201(PostPetRequest request, PostResponse response, CancellationToken cancellationToken)
        {
            // Given
            _petService.Setup(s => s.AdoptPetAsync(request, cancellationToken)).ReturnsAsync(response);

            // When
            var result = await _sut.PostAsync(request, cancellationToken);

            // Then
            result.Should().BeOfType<CreatedAtActionResult>();
            result.As<CreatedAtActionResult>().StatusCode.Should().Be(201);
            result.As<CreatedAtActionResult>().Value.Should().Be(response);
            VerifyMocks();
        }

        [Theory, AutoData]
        public async Task Given_valid_request_When_calling_get_Then_should_return_200(int id, Pet response)
        {
            // Given
            _petService.Setup(s => s.GetPetAsync(id)).ReturnsAsync(response);

            // When
            var result = await _sut.GetAsync(id);

            // Then            
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().StatusCode.Should().Be(200);
            result.As<OkObjectResult>().Value.Should().Be(response);
            VerifyMocks();
        }

        [Theory, AutoData]
        public async Task Given_invalid_request_When_calling_put_Then_should_return_400(int id, PutPetRequest request)
        {
            // Given
            _sut.ModelState.AddModelError("key", "error message");

            // When
            var result = await _sut.PutAsync(id, request);

            // Then
            result.Should().BeOfType<BadRequestObjectResult>();
            result.As<BadRequestObjectResult>().StatusCode.Should().Be(400);
            VerifyMocks();
        }

        [Theory, AutoData]
        public async Task Given_valid_request_When_calling_put_Then_should_return_204(int id, PutPetRequest request, CancellationToken cancellationToken)
        {
            // Given
            _petService.Setup(s => s.UpdatePetAsync(id, request, cancellationToken)).Returns(Task.CompletedTask);

            // When
            var result = await _sut.PutAsync(id, request, cancellationToken);

            // Then
            result.Should().BeOfType<NoContentResult>();
            result.As<NoContentResult>().StatusCode.Should().Be(204);
            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _petService.VerifyAll();
        }
    }
}
