using System;
using System.Threading.Tasks;
using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Events;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.Core.Domain.Extensions;
using HelpDesk.ProducerService.Application.Core.Abstractions.EventBus;
using HelpDesk.ProducerService.Application.Core.Abstractions.Services;
using HelpDesk.ProducerService.Domain.Repositories;

namespace HelpDesk.ProducerService.Application.Services
{
    internal sealed class TicketService : ITicketService
    {
        #region Read-Only Fields

        private readonly IEventBus _eventBus;
        private readonly IUserRepository _userRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ICategoryRepository _categoryRepository;

        #endregion

        #region Constructors

        public TicketService(IEventBus eventBus,
            ITicketRepository ticketRepository, IUserRepository userRepository, ICategoryRepository categoryRepository)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        #endregion

        #region ITicketService Members

        public async Task CreateAsync(int idCategory, string description, int idUserRequester)
        {
            var userRequester = await _userRepository.GetByIdAsync(idUserRequester);
            if (userRequester is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var category = await _categoryRepository.GetByIdAsync(idCategory);
            if (category is null)
                throw new NotFoundException(DomainErrors.Category.NotFound);

            await _eventBus.PublishAsync(new CreateTicketEvent(category.IdCategory, userRequester.IdUser, description));
        }

        public async Task UpdateAsync(int idTicket, int idCategory, string description, int idUserPerformedAction)
        {
            var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            if (userPerformedAction is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var category = await _categoryRepository.GetByIdAsync(idCategory);
            if (category is null)
                throw new NotFoundException(DomainErrors.Category.NotFound);

            var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            if (ticket is null)
                throw new NotFoundException(DomainErrors.Ticket.NotFound);

            //TODO: Implementar o consumer do MasstTransit [UpdateTicketEvent]
            await _eventBus.PublishAsync(new UpdateTicketEvent(ticket.IdTicket, category.IdCategory,
                description, userPerformedAction.IdUser));
        }

        public async Task CancelAsync(int idTicket, string cancellationReason, int idUserPerformedAction)
        {
            var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            if (userPerformedAction is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            if (ticket is null)
                throw new NotFoundException(DomainErrors.Ticket.NotFound);

            if (cancellationReason.IsNullOrWhiteSpace())
                throw new DomainException(DomainErrors.Ticket.CancellationReasonIsRequired);

            //TODO: Implementar o consumer do MasstTransit [CancelTicketEvent]
            await _eventBus.PublishAsync(new CancelTicketEvent(ticket.IdTicket, cancellationReason,
                userPerformedAction.IdUser));
        }

        public async Task ChangeStatusAsync(int idTicket, TicketStatuses changedStatus, int idUserPerformedAction)
        {
            var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            if (userPerformedAction is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            if (ticket is null)
                throw new NotFoundException(DomainErrors.Ticket.NotFound);

            if (ticket.TicketStatus == TicketStatuses.Completed || ticket.TicketStatus == TicketStatuses.Cancelled)
                throw new DomainException(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled);

            //TODO: Implementar o consumer do MasstTransit [ChangeStatusTicketEvent]
            await _eventBus.PublishAsync(new ChangeStatusTicketEvent(ticket.IdTicket, changedStatus,
                userPerformedAction.IdUser));
        }

        public async Task AssignToUserAsync(int idTicket, int idUserAssigned, int idUserPerformedAction)
        {
            var userAssigned = await _userRepository.GetByIdAsync(idUserAssigned);
            if (userAssigned is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            if (userPerformedAction is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            if (ticket is null)
                throw new NotFoundException(DomainErrors.Ticket.NotFound);

            //TODO: Implementar o consumer do MasstTransit [AssignToUserTicketEvent]
            await _eventBus.PublishAsync(new AssignToUserTicketEvent(ticket.IdTicket, userAssigned.IdUser,
                userPerformedAction.IdUser));
        }

        public async Task CompleteAsync(int idTicket, int idUserPerformedAction)
        {
            var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            if (userPerformedAction is null)
                throw new NotFoundException(DomainErrors.User.NotFound);

            var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            if (ticket is null)
                throw new NotFoundException(DomainErrors.Ticket.NotFound);

            //TODO: Implementar o consumer do MasstTransit [CompleteTicketEvent]
            await _eventBus.PublishAsync(new CompleteTicketEvent(ticket.IdTicket, userPerformedAction.IdUser));
        }

        #endregion
    }
}
