using System;
using System.Globalization;
using Avalonia.Data.Converters;
using VkPlayer.Domain;

namespace VkPlayer.Module.Converters
{
    public class RepostTypeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is RepostToType)
            {
                RepostToType val = (RepostToType)value;
                return val switch
                {
                    RepostToType.Friend => "Друзья",
                    RepostToType.Dialog => "Диалоги",
                    _ => val.ToString(),
                };
            }
            else return "Неизвестно";

        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
