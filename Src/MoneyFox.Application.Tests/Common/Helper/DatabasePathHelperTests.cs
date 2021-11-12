﻿using FluentAssertions;
using MoneyFox.Application.Common;
using MoneyFox.Application.Common.Helpers;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace MoneyFox.Application.Tests.Common.Helper
{
    [ExcludeFromCodeCoverage]
    public class DatabasePathHelperTests
    {
        [Theory]
        [InlineData(AppPlatform.Android, @"moneyfox3.db")]
        [InlineData(AppPlatform.iOS, @"Library")]
        [InlineData(AppPlatform.UWP, "moneyfox3.db")]
        public void GetDbPath_Platform_CorrectPath(AppPlatform platform, string expectedPathSegment)
        {
            // Arrange
            ExecutingPlatform.Current = platform;

            // Act
            string result = DatabasePathHelper.DbPath;

            // Assert
            result.Should().Contain(expectedPathSegment);
        }
    }
}
