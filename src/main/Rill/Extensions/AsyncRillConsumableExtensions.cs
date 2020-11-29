﻿using System;
using System.Threading.Tasks;
using Rill.Operators;

namespace Rill.Extensions
{
    public static class AsyncRillConsumableExtensions
    {
        public static IDisposable Subscribe<T>(
            this IAsyncRillConsumable<T> consumable,
            AsyncNewEventHandler<T> onNew,
            AsyncSuccessfulEventHandler? onAllSucceeded = null,
            AsyncFailedEventHandler? onAnyFailed = null,
            Func<ValueTask>? onCompleted = null)
            => consumable.Subscribe(ConsumerFactory.AsynchronousConsumer(onNew, onAllSucceeded, onAnyFailed, onCompleted));

        public static IAsyncRillConsumable<T> Catch<T, TException>(this IAsyncRillConsumable<T> consumable, Action<TException> handler)
            where TException : Exception
            where T : class
            => new AsyncCatchOp<T, TException>(consumable, handler);

        public static IAsyncRillConsumable<T> CatchAny<T>(this IAsyncRillConsumable<T> consumable, Action<Exception> handler)
            where T : class
            => consumable.Catch(handler);

        public static IAsyncRillConsumable<T> OfEventType<T>(this IAsyncRillConsumable<object> consumable)
            => new AsyncOfTypeOp<object, T>(consumable);

        public static IAsyncRillConsumable<TResult> OfEventType<TSrc, TResult>(this IAsyncRillConsumable<TSrc> consumable)
            => new AsyncOfTypeOp<TSrc, TResult>(consumable);

        public static IAsyncRillConsumable<TResult> Select<TSource, TResult>(this IAsyncRillConsumable<TSource> consumable, Func<TSource, TResult> map)
            where TSource : class where TResult : class
            => new AsyncMapOp<TSource, TResult>(consumable, map);

        public static IAsyncRillConsumable<T> Where<T>(this IAsyncRillConsumable<T> consumable, Func<Event<T>, bool> predicate) where T : class
            => new AsyncFilterOp<T>(consumable, predicate);

        public static IAsyncRillConsumable<T> Where<T>(this IAsyncRillConsumable<T> consumable, Func<T, bool> predicate) where T : class
            => new AsyncFilterContentOp<T>(consumable, predicate);
    }
}
