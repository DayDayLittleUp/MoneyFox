﻿namespace MoneyFox.Ui.Converter;

using System.Globalization;
using Core.Common.Helpers;

/// <summary>
///     Formats the date with the culture in the CultureHelper.
/// </summary>
public class DateFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((DateTime)value).ToString(format: "d", provider: CultureHelper.CurrentCulture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
