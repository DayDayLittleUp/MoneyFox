﻿namespace MoneyFox.Core.Tests.TestFramework;

using Core.ApplicationCore.Domain.Aggregates.CategoryAggregate;
using Infrastructure.Persistence;

internal static class TestCategoryDbExtensions
{
    public static void RegisterCategories(this AppDbContext db, params TestData.ICategory[] categories)
    {
        foreach (var testCategories in categories)
        {
            db.Add(testCategories.CreateDbCategory());
        }

        db.SaveChanges();
    }

    public static Category RegisterCategory(this AppDbContext db, TestData.ICategory testCategory)
    {
        var dbCategory = testCategory.CreateDbCategory();
        db.Add(dbCategory);
        db.SaveChanges();

        return dbCategory;
    }
}
