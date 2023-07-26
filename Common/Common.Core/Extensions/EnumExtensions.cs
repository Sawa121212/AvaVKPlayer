using System;

namespace Common.Core.Extensions { 

public static class EnumExtensions
{
    /// <summary>
    /// Преобразует текст к перечислению.
    /// </summary>
    /// <param name="enumString">Текст для преобразования.</param>
    /// <param name="isThrowException">Признак генерации исключения при неудачном преобразовании.</param>
    public static T ToEnum<T>(this string enumString, bool isThrowException = false)
        where T : struct, IComparable, IFormattable, IConvertible
    {
        return Enum.TryParse(enumString, true, out T value)
            ? value
            : isThrowException
                ? throw new ArgumentException(nameof(enumString))
                : default(T);
    }
} }