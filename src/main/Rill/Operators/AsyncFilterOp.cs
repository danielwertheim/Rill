﻿using System;
using System.Threading.Tasks;

namespace Rill.Operators
{
    internal sealed class AsyncFilterOp<T> : IAsyncRillConsumable<T>
    {
        private readonly IAsyncRillConsumable<T> _src;
        private readonly Func<Event<T>, bool> _predicate;

        public AsyncFilterOp(IAsyncRillConsumable<T> src, Func<Event<T>, bool> predicate)
        {
            _src = src;
            _predicate = predicate;
        }

        public void Dispose() => _src.Dispose();

        public IDisposable Subscribe(IAsyncRillConsumer<T> consumer)
            => _src.Subscribe(new AsyncFilteringConsumer(consumer, _predicate));

        private sealed class AsyncFilteringConsumer : IAsyncRillConsumer<T>
        {
            private readonly IAsyncRillConsumer<T> _consumer;
            private readonly Func<Event<T>, bool> _predicate;

            public AsyncFilteringConsumer(IAsyncRillConsumer<T> consumer, Func<Event<T>, bool> predicate)
            {
                _consumer = consumer;
                _predicate = predicate;
            }

            public async ValueTask OnNewAsync(Event<T> ev)
            {
                if (_predicate(ev))
                    await _consumer.OnNewAsync(ev).ConfigureAwait(false);
            }

            public ValueTask OnAllSucceededAsync(EventId eventId)
                => _consumer.OnAllSucceededAsync(eventId);

            public ValueTask OnAnyFailedAsync(EventId eventId)
                => _consumer.OnAnyFailedAsync(eventId);

            public ValueTask OnCompletedAsync()
                => _consumer.OnCompletedAsync();
        }
    }
}
