using System;
using Treefort.Commanding;
using Treefort.Common;

namespace Treefort.Messaging
{
    /// <summary>
    /// Static factory class for <see cref="Envelope{T}"/>.
    /// </summary>
    public abstract class Envelope
    {
        /// <summary>
        /// Creates an envelope for the given body.
        /// </summary>
        public static Envelope<T> Create<T>(T body)
        {
            var envelope = new Envelope<T>(body);

            body.CastAction<ICommand>(command => envelope
                .Tap(x => x.CorrelationId = command.CorrelationId.ToString())
                .Tap(x => x.MessageId = command.AggregateId.ToString()));

            return envelope;
        }
    }

    /// <summary>
    /// Provides the envelope for an object that will be sent to a bus.
    /// </summary>
    public class Envelope<T> : Envelope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope{T}"/> class.
        /// </summary>
        public Envelope(T body)
        {
            this.Body = body;
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public T Body { get; private set; }

        /// <summary>
        /// Gets or sets the delay for sending, enqueing or processing the body.
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Gets or sets the time to live for the message in the queue.
        /// </summary>
        public TimeSpan TimeToLive { get; set; }

        /// <summary>
        /// Gets the correlation id.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets the correlation id.
        /// </summary>
        public string MessageId { get; set; }

        public static implicit operator Envelope<T>(T body)
        {
            return Envelope.Create(body);
        }
    }
}
