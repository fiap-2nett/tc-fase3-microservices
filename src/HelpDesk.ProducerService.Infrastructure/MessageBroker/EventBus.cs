using System;
using System.Threading;
using System.Threading.Tasks;
using HelpDesk.ProducerService.Application.Core.Abstractions.EventBus;
using MassTransit;

namespace HelpDesk.ProducerService.Infrastructure.MessageBroker
{
    internal sealed class EventBus : IEventBus
    {
        #region Read-Only Fields

        private readonly IPublishEndpoint _publishEndpoint;

        #endregion

        #region Constructors

        public EventBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        #endregion

        #region IEventBus Members

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
            => await _publishEndpoint.Publish(@event, cancellationToken);

        #endregion
    }
}
