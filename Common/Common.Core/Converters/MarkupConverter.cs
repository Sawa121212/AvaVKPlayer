using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Common.Core.Converters
{
    /// <summary>
    /// This is easier to read and will create a new instance of the converter every time
    /// </summary>
    /// <remarks>https://michaelscodingspot.com/4-tips-increase-productivity-wpf-converters/ </remarks>
    public abstract class MarkupConverter : MarkupExtension, IValueConverter
    {
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <inheritdoc />
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}