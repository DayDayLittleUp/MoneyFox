namespace MoneyFox.Core.Tests.ApplicationCore.Queries.Payments.GetPaymentsForCategory;

using System.Diagnostics.CodeAnalysis;
using Core.ApplicationCore.Queries;
using FluentAssertions;

[ExcludeFromCodeCoverage]
public class GetPaymentsForCategoryQueryTests
{
    [Fact]
    public void AssignCorrectlyInCtor()
    {
        // Arrange
        const int catId = 5234;
        var dateRangeFrom = DateTime.Now.AddDays(1);
        var dateRangeTo = DateTime.Now.AddDays(2);

        // Act
        var query = new GetPaymentsForCategorySummary.Query(categoryId: catId, dateRangeFrom: dateRangeFrom, dateRangeTo: dateRangeTo);

        // Assert
        query.CategoryId.Should().Be(catId);
        query.DateRangeFrom.Should().Be(dateRangeFrom);
        query.DateRangeTo.Should().Be(dateRangeTo);
    }
}
