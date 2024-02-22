using HelpDesk.ConsumerService.Application.Core.Abstractions.Data;
using HelpDesk.ConsumerService.Domain.Entities;
using HelpDesk.ConsumerService.Domain.Repositories;
using HelpDesk.ConsumerService.Persistence.Core.Primitives;

namespace HelpDesk.ConsumerService.Persistence.Repositories
{
    internal sealed class TicketRepository : GenericRepository<Ticket, int>, ITicketRepository
    {
        #region Constructors

        public TicketRepository(IDbContext dbContext)
            : base(dbContext)
        { }

        #endregion
    }
}
