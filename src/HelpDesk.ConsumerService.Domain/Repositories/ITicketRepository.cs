using System.Threading.Tasks;
using HelpDesk.ConsumerService.Domain.Entities;

namespace HelpDesk.ConsumerService.Domain.Repositories
{
    public interface ITicketRepository
    {
        #region ITicketRepository Members

        Task<Ticket> GetByIdAsync(int idTicket);
        void Insert(Ticket ticket);

        #endregion
    }
}
