using Microsoft.UI.Xaml.Data;
using MoneyFox.Core._Pending_;
using System;

namespace MoneyFox.Win.Converter
{
    public class AmountFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal currencyValue = (decimal)value;
            return currencyValue.ToString("C", CultureHelper.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotSupportedException();
    }
}