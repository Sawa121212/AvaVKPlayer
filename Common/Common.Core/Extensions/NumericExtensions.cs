using System;

namespace Common.Core.Extensions
{
    /// <summary>
    /// Методы расширения числовых значений.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Сравнение двух вещественных чисел типа <see cref="double"/> с указанной точностью.
        /// </summary>
        /// <param name="first">Первое сравниваемое значение.</param>
        /// <param name="second">Второе сравниваемое значение.</param>
        /// <param name="eps">Точность.</param>
        /// <returns>Результат операции сравнения.</returns>
        public static bool IsEquals(this double first, double second, double eps = 0.000000001)
        {
            return Math.Abs(first - second) < eps;
        }
    }
}