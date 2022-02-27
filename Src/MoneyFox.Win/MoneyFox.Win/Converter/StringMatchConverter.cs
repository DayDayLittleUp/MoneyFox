﻿namespace MoneyFox.Win.Converter;

using Microsoft.UI.Xaml.Data;
using System;

public class StringMatchConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if(parameter is string enumString)
        {
            return enumString.Equals(value);
        }

        throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}