﻿namespace MoneyFox.Tests.Core.ApplicationCore.UseCases
{

    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MoneyFox.Core.ApplicationCore.Domain.Aggregates.CategoryAggregate;
    using MoneyFox.Core.ApplicationCore.UseCases.CategoryCreation;
    using NSubstitute;
    using TestFramework.Category;
    using Xunit;
    using static TestFramework.Category.CategoryAssertion;

    public class CreateCategoryHandlerTests
    {
        private readonly ICategoryRepository repository;

        public CreateCategoryHandlerTests()
        {
            repository = Substitute.For<ICategoryRepository>();
        }

        [Fact]
        public async Task CategoryAdded_WhenValidValuesPassed()
        {
            // Capture
            Category? passedCategory = null;
            await repository.AddAsync(category: Arg.Do<Category>(c => passedCategory = c), cancellationToken: Arg.Any<CancellationToken>());

            // Arrange
            var testCategory = new TestData.DefaultCategory();

            // Act
            var command = new CreateCategory.Command(name: testCategory.Name, note: testCategory.Note, requireNote: testCategory.RequireNote);
            await new CreateCategory.Handler(repository).Handle(request: command, cancellationToken: CancellationToken.None);

            // Assert
            await repository.Received().AddAsync(category: Arg.Any<Category>(), cancellationToken: Arg.Any<CancellationToken>());
            passedCategory.Should().NotBeNull();
            AssertCategory(actual: passedCategory!, expected: testCategory);
        }
    }

}
