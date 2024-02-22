using System.Collections.Generic;
using System.Threading.Tasks;
using HelpDesk.ConsumerService.Application.Contracts.Tickets;

namespace HelpDesk.ConsumerService.Application.Core.Abstractions.Services
{
    public interface ITicketStatusService
    {
        #region ITicketStatusService Members

        Task<IEnumerable<StatusResponse>> GetAsync();
        Task<StatusResponse> GetByIdAsync(byte idTicketStatus);

        #endregion
    }
}
