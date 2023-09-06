using System;
using Prism.Ioc;

namespace Common.Core.Extensions
{
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

        public static void TryRegister<T>(this IContainerRegistry containerRegistry)
        {
            if (!containerRegistry.IsRegistered<T>())
            {
                containerRegistry.Register(typeof(T));
            }
        }

        public static void TryRegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            if (!containerRegistry.IsRegistered<TFrom>())
            {
                containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo));
            }
        }

        public static void TryRegisterForNavigation<Type, Name>(this IContainerRegistry containerRegistry)
        {
            if (!containerRegistry.IsRegistered<Type>())
            {
                containerRegistry.Register(typeof(object), typeof(Type), nameof(Name));
            }
        }
    }
}