using Catalog.API.Contracts.Data;
using Catalog.API.Services;
using Catalog.API.Specifications.Plates;
using Catalog.Domain;
using Catalog.UnitTests.Fakers;
using Contracts.Pagination;
using Contracts.Plates;
using FluentAssertions;
using MassTransit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests.Services.PlateService
{
    public class GetPlatesAsyncTests
    {
        private readonly Mock<ICatalogRepository<Plate>> _repositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly PlatesService _sut;
        private readonly PlateFaker _plateFaker;

        public GetPlatesAsyncTests()
        {
            _repositoryMock = new Mock<ICatalogRepository<Plate>>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _plateFaker = new PlateFaker();

            _sut = new PlatesService(
                _repositoryMock.Object,
                _publishEndpointMock.Object);
        }

        [Fact]
        public async Task GetPlatesAsync_WhenCalled_ShouldCallRepositoryWithCorrectSpecification()
        {
            // Arrange
            var filter = new PlatesFilter { Page = 1, PageSize = 10 };
            var isAdmin = false;
            var plates = _plateFaker.Generate(5);
            var pagedResponse = new PagedResponse<Plate>(plates, new Pagination(5, filter));

            _repositoryMock
                .Setup(x => x.PaginatedAsync(
                    It.IsAny<GetPlatesSpecification>(),
                    It.IsAny<PlatesFilter>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _sut.GetPlatesAsync(filter, isAdmin);

            // Assert
            _repositoryMock.Verify(
                x => x.PaginatedAsync(
                    It.Is<GetPlatesSpecification>(spec => 
                        spec != null),
                    filter,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1, 10, 30, 10)]
        [InlineData(2, 10, 30, 10)]
        [InlineData(3, 10, 30, 10)]
        [InlineData(1, 15, 30, 15)]
        [InlineData(2, 10, 15, 5)]
        public async Task GetPlatesAsync_ShouldReturnCorrectPagination(
            int page,
            int pageSize,
            int totalItems,
            int expectedItemCount)
        {
            // Arrange
            var filter = new PlatesFilter { Page = page, PageSize = pageSize };
            var isAdmin = false;
            var allPlates = _plateFaker.Generate(totalItems);
            var pagedPlates = allPlates
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResponse = new PagedResponse<Plate>(
                pagedPlates,
                new Pagination(totalItems, filter));

            _repositoryMock
                .Setup(x => x.PaginatedAsync(
                    It.IsAny<GetPlatesSpecification>(),
                    It.IsAny<PlatesFilter>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _sut.GetPlatesAsync(filter, isAdmin);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(expectedItemCount);
            result.Pagination.Should().NotBeNull();
            result.Pagination.Page.Should().Be(page);
            result.Pagination.PageSize.Should().Be(pageSize);
            result.Pagination.TotalItems.Should().Be(totalItems);
            result.Pagination.TotalPages.Should().Be((int)Math.Ceiling(totalItems / (double)pageSize));
            result.Pagination.HasPrevious.Should().Be(page > 1);
            result.Pagination.HasNext.Should().Be(page < result.Pagination.TotalPages);
        }

        [Fact]
        public async Task GetPlatesAsync_WhenNoPlatesExist_ShouldReturnEmptyList()
        {
            // Arrange
            var filter = new PlatesFilter { Page = 1, PageSize = 10 };
            var isAdmin = false;
            var pagedResponse = new PagedResponse<Plate>(
                new List<Plate>(),
                new Pagination(0, filter));

            _repositoryMock
                .Setup(x => x.PaginatedAsync(
                    It.IsAny<GetPlatesSpecification>(),
                    It.IsAny<PlatesFilter>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _sut.GetPlatesAsync(filter, isAdmin);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
            result.Pagination.TotalItems.Should().Be(0);
            result.Pagination.TotalPages.Should().Be(1);
            result.Pagination.HasNext.Should().BeFalse();
            result.Pagination.HasPrevious.Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 10)]
        [InlineData(1, -10)]
        public async Task GetPlatesAsync_WithInvalidPagination_ShouldUseDefaultValues(
            int page,
            int pageSize)
        {
            // Arrange
            var filter = new PlatesFilter { Page = page, PageSize = pageSize };
            var isAdmin = false;
            var plates = _plateFaker.Generate(5);
            var pagedResponse = new PagedResponse<Plate>(
                plates,
                new Pagination(5, filter));

            _repositoryMock
                .Setup(x => x.PaginatedAsync(
                    It.IsAny<GetPlatesSpecification>(),
                    It.IsAny<PlatesFilter>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _sut.GetPlatesAsync(filter, isAdmin);

            // Assert
            result.Should().NotBeNull();
            result.Pagination.Page.Should().BeGreaterThan(0);
            result.Pagination.PageSize.Should().BeGreaterThan(0);
        }
    }
}
