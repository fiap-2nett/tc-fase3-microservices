using System.Threading.Tasks;
using HelpDesk.Core.Domain.Enumerations;

namespace HelpDesk.ProducerService.Application.Core.Abstractions.Services
{
    public interface ITicketService
    {
        #region ITicketService Members

        Task<int> CreateAsync(int idCategory, string description, int idUserRequester);
        Task UpdateAsync(int idTicket, int idCategory, string description, int idUserPerformedAction);
        Task ChangeStatusAsync(int idTicket, TicketStatuses changedStatus, int idUserPerformedAction);
        Task CancelAsync(int idTicket, string cancellationReason, int idUserPerformedAction);
        Task AssignToUserAsync(int idTicket, int idUserAssigned, int idUserPerformedAction);
        Task CompleteAsync(int idTicket, int idUserPerformedAction);

        #endregion
    }
}
