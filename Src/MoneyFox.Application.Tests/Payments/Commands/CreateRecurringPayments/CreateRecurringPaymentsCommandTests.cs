﻿using FluentAssertions;
using MoneyFox.Application.Common.Interfaces;
using MoneyFox.Application.Payments.Commands.CreateRecurringPayments;
using MoneyFox.Application.Tests.Infrastructure;
using MoneyFox.Core;
using MoneyFox.Core.Aggregates;
using MoneyFox.Core.Aggregates.Payments;
using MoneyFox.Infrastructure.Persistence;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoneyFox.Application.Tests.Payments.Commands.CreateRecurringPayments
{
    [ExcludeFromCodeCoverage]
    public class CreateRecurringPaymentsCommandTests : IDisposable
    {
        private readonly EfCoreContext context;
        private readonly Mock<IContextAdapter> contextAdapterMock;

        public CreateRecurringPaymentsCommandTests()
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
        public async Task PaymentsClearedAndSaved()
        {
            // Arrange
            var payment = new Payment(DateTime.Now.AddDays(-1), 166, PaymentType.Expense, new Account("Foo"));
            payment.AddRecurringPayment(PaymentRecurrence.Daily);

            context.AddRange(payment);
            await context.SaveChangesAsync();

            // Act
            await new CreateRecurringPaymentsCommand.Handler(contextAdapterMock.Object).Handle(
                new CreateRecurringPaymentsCommand(),
                default);
            List<Payment> loadedPayments = context.Payments.ToList();

            // Assert
            loadedPayments.Should().HaveCount(2);
            loadedPayments.ForEach(x => x.Amount.Should().Be(166));
        }
    }
}