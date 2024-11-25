using Catalog.Domain;
using FluentAssertions;
using System;
using System.Reflection;
using Xunit;

namespace Catalog.UnitTests.Domain
{
    public class TryParseRegistrationTests
    {
        private readonly MethodInfo _tryParseRegistrationMethod;

        public TryParseRegistrationTests()
        {
            _tryParseRegistrationMethod = typeof(Plate).GetMethod(
                "TryParseRegistration",
                BindingFlags.NonPublic | BindingFlags.Static);
        }

        [Theory]
        [InlineData("ABC123", "ABC", 123)]
        [InlineData("XYZ789", "XYZ", 789)]
        [InlineData("AB12", "AB", 12)]
        [InlineData("ABCD1234", "ABCD", 1234)]
        [InlineData("A1", "A", 1)]
        [InlineData("ABC 123", "ABC", 123)]
        [InlineData("abc123", "ABC", 123)]
        public void TryParseRegistration_WithValidMixedFormat_ShouldReturnTrue(
            string registration,
            string expectedLetters,
            int expectedNumbers)
        {
            // Arrange
            var parameters = new object[] { registration, null, 0 };

            // Act
            var result = (bool)_tryParseRegistrationMethod.Invoke(null, parameters);
            var letters = (string)parameters[1];
            var numbers = (int)parameters[2];

            // Assert
            result.Should().BeTrue();
            letters.Should().Be(expectedLetters);
            numbers.Should().Be(expectedNumbers);
        }

        [Theory]
        [InlineData("AB*123")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("ABC12.3")]
        [InlineData("AB.C123")]
        public void TryParseRegistration_WithInvalidFormat_ShouldReturnFalse(string registration)
        {
            // Arrange
            var parameters = new object[] { registration, null, 0 };

            // Act
            var result = (bool)_tryParseRegistrationMethod.Invoke(null, parameters);
            var letters = (string)parameters[1];
            var numbers = (int)parameters[2];

            // Assert
            result.Should().BeFalse();
            letters.Should().BeNull();
            numbers.Should().Be(0);
        }

        [Fact]
        public void Create_WithValidRegistration_ShouldCreatePlate()
        {
            // Arrange
            var registration = "ABC123";
            var purchasePrice = 1000m;
            var salePrice = 1500m;

            // Act
            var plate = Plate.Create(registration, purchasePrice, salePrice);

            // Assert
            plate.Letters.Should().Be("ABC");
            plate.Numbers.Should().Be(123);
            plate.Registration.Should().Be("ABC123");
        }
    }
} 