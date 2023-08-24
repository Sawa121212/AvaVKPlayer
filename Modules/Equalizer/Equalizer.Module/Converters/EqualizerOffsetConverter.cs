using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Equalizer.Module.Converters
{
    public class EqualizerOffsetConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                double.NaN => value,
                double val => -(int) (val / 2),
                int i => -(int) (i / 2),
                _ => value
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}