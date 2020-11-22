﻿using System;
using System.Threading.Tasks;

namespace Rill.Operators
{
    internal sealed class AsyncCatchOp<T, TException> : IAsyncRillConsumable<T>
        where T : class
        where TException : Exception
    {
        private readonly IAsyncRillConsumable<T> _src;
        private readonly Action<TException> _handler;

        public AsyncCatchOp(IAsyncRillConsumable<T> src, Action<TException> handler)
        {
            _src = src;
            _handler = handler;
        }

        public void Dispose()
            => _src.Dispose();

        public IDisposable Subscribe(IAsyncRillConsumer<T> consumer)
            => _src.Subscribe(new AsyncCatchingConsumer(consumer, _handler));

        private sealed class AsyncCatchingConsumer : IAsyncRillConsumer<T>
        {
            private readonly IAsyncRillConsumer<T> _consumer;
            private readonly Action<TException> _handler;

            public AsyncCatchingConsumer(IAsyncRillConsumer<T> consumer, Action<TException> handler)
            {
                _consumer = consumer;
                _handler = handler;
            }

            public async ValueTask OnNewAsync(Event<T> ev)
            {
                try
                {
                    await _consumer.OnNewAsync(ev).ConfigureAwait(false);
                }
                catch (TException ex)
                {
                    _handler(ex);
                }
            }

            public ValueTask OnAllSucceededAsync(EventId eventId)
                => _consumer.OnAllSucceededAsync(eventId);

            public ValueTask OnAnyFailedAsync(EventId eventId, Exception error)
                => _consumer.OnAnyFailedAsync(eventId, error);

            public ValueTask OnCompletedAsync()
                => _consumer.OnCompletedAsync();
        }
    }
}
