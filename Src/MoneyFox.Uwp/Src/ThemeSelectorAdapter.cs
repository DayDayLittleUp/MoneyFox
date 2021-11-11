﻿using MoneyFox.Application.Common;
using MoneyFox.Application.Common.Interfaces;
using MoneyFox.Uwp.Services;
using System;
using Windows.UI.Xaml;

#nullable enable
namespace MoneyFox.Uwp
{
    public class ThemeSelectorAdapter : IThemeSelectorAdapter
    {
        public string Theme => ThemeSelectorService.Theme.ToString();

        public void SetTheme(string theme)
        {
            if(Enum.TryParse(theme, out ElementTheme cacheTheme))
            {
                ThemeSelectorService.SetThemeAsync(cacheTheme).FireAndForgetSafeAsync();
            }
        }
    }
}
