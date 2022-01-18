﻿using FluentAssertions;
using MoneyFox.Application.Tests.Infrastructure;
using MoneyFox.Core;
using MoneyFox.Core._Pending_.Common.Interfaces;
using MoneyFox.Core.Aggregates;
using MoneyFox.Core.Aggregates.Payments;
using MoneyFox.Core.Queries.Payments.GetPaymentsForAccountId;
using MoneyFox.Infrastructure.Persistence;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoneyFox.Application.Tests.Payments.Query.GetPaymentsForAccountId
{
    [ExcludeFromCodeCoverage]
    public class GetPaymentsForAccountIdQueryTests : IDisposable
    {
        private readonly EfCoreContext context;
        private readonly Mock<IContextAdapter> contextAdapterMock;

        public GetPaymentsForAccountIdQueryTests()
        {
            context = InMemoryEfCoreContextFactory.Create();

            contextAdapterMock = new Mock<IContextAdapter>();
            contextAdapterMock.SetupGet(x => x.Context).Returns(context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => InMemoryEfCoreContextFactory.Destroy(context);

        [Fact]
        public async Task GetPaymentsForAccountId_CorrectAccountId_PaymentFound()
        {
            // Arrange
            var account = new Account("test", 80);

            var payment1 = new Payment(DateTime.Now, 20, PaymentType.Expense, account);
            var payment2 = new Payment(DateTime.Now, 20, PaymentType.Expense, new Account("test", 80));
            await context.AddAsync(payment1);
            await context.AddAsync(payment2);
            await context.SaveChangesAsync();

            // Act
            List<Payment> result = await new GetPaymentsForAccountIdQuery.Handler(contextAdapterMock.Object).Handle(
                new GetPaymentsForAccountIdQuery(account.Id, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)),
                default);

            // Assert
            result.First().Id.Should().Be(payment1.Id);
        }

        [Fact]
        public async Task GetPaymentsForAccountId_CorrectDateRange_PaymentFound()
        {
            // Arrange
            var account = new Account("test", 80);

            var payment1 = new Payment(DateTime.Now.AddDays(-2), 20, PaymentType.Expense, account);
            var payment2 = new Payment(DateTime.Now, 20, PaymentType.Expense, account);
            await context.AddAsync(payment1);
            await context.AddAsync(payment2);
            await context.SaveChangesAsync();

            // Act
            List<Payment> result = await new GetPaymentsForAccountIdQuery.Handler(contextAdapterMock.Object).Handle(
                new GetPaymentsForAccountIdQuery(account.Id, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)),
                default);

            // Assert
            result.First().Id.Should().Be(payment2.Id);
        }
    }
}