using System;
using System.Threading.Tasks;
using HelpDesk.ConsumerService.Api.Contracts;
using HelpDesk.ConsumerService.Api.Infrastructure;
using HelpDesk.ConsumerService.Application.Contracts.Common;
using HelpDesk.ConsumerService.Application.Contracts.Tickets;
using HelpDesk.ConsumerService.Application.Core.Abstractions.Services;
using HelpDesk.Core.Domain.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.ConsumerService.Api.Controllers
{
    public sealed class TicketsController : ApiController
    {
        #region Read-Only Fields

        private readonly ITicketService _ticketService;
        private readonly IUserSessionProvider _userSessionProvider;

        #endregion

        #region Constructors

        public TicketsController(ITicketService ticketService, IUserSessionProvider userSessionProvider)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _userSessionProvider = userSessionProvider ?? throw new ArgumentNullException(nameof(userSessionProvider));
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Represents the query to retrieve all tickets.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size. The max page size is 100.</param>
        /// <returns>The paged list of the tickets.</returns>
        [HttpGet(ApiRoutes.Tickets.Get)]
        [ProducesResponseType(typeof(PagedList<TicketResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _ticketService.GetTicketsAsync(new GetTicketsRequest(page, pageSize), _userSessionProvider.IdUser));

        /// <summary>
        /// Represents the query to retrieve a specific ticket.
        /// </summary>
        /// <param name="idTicket">The ticket identifier.</param>
        /// <returns>The detailed ticket info.</returns>
        [HttpGet(ApiRoutes.Tickets.GetById)]
        [ProducesResponseType(typeof(DetailedTicketResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int idTicket)
            => Ok(await _ticketService.GetTicketByIdAsync(idTicket, _userSessionProvider.IdUser));
        
        #endregion
    }
}
