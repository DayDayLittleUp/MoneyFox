﻿namespace MoneyFox.Win.Converter;

using System;
using Microsoft.UI.Xaml.Data;

public class DateVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (DateTime)value != new DateTime();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}
