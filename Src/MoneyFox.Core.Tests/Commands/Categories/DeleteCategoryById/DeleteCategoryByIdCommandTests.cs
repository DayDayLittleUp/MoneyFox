﻿namespace MoneyFox.Core.Tests.Commands.Categories.DeleteCategoryById
{
    using Common.Interfaces;
    using Core.Aggregates.Payments;
    using Core.Commands.Categories.DeleteCategoryById;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using MoneyFox.Infrastructure.Persistence;
    using Moq;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class DeleteCategoryByIdCommandTests : IDisposable
    {
        private readonly AppDbContext context;
        private readonly Mock<IContextAdapter> contextAdapterMock;

        public DeleteCategoryByIdCommandTests()
        {
            context = InMemoryAppDbContextFactory.Create();

            contextAdapterMock = new Mock<IContextAdapter>();
            contextAdapterMock.SetupGet(x => x.Context).Returns(context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => InMemoryAppDbContextFactory.Destroy(context);

        [Fact]
        public async Task GetExcludedAccountQuery_WithoutFilter_CorrectNumberLoaded()
        {
            // Arrange
            var category1 = new Category("test");
            await context.AddAsync(category1);
            await context.SaveChangesAsync();

            // Act
            await new DeleteCategoryByIdCommand.Handler(
                    contextAdapterMock.Object)
                .Handle(new DeleteCategoryByIdCommand(category1.Id), default);

            // Assert
            (await context.Categories.FirstOrDefaultAsync(x => x.Id == category1.Id)).Should().BeNull();
        }
    }
}