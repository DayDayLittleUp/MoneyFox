﻿using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

#nullable enable
namespace MoneyFox.Uwp.Converter
{
    public class ClickConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language) => ((ItemClickEventArgs)value)?.ClickedItem;

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
    }
}
