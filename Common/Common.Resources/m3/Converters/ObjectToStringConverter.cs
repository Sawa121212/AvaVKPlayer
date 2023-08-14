using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Common.Resources.m3.Converters {
    internal class ObjectToStringConverter : IValueConverter {
       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value.ToString() ?? string.Empty;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}