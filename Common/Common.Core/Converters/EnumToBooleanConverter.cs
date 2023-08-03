using System;
using System.Globalization;
using Avalonia.Data;

namespace Common.Core.Converters
{
    /// <summary>
    /// Сравнить значение из Enum параметра
    /// </summary>
    public class EnumToBooleanConverter : MarkupConverter
    {
        /// <inheritdoc />
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                switch (parameter)
                {
                    case string parameterString:
                    {
                        if (Enum.IsDefined(enumValue.GetType(), value))
                        {
                            try
                            {
                                object? parameterValue = Enum.Parse(enumValue.GetType(), parameterString);
                                return parameterValue.Equals(value);
                            }
                            catch
                            {
                                //ToDo: Нужно ли что то делать?
                            }
                        }

                        break;
                    }
                    case Enum parameterEnum:
                        return value?.Equals(parameterEnum);
                }
            }

            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter)
            {
                case string parameterString:
                    return Enum.Parse(targetType, parameterString);
                case Enum parameterEnum:
                    return value?.Equals(true) == true ? parameterEnum : BindingOperations.DoNothing;
            }

            throw new NotSupportedException();
        }
    }
}