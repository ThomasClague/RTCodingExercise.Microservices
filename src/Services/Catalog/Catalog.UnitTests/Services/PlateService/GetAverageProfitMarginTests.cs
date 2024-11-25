using Catalog.API.Contracts.Data;
using Catalog.API.Services;
using Catalog.API.Specifications.Plates;
using Catalog.Domain;
using FluentAssertions;
using MassTransit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests.Services.PlateService
{
    public class GetAverageProfitMarginTests
    {
        private readonly Mock<ICatalogRepository<Plate>> _repositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly PlatesService _sut;

        public GetAverageProfitMarginTests()
        {
            _repositoryMock = new Mock<ICatalogRepository<Plate>>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _sut = new PlatesService(_repositoryMock.Object, _publishEndpointMock.Object);
        }

        [Fact]
        public async Task GetAverageProfitMarginAsync_WithNoPlates_ReturnsZero()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.ListAsync(It.IsAny<GetAllPlatesSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Plate>());

            // Act
            var result = await _sut.GetAverageProfitMarginAsync();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task GetAverageProfitMarginAsync_WithSinglePlate_ReturnsCorrectMargin()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 1000, 1200);
            _repositoryMock
                .Setup(x => x.ListAsync(It.IsAny<GetAllPlatesSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Plate> { plate });

            // Act
            var result = await _sut.GetAverageProfitMarginAsync();

            // Assert
            result.Should().Be(20);
        }

        [Fact]
        public async Task GetAverageProfitMarginAsync_WithMultiplePlates_ReturnsAverageMargin()
        {
            // Arrange
            var plates = new List<Plate>
            {
                Plate.Create("ABC123", 1000, 1200),
                Plate.Create("DEF456", 2000, 3000),
                Plate.Create("GHI789", 3000, 3900) 
            };

            _repositoryMock
                .Setup(x => x.ListAsync(It.IsAny<GetAllPlatesSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(plates);

            // Act
            var result = await _sut.GetAverageProfitMarginAsync();

            // Assert
            result.Should().Be(33.33m);
        }

        [Fact]
        public async Task GetAverageProfitMarginAsync_WithDifferentPriceRanges_HandlesDecimalsCorrectly()
        {
            // Arrange
            var plates = new List<Plate>
            {
                Plate.Create("ABC123", 100, 150),
                Plate.Create("DEF456", 1000, 1250),
                Plate.Create("GHI789", 10000, 11000)
            };

            _repositoryMock
                .Setup(x => x.ListAsync(It.IsAny<GetAllPlatesSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(plates);

            // Act
            var result = await _sut.GetAverageProfitMarginAsync();

            // Assert
            result.Should().Be(28.33m);
        }

        [Fact]
        public async Task GetAverageProfitMarginAsync_UsesCorrectSpecification()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.ListAsync(It.IsAny<GetAllPlatesSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Plate>());

            // Act
            await _sut.GetAverageProfitMarginAsync();

            // Assert
            _repositoryMock.Verify(
                x => x.ListAsync(
                    It.Is<GetAllPlatesSpecification>(spec => spec != null),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
} 