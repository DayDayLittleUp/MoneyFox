﻿using Microsoft.UI.Xaml.Data;
using System;

namespace MoneyFox.Win.Converter
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => value;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            decimal.TryParse((string)value, out decimal result);

            return result;
        }
    }
}