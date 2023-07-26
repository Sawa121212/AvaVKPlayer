using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Media;

namespace Common.Core.Extensions
{
    /// <summary>
    /// Методы-расширения для <see cref="string" />.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class StringExtensions
    {
        /// <summary>
        /// Проверка строки на null.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результат проверки.</returns>
        public static bool IsNull(this string text)
        {
            return text is null;
        }

        /// <summary>
        /// Проверка строки на null и пустоту.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результат проверки.</returns>
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// Проверка строки на пустоту.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результат проверки.</returns>
        public static bool IsEmpty(this string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return text.Length == 0;
        }

        /// <summary>
        /// Признак того, что строка содержит только пробельные (white space) символы.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результат проверки.</returns>
        public static bool IsWhiteSpace(this string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return text.All(char.IsWhiteSpace);
        }

        /// <summary>
        /// Признак того, что строка есть null, или пустая, или содержит только пробельные (white space) символы.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результирующая строка.</returns>
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// Замена пустой строки на null.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результирующая строка.</returns>
        public static string EmptyToNull(this string text)
        {
            return text != null && text.IsEmpty() ? null : text;
        }

        /// <summary>
        /// Замена null строки на пустую.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        /// <returns>Результирующая строка.</returns>
        public static string NullToEmpty(this string text)
        {
            return text ?? string.Empty;
        }

        /// <summary>
        /// Удаление пробелов из строки.
        /// </summary>
        /// <param name="text">Исходная строка.</param>
        public static string RemoveWhiteSpace(this string text)
        {
            var sb = new StringBuilder();
            if (text == null)
                return sb.ToString();

            foreach (var c in text.Where(c => !char.IsWhiteSpace(c)))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }
        
        /// <summary>
        /// Получить размеры строки.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontFamily"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        /// Могут быть проблемы
        public static Size GetSize(this string text, string fontFamily, float fontSize)
        {
            var formatted = new FormattedText(text, new Typeface(fontFamily), fontSize, TextAlignment.Left, TextWrapping.Wrap, Size.Infinity);

            return new Size(formatted.Bounds.Width, formatted.Bounds.Height);
        }        

        /// <summary>
        /// Поиск совпадений в строках с учетом StringComparison.
        /// </summary>
        /// <param name="source">Исходная строка.</param>
        /// <param name="toCheck">Сравниваемая строка.</param>
        /// <param name="comp">Способ (компаратор) сравнения.</param>
        public static bool IsContains(this string source, string toCheck,
            StringComparison comp = StringComparison.InvariantCultureIgnoreCase)
        {
            if (source.IsNullOrEmpty())
                return toCheck.IsNullOrEmpty();

            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Проверка строк с учетом StringComparison.
        /// </summary>
        /// <param name="source">Исходная строка.</param>
        /// <param name="other">Сравниваемая строка.</param>
        /// <param name="comp">Способ (компаратор) сравнения.</param>
        public static bool IsEquals(this string source, string other,
            StringComparison comp = StringComparison.InvariantCultureIgnoreCase)
        {
            if (ReferenceEquals(source, null))
                return ReferenceEquals(other, null);

            if (ReferenceEquals(other, null))
                return false;

            return source.Equals(other, comp);
        }
    }
}