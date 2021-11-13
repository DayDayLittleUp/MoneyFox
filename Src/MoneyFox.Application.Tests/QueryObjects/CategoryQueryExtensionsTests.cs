﻿using MoneyFox.Application.Common.QueryObjects;
using MoneyFox.Domain.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace MoneyFox.Application.Tests.QueryObjects
{
    [ExcludeFromCodeCoverage]
    public class CategoryQueryExtensionsTests
    {
        [Fact]
        public void NameContains()
        {
            // Arrange
            var categoryQueryList = new List<Category>
            {
                new Category("Foo1"),
                new Category("Foo2"),
                new Category("absd")
            };

            // Act
            var resultList = categoryQueryList.WhereNameContains("Foo").ToList();

            // Assert
            Assert.Equal(2, resultList.Count);
            Assert.Equal("Foo1", resultList[0].Name);
            Assert.Equal("Foo2", resultList[1].Name);
        }

        [Fact]
        public void OrderByName()
        {
            // Arrange
            IQueryable<Category> categoryQueryList = new List<Category>
            {
                new Category("Foo2"),
                new Category("Foo3"),
                new Category("Foo1")
            }.AsQueryable();

            // Act
            var resultList = categoryQueryList.OrderByName().ToList();

            // Assert
            Assert.Equal(3, resultList.Count);
            Assert.Equal("Foo1", resultList[0].Name);
            Assert.Equal("Foo2", resultList[1].Name);
            Assert.Equal("Foo3", resultList[2].Name);
        }
    }
}
