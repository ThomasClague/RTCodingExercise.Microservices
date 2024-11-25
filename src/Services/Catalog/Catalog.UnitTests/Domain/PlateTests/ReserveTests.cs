using Catalog.Domain;
using Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Domain.PlateTests.PlateTests
{
    public class ReserveTests
    {
        [Fact]
        public void Reserve_WhenPlateIsAvailable_ShouldSucceed()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 200m);

            // Act
            plate.Reserve();

            // Assert
            plate.StatusId.Should().Be((int)PlateStatusEnum.Reserved);
            plate.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "PlateReservedEvent");
        }

        [Fact]
        public void Reserve_WhenPlateIsAlreadyReserved_ShouldThrowException()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 200m);
            plate.Reserve();

            // Act
            var action = () => plate.Reserve();

            // Assert
            action.Should().Throw<PlateAlreadyReservedException>()
                .WithMessage($"Plate {plate.Registration} is already reserved");
        }

        [Fact]
        public void Reserve_WhenPlateIsSold_ShouldThrowException()
        {
            // Arrange
            var plate = Plate.Create("ABC123", 100m, 200m);
            plate.MarkAsSold();

            // Act
            var action = () => plate.Reserve();

            // Assert
            action.Should().Throw<PlateAlreadySoldException>()
                .WithMessage($"Plate {plate.Registration} has already been sold");
        }
    }
} 