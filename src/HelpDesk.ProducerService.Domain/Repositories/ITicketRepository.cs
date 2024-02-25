using System.Threading.Tasks;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Domain.Repositories
{
    public interface ITicketRepository
    {
        #region ITicketRepository Members

        Task<TicketDto> GetByIdAsync(int idTicket);        

        #endregion
    }
}
