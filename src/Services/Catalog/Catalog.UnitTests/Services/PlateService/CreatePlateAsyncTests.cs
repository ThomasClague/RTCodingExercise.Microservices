using Contracts.Plates;
using FluentAssertions;
using MassTransit;
using Moq;
using Ardalis.Specification;
using Xunit;
using Catalog.API.Contracts.Data;
using Catalog.Domain;
using Catalog.API.Services;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using DomainEvents.Plates;

namespace Catalog.UnitTests.Services.PlateService
{
    public class CreatePlateAsyncTests
    {
        private readonly Mock<ICatalogRepository<Plate>> _repositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly PlatesService _sut;

        public CreatePlateAsyncTests()
        {
            _repositoryMock = new Mock<ICatalogRepository<Plate>>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();

            _sut = new PlatesService(
                _repositoryMock.Object,
                _publishEndpointMock.Object);
        }

        [Theory]
        [InlineData("ABC123", "ABC", 123)]
        [InlineData("XYZ789", "XYZ", 789)]
        [InlineData("AB12", "AB", 12)]
        public async Task CreatePlateAsync_WhenCommandIsValid_ShouldCreatePlateWithDomainEvent(
            string registration, string expectedLetters, int expectedNumbers)
        {
            // Arrange
            var command = new CreatePlateCommand
            {
                Registration = registration,
                PurchasePrice = 1000m,
                SalePrice = 1500m
            };

            Plate capturedPlate = null;
            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Plate>(), It.IsAny<CancellationToken>()))
                .Callback<Plate, CancellationToken>((plate, _) => capturedPlate = plate)
                .ReturnsAsync((Plate plate, CancellationToken _) => plate);

            // Act
            var result = await _sut.CreatePlateAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            
            // Verify plate properties
            result.Value.Registration.Should().Be(command.Registration);
            result.Value.PurchasePrice.Should().Be(command.PurchasePrice);
            result.Value.SalePrice.Should().Be(command.SalePrice);
            result.Value.Letters.Should().Be(expectedLetters);
            result.Value.Numbers.Should().Be(expectedNumbers);

            // Verify domain event
            capturedPlate.Should().NotBeNull();
            capturedPlate.DomainEvents.Should().ContainSingle();
            var domainEvent = capturedPlate.DomainEvents.First() as PlateCreatedEvent;
            domainEvent.Should().NotBeNull();
            domainEvent.Id.Should().Be(capturedPlate.Id);
            domainEvent.Registration.Should().Be(command.Registration);
            domainEvent.PurchasePrice.Should().Be(command.PurchasePrice);
            domainEvent.SalePrice.Should().Be(command.SalePrice);

            _repositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<Plate>(p =>
                        p.Registration == command.Registration &&
                        p.PurchasePrice == command.PurchasePrice &&
                        p.SalePrice == command.SalePrice &&
                        p.Letters == expectedLetters &&
                        p.Numbers == expectedNumbers),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData("AB*123")]
        public async Task CreatePlateAsync_WhenRegistrationFormatIsInvalid_ShouldReturnFailureResult(string invalidRegistration)
        {
            // Arrange
            var command = new CreatePlateCommand
            {
                Registration = invalidRegistration,
                PurchasePrice = 1000m,
                SalePrice = 1500m
            };

            // Act
            var result = await _sut.CreatePlateAsync(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().ContainSingle()
                .Which.Should().Be("Invalid registration format. Expected format: ABC123");

            VerifyNoInteractions();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99.99)]
        public async Task CreatePlateAsync_WhenPricesAreInvalid_ShouldReturnFailureResult(decimal invalidPrice)
        {
            // Arrange
            var command = new CreatePlateCommand
            {
                Registration = "ABC123",
                PurchasePrice = invalidPrice,
                SalePrice = invalidPrice
            };

            // Act
            var result = await _sut.CreatePlateAsync(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain("Purchase price must be greater than zero");
            result.Errors.Should().Contain("Sale price must be greater than zero");

            VerifyNoInteractions();
        }

        [Fact]
        public async Task CreatePlateAsync_WhenSalePriceIsLessThanPurchasePrice_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new CreatePlateCommand
            {
                Registration = "ABC123",
                PurchasePrice = 1500m,
                SalePrice = 1000m
            };

            // Act
            var result = await _sut.CreatePlateAsync(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().ContainSingle()
                .Which.Should().Be("Sale price must be greater than purchase price");

            VerifyNoInteractions();
        }

        [Fact]
        public async Task CreatePlateAsync_WhenRepositoryThrows_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new CreatePlateCommand
            {
                Registration = "ABC123",
                PurchasePrice = 1000m,
                SalePrice = 1500m
            };

            var exceptionMessage = "Database error";
            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Plate>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _sut.CreatePlateAsync(command);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().ContainSingle()
                .Which.Should().Be($"Failed to create plate: {exceptionMessage}");

            _publishEndpointMock.Verify(
                x => x.Publish(It.IsAny<PlateCreatedEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private void VerifyNoInteractions()
        {
            _repositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Plate>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
} 