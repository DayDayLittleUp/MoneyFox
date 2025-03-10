﻿namespace MoneyFox.Core.Tests.TestFramework;

using Core.ApplicationCore.Domain.Aggregates.BudgetAggregate;

internal static class TestBudgetDbFactory
{
    internal static Budget CreateDbBudget(this TestData.IBudget budget)
    {
        var spendingLimit = new SpendingLimit(budget.SpendingLimit);

        return new(name: budget.Name, spendingLimit: spendingLimit, timeRange: budget.BudgetTimeRange, includedCategories: budget.Categories);
    }
}
