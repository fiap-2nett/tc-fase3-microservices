using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.ProducerService.Application.Core.Abstractions.EventBus
{
    public  interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class;
    }
}
