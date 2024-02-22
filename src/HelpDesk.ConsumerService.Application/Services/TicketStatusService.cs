using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpDesk.ConsumerService.Application.Contracts.Tickets;
using HelpDesk.ConsumerService.Application.Core.Abstractions.Data;
using HelpDesk.ConsumerService.Application.Core.Abstractions.Services;
using HelpDesk.ConsumerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.ConsumerService.Application.Services
{
    internal sealed class TicketStatusService : ITicketStatusService
    {
        #region Read-Only Fields

        private readonly IDbContext _dbContext;

        #endregion

        #region Constructors

        public TicketStatusService(IDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        #endregion

        #region ITicketStatusService Members

        public async Task<IEnumerable<StatusResponse>> GetAsync()
        {
            IQueryable<StatusResponse> ticketStatusQuery =
                from ticketStatus in _dbContext.Set<TicketStatus, byte>().AsNoTracking()
                select new StatusResponse
                {
                    IdStatus = ticketStatus.Id,
                    Name = ticketStatus.Name
                }
            ;

            return await ticketStatusQuery.ToListAsync();
        }

        public async Task<StatusResponse> GetByIdAsync(byte idTicketStatus)
        {
            IQueryable<StatusResponse> ticketStatusQuery =
                from ticketStatus in _dbContext.Set<TicketStatus, byte>().AsNoTracking()
                where
                    ticketStatus.Id == idTicketStatus
                select new StatusResponse
                {
                    IdStatus = ticketStatus.Id,
                    Name = ticketStatus.Name,
                }
            ;

            return await ticketStatusQuery.FirstOrDefaultAsync();
        }

        #endregion
    }
}
