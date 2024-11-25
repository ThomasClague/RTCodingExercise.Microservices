using FluentAssertions;
using Xunit;
using System.Linq;
using System;
using DomainEvents.Plates;
using Catalog.Domain.Exceptions;
using Catalog.Domain;

namespace Catalog.UnitTests.Domain.PlateTests
{
    public class CreatePlateTests
    {
        [Theory]
        [InlineData(1000)]
        [InlineData(1500)]
        [InlineData(2000)]
        public void MarkedUpSalePrice_ShouldAdd20PercentToSalePrice(decimal salePrice)
        {
            // Arrange
            var plate = Plate.Create("ABC123", salePrice / 2, salePrice); 
            var expectedMarkedUpPrice = salePrice * 1.2m;

            // Assert
            plate.DisplayPrice.Should().Be(expectedMarkedUpPrice);
        }

        [Fact]
        public void Create_WithValidValues_ShouldCreatePlate()
        {
            // Arrange
            var registration = "ABC123";
            var purchasePrice = 1000m;
            var salePrice = 1500m;

            // Act
            var plate = Plate.Create(registration, purchasePrice, salePrice);

            // Assert
            plate.Should().NotBeNull();
            plate.Registration.Should().Be("ABC123");
            plate.PurchasePrice.Should().Be(purchasePrice);
            plate.SalePrice.Should().Be(salePrice);
            plate.DisplayPrice.Should().Be(salePrice * 1.2m);
            plate.Letters.Should().Be("ABC");
            plate.Numbers.Should().Be(123);
        }

        [Fact]
        public void Create_ShouldAddPlateCreatedDomainEvent()
        {
            // Arrange
            var registration = "ABC123";
            var purchasePrice = 1000m;
            var salePrice = 1500m;

            // Act
            var plate = Plate.Create(registration, purchasePrice, salePrice);

            // Assert
            plate.DomainEvents.Should().ContainSingle();
            var domainEvent = plate.DomainEvents.First() as PlateCreatedEvent;

            domainEvent.Should().NotBeNull();
            domainEvent.Id.Should().Be(plate.Id);
            domainEvent.Registration.Should().Be(registration);
            domainEvent.PurchasePrice.Should().Be(purchasePrice);
            domainEvent.SalePrice.Should().Be(salePrice);
            domainEvent.DateCreated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Create_WhenInvalid_ShouldNotCreateDomainEvent()
        {
            // Arrange
            var registration = "ABC123";
            var purchasePrice = 1500m;
            var salePrice = 1000m;

            // Act & Assert
            var action = () => Plate.Create(registration, purchasePrice, salePrice);

            action.Should().Throw<InvalidPlateException>()
                .Which.Errors.Should().Contain("Sale price must be greater than purchase price");
        }
    }
}