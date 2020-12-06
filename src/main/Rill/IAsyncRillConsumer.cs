﻿using System;
using System.Threading.Tasks;

namespace Rill
{
    /// <summary>
    /// Defines a consumer which can consume events from a <see cref="IRillConsumable{T}"/> implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncRillConsumer<T>
    {
        /// <summary>
        /// Invoked when the Rill gets a new event.
        /// </summary>
        /// <param name="ev"></param>
        ValueTask OnNewAsync(Event<T> ev);

        /// <summary>
        /// Invoked when the event has been dispatched and handled
        /// by all consumers without causing any errors.
        /// </summary>
        /// <param name="eventId"></param>
        ValueTask OnAllSucceededAsync(EventId eventId);

        /// <summary>
        /// Invoked when the event caused a failure in any consumer.
        /// </summary>
        /// <param name="eventId"></param>
        ValueTask OnAnyFailedAsync(EventId eventId);

        /// <summary>
        /// Invoked when the Rill signals it has completed and no more
        /// events will be emitted.
        /// </summary>
        ValueTask OnCompletedAsync();
    }
}
