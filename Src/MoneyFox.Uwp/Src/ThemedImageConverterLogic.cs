﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

#nullable enable
namespace MoneyFox.Uwp
{
    public static class ThemedImageConverterLogic
    {
        private static readonly Dictionary<string, BitmapImage> ImageCache = new Dictionary<string, BitmapImage>();

        public static BitmapImage? GetImage(string path, bool negateResult = false)
        {
            if(string.IsNullOrEmpty(path))
            {
                return null;
            }

            bool isDarkTheme = Windows.UI.Xaml.Application.Current.RequestedTheme == ApplicationTheme.Dark;

            if(negateResult)
            {
                isDarkTheme = !isDarkTheme;
            }

            path = $"ms-appx:{(string.Format(CultureInfo.InvariantCulture, path, isDarkTheme ? "dark" : "light"))}";

            // Check if we already cached the image
            if(!ImageCache.TryGetValue(path, out BitmapImage result))
            {
                result = new BitmapImage(new Uri(path, UriKind.Absolute));
                ImageCache.Add(path, result);
            }

            return result;
        }
    }
}
