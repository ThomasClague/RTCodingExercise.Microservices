using Audit.API.Models;
using Audit.API.Models.Filters;
using Audit.API.Specifications;
using FluentAssertions;

namespace Audit.UnitTests.Specifications
{
    public class GetAuditRecordsSpecificationTests
    {
        private readonly List<AuditRecord> _auditRecords;

        public GetAuditRecordsSpecificationTests()
        {
            _auditRecords = new List<AuditRecord>
            {
                new AuditRecord 
                { 
                    Id = Guid.NewGuid(),
                    EntityType = "Plate",
                    EntityId = Guid.NewGuid(),
                    EventType = "PlateCreated",
                    Timestamp = DateTime.UtcNow.AddDays(-1),
                    InitiatedBy = "User1",
                    Data = "{}",
                    Description = "Test1"
                },
                new AuditRecord 
                { 
                    Id = Guid.NewGuid(),
                    EntityType = "Plate",
                    EntityId = Guid.NewGuid(),
                    EventType = "PlateReserved",
                    Timestamp = DateTime.UtcNow.AddDays(-2),
                    InitiatedBy = "User2",
                    Data = "{}",
                    Description = "Test2"
                },
                new AuditRecord 
                { 
                    Id = Guid.NewGuid(),
                    EntityType = "Order",
                    EntityId = Guid.NewGuid(),
                    EventType = "OrderCreated",
                    Timestamp = DateTime.UtcNow,
                    InitiatedBy = "User1",
                    Data = "{}",
                    Description = "Test3"
                }
            };
        }

        [Theory]
        [InlineData("Plate", true, 0)] 
        [InlineData("Plate", true, 1)] 
        [InlineData("Order", true, 2)] 
        [InlineData("Invalid", false, 0)] 
        public void Should_Filter_By_EntityType(string entityType, bool shouldMatch, int recordIndex)
        {
            // Arrange
            var filter = new AuditRecordFilter { EntityType = entityType };
            var spec = new GetAuditRecordsSpecification(filter);

            // Act
            var result = spec.IsSatisfiedBy(_auditRecords[recordIndex]);

            // Assert
            result.Should().Be(shouldMatch);
        }

        [Theory]
        [InlineData("PlateCreated", true, 0)]
        [InlineData("PlateReserved", true, 1)]
        [InlineData("OrderCreated", true, 2)]
        [InlineData("Invalid", false, 0)]
        public void Should_Filter_By_EventType(string eventType, bool shouldMatch, int recordIndex)
        {
            // Arrange
            var filter = new AuditRecordFilter { EventType = eventType };
            var spec = new GetAuditRecordsSpecification(filter);

            // Act
            var result = spec.IsSatisfiedBy(_auditRecords[recordIndex]);

            // Assert
            result.Should().Be(shouldMatch);
        }

        [Theory]
        [InlineData("User1", true, 0)]
        [InlineData("User2", true, 1)]
        [InlineData("User3", false, 0)]
        public void Should_Filter_By_InitiatedBy(string initiatedBy, bool shouldMatch, int recordIndex)
        {
            // Arrange
            var filter = new AuditRecordFilter { InitiatedBy = initiatedBy };
            var spec = new GetAuditRecordsSpecification(filter);

            // Act
            var result = spec.IsSatisfiedBy(_auditRecords[recordIndex]);

            // Assert
            result.Should().Be(shouldMatch);
        }

        [Fact]
        public void Should_Filter_By_DateRange()
        {
            // Arrange
            var filter = new AuditRecordFilter 
            { 
                FromDate = DateTime.UtcNow.AddDays(-1.5),
                ToDate = DateTime.UtcNow.AddDays(1)
            };
            var spec = new GetAuditRecordsSpecification(filter);

            // Act & Assert
            spec.IsSatisfiedBy(_auditRecords[0]).Should().BeTrue(); 
            spec.IsSatisfiedBy(_auditRecords[1]).Should().BeFalse();
            spec.IsSatisfiedBy(_auditRecords[2]).Should().BeTrue();
        }

        [Theory]
        [InlineData("Plate", "PlateCreated", "User1", true, 0)]
        [InlineData("Plate", "PlateReserved", "User2", true, 1)]
        [InlineData("Order", "OrderCreated", "User1", true, 2)]
        [InlineData("Plate", "PlateCreated", "User2", false, 0)]
        [InlineData("Order", "PlateCreated", "User1", false, 2)]
        public void Should_Filter_By_Multiple_Criteria(
            string entityType, 
            string eventType, 
            string initiatedBy, 
            bool shouldMatch, 
            int recordIndex)
        {
            // Arrange
            var filter = new AuditRecordFilter 
            { 
                EntityType = entityType,
                EventType = eventType,
                InitiatedBy = initiatedBy
            };
            var spec = new GetAuditRecordsSpecification(filter);

            // Act
            var result = spec.IsSatisfiedBy(_auditRecords[recordIndex]);

            // Assert
            result.Should().Be(shouldMatch);
        }
    }
} 