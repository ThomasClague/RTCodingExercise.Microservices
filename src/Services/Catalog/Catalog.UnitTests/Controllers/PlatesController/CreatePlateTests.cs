using Catalog.API.Controllers;
using Catalog.API.Contracts.Services;
using Catalog.Domain;
using Contracts.Plates;
using Contracts.Primitives;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Catalog.UnitTests.Controllers.PlatesControllerTests
{
    public class CreatePlateTests
    {
        private readonly Mock<IPlatesService> _platesServiceMock;
        private readonly PlatesController _sut;

        public CreatePlateTests()
        {
            _platesServiceMock = new Mock<IPlatesService>();
            _sut = new PlatesController(_platesServiceMock.Object);
        }

        [Fact]
        public async Task CreatePlate_WhenResultIsSuccess_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var command = new CreatePlateCommand
            {
                Registration = "ABC123",
                PurchasePrice = 1000m,
                SalePrice = 1500m
            };

            var plate = Plate.Create(command.Registration, command.PurchasePrice, command.SalePrice);

            _platesServiceMock
                .Setup(x => x.CreatePlateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Plate>.Success(plate));

            // Act
            var result = await _sut.CreatePlate(command, default);

            // Assert
            var createdAtResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtResult.ActionName.Should().Be(nameof(PlatesController.GetPlateById));
            createdAtResult.RouteValues["id"].Should().Be(plate.Id);
            createdAtResult.Value.Should().Be(plate);
        }

        [Fact]
        public async Task CreatePlate_WhenInvalidRegistration_ShouldReturnBadRequestWithErrorMessage()
        {
            // Arrange
            var command = new CreatePlateCommand
            {
                Registration = "invalid",
                PurchasePrice = 1000m,
                SalePrice = 1500m
            };

            _platesServiceMock
                .Setup(x => x.CreatePlateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Plate>.Failure("Invalid registration format. Expected format: ABC123"));

            // Act
            var result = await _sut.CreatePlate(command, default);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Invalid registration format. Expected format: ABC123");
        }

        [Fact]
        public async Task CreatePlate_WhenMultipleValidationErrors_ShouldReturnBadRequestWithErrorList()
        {
            // Arrange
            var command = new CreatePlateCommand
            {
                Registration = "",
                PurchasePrice = -1m,
                SalePrice = -2m
            };

            var expectedErrors = new[]
            {
                "Registration cannot be empty",
                "Purchase price must be greater than zero",
                "Sale price must be greater than zero",
                "Invalid registration format. Expected format: ABC123"
            };

            _platesServiceMock
                .Setup(x => x.CreatePlateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Plate>.Failure(expectedErrors));

            // Act
            var result = await _sut.CreatePlate(command, default);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task CreatePlate_WhenDatabaseError_ShouldReturnBadRequestWithErrorMessage()
        {
            // Arrange
            var command = new CreatePlateCommand
            {
                Registration = "ABC123",
                PurchasePrice = 1000m,
                SalePrice = 1500m
            };

            var errorMessage = "Failed to create plate: Database error";
            _platesServiceMock
                .Setup(x => x.CreatePlateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Plate>.Failure(errorMessage));

            // Act
            var result = await _sut.CreatePlate(command, default);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be(errorMessage);
        }
    }
} 