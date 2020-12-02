﻿using System;

namespace Rill.Operators
{
    internal sealed class FilterContentOp<T> : IRillConsumable<T>
    {
        private readonly IRillConsumable<T> _src;
        private readonly Func<T, bool> _predicate;

        public FilterContentOp(IRillConsumable<T> src, Func<T, bool> predicate)
        {
            _src = src;
            _predicate = predicate;
        }

        public void Dispose()
            => _src.Dispose();

        public IDisposable Subscribe(IRillConsumer<T> consumer)
            => _src.Subscribe(new FilteringConsumer(consumer, _predicate));

        private sealed class FilteringConsumer : IRillConsumer<T>
        {
            private readonly IRillConsumer<T> _consumer;
            private readonly Func<T, bool> _predicate;

            public FilteringConsumer(IRillConsumer<T> consumer, Func<T, bool> predicate)
            {
                _consumer = consumer;
                _predicate = predicate;
            }

            public void OnNew(Event<T> ev)
            {
                if (_predicate(ev.Content))
                    _consumer.OnNew(ev);
            }

            public void OnAllSucceeded(EventId eventId)
                => _consumer.OnAllSucceeded(eventId);

            public void OnAnyFailed(EventId eventId)
                => _consumer.OnAnyFailed(eventId);

            public void OnCompleted()
                => _consumer.OnCompleted();
        }
    }
}
