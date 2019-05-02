// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;

namespace Softeq.NetKit.Chat.SignalRClient.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static void Apply<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
    }
}