using System;
using System.Threading.Tasks;
using HelpDesk.Core.Domain.Authentication;
using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.Core.Domain.Helpers;
using HelpDesk.ProducerService.Api.Constants;
using HelpDesk.ProducerService.Api.Contracts;
using HelpDesk.ProducerService.Api.Infrastructure;
using HelpDesk.ProducerService.Application.Core.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.ProducerService.Api.Controllers
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
        /// Represents the request to create a ticket.
        /// </summary>
        /// <param name="createTicketRequest">Represents the request to create a ticket</param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Tickets.Create)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest createTicketRequest)
        {
            if (createTicketRequest is null)
                throw new DomainException(DomainErrors.Ticket.DataSentIsInvalid);

            await _ticketService.CreateAsync(createTicketRequest.IdCategory,
                createTicketRequest.Description,
                idUserRequester: _userSessionProvider.IdUser);
            
            return Ok();
        }

        /// <summary>
        /// Represents the request to assign the ticket to the logged in user.
        /// </summary>
        /// <param name="idTicket">The ticket identifier.</param>
        [HttpPost(ApiRoutes.Tickets.AssignToMe)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignToMe([FromRoute] int idTicket)
        {
            await _ticketService.AssignToUserAsync(idTicket,
                idUserAssigned: _userSessionProvider.IdUser,
                idUserPerformedAction: _userSessionProvider.IdUser);

            return Ok();
        }

        /// <summary>
        /// Represents the request to update the ticket.
        /// </summary>
        /// <param name="idTicket">The ticket identifier.</param>
        /// <param name="updateTicketRequest">Represents the request to update the ticket.</param>
        [HttpPut(ApiRoutes.Tickets.Update)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int idTicket, [FromBody] UpdateTicketRequest updateTicketRequest)
        {
            if (updateTicketRequest is null)
                throw new DomainException(DomainErrors.Ticket.DataSentIsInvalid);

            await _ticketService.UpdateAsync(idTicket,
                updateTicketRequest.IdCategory,
                updateTicketRequest.Description,
                idUserPerformedAction: _userSessionProvider.IdUser);

            return Ok();
        }

        /// <summary>
        /// Represents the request to cancel the ticket.
        /// </summary>
        /// <param name="idTicket">The ticket identifier.</param>
        /// <param name="cancelTicketRequest">Represents the request to cancel the ticket.</param>
        [HttpPost(ApiRoutes.Tickets.Cancel)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel([FromRoute] int idTicket, [FromBody] CancelTicketRequest cancelTicketRequest)
        {
            if (cancelTicketRequest is null)
                throw new DomainException(DomainErrors.Ticket.DataSentIsInvalid);

            await _ticketService.CancelAsync(idTicket,
                cancelTicketRequest.CancellationReason,
                idUserPerformedAction: _userSessionProvider.IdUser);

            return Ok();
        }

        /// <summary>
        /// Represents the request to assign the ticket to other users.
        /// </summary>
        /// <param name="idTicket">The ticket identifier.</param>
        /// <param name="assignToRequest">Represents the request to assign the ticket to other users</param>
        [HttpPost(ApiRoutes.Tickets.AssignTo)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignTo([FromRoute] int idTicket, [FromBody] AssignToRequest assignToRequest)
        {
            if (assignToRequest is null)
                throw new DomainException(DomainErrors.Ticket.DataSentIsInvalid);

            await _ticketService.AssignToUserAsync(idTicket,
                idUserAssigned: assignToRequest.IdUserAssigned,
                idUserPerformedAction: _userSessionProvider.IdUser);

            return Ok();
        }

        /// <summary>
        /// Represents the request to complete the ticket.
        /// </summary>
        /// <param name="idTicket">The ticket identifier.</param>
        [HttpPost(ApiRoutes.Tickets.Complete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Complete([FromRoute] int idTicket)
        {
            await _ticketService.CompleteAsync(idTicket, _userSessionProvider.IdUser);

            return Ok();
        }

        /// <summary>
        /// Represents the request to change the ticket status. Internal Movement only.
        /// </summary>
        /// <param name="idTicket">The ticket identifier.</param>
        /// <param name="changeStatusRequest">Represents the request to change the ticket status.</param>
        [HttpPost(ApiRoutes.Tickets.ChangeStatus)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangeStatus([FromRoute] int idTicket, [FromBody] ChangeStatusRequest changeStatusRequest)
        {
            if (changeStatusRequest is null)
                throw new DomainException(DomainErrors.Ticket.DataSentIsInvalid);

            if (!EnumHelper.TryConvert(changeStatusRequest.IdStatus, out TicketStatuses changedStatus))
                throw new DomainException(DomainErrors.Ticket.StatusDoesNotExist);

            await _ticketService.ChangeStatusAsync(idTicket, changedStatus, _userSessionProvider.IdUser);            

            return Ok();
        }

        #endregion
    }
}
