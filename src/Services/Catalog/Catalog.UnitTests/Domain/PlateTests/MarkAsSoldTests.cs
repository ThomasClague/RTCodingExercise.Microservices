using Catalog.Domain;
using Catalog.Domain.Exceptions;
using Contracts.Plates.Events;
using FluentAssertions;
using MassTransit.Registration;
using System;
using System.Linq;
using Xunit;
using static System.Collections.Specialized.BitVector32;

namespace Catalog.UnitTests.Domain.PlateTests.PlateTests
{
    public class MarkAsSoldTests
    {
        [Fact]
        public void MarkAsSold_WhenPlateIsAvailable_ShouldSucceed()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 200m);

            // Act
            plate.MarkAsSold();

            // Assert
            plate.StatusId.Should().Be((int)PlateStatusEnum.Sold);
            plate.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "PlateSoldEvent");
        }

        [Fact]
        public void MarkAsSold_WhenPlateIsReserved_ShouldNotSucceed()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 200m);
            plate.Reserve();

            // Act
            var action = () => plate.MarkAsSold();

            // Assert
            action.Should().Throw<PlateAlreadyReservedException>()
                .WithMessage($"Plate {plate.Registration} is already reserved");
        }

        [Fact]
        public void MarkAsSold_WhenPlateIsAlreadySold_ShouldThrowException()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 200m);
            plate.MarkAsSold();

            // Act
            var action = () => plate.MarkAsSold();

            // Assert
            action.Should().Throw<PlateAlreadySoldException>()
                .WithMessage($"Plate {plate.Registration} has already been sold");
        }

        [Fact]
        public void MarkAsSold_WithValidDiscountCode_ShouldSucceed()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 300m);
            var markedUpPrice = 300m * 1.2m;
            var expectedPrice = markedUpPrice - 25m; 

            // Act
            plate.MarkAsSold("DISCOUNT");

            // Assert
            plate.StatusId.Should().Be((int)PlateStatusEnum.Sold);
            var soldEvent = plate.DomainEvents.OfType<PlateSoldEvent>().Single();
            soldEvent.SalePrice.Should().Be(expectedPrice);
        }

        [Fact]
        public void MarkAsSold_WithInvalidDiscountCode_ShouldThrowException()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 200m);

            // Act
            var action = () => plate.MarkAsSold("INVALID");

            // Assert
            action.Should().Throw<InvalidDiscountCodeException>()
                .WithMessage("The discount code 'INVALID' is not valid");
        }

        [Fact]
        public void MarkAsSold_ShouldRaisePlateSoldEventWithCorrectData()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 200m);

            // Act
            plate.MarkAsSold();

            // Assert
            var @event = plate.DomainEvents.OfType<PlateSoldEvent>().Single();
            @event.Id.Should().Be(plate.Id);
            @event.Registration.Should().Be(plate.Registration);
            @event.SalePrice.Should().Be(plate.DisplayPrice);
            @event.DateCreated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void MarkAsSold_WithDiscount_ShouldRaisePlateSoldEventWithDiscountedPrice()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 300);
            var markedUpPrice = 300m * 1.2m;
            var expectedDiscountedPrice = markedUpPrice -25;

            // Act
            plate.MarkAsSold("DISCOUNT");

            // Assert
            var @event = plate.DomainEvents.OfType<PlateSoldEvent>().Single();
            @event.SalePrice.Should().Be(expectedDiscountedPrice);
        }

        [Theory]
        [InlineData(100, 300, "PERCENTOFF", false)]
        [InlineData(100, 200, "PERCENTOFF", false)]
        [InlineData(100, 300, "DISCOUNT", true)]
        [InlineData(40, 50, "DISCOUNT", false)]
        public void MarkAsSold_WithDiscount_ShouldValidateMinimumPrice(
            decimal purchasePrice, 
            decimal salePrice, 
            string discountCode, 
            bool shouldSucceed)
        {
            // Arrange
            var plate = Plate.Create("ABC123", purchasePrice, salePrice);

            // Act
            var action = () => plate.MarkAsSold(discountCode);

            // Assert
            if (shouldSucceed)
            {
                action.Should().NotThrow<DiscountExceedsMinimumPriceException>();
            }
            else
            {
                action.Should().Throw<DiscountExceedsMinimumPriceException>();
            }
        }
    }
} 