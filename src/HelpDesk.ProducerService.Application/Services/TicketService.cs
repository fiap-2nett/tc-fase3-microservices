using System;
using System.Threading.Tasks;
using HelpDesk.Core.Domain.Events;
using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.ProducerService.Application.Core.Abstractions.EventBus;
using HelpDesk.ProducerService.Application.Core.Abstractions.Services;

namespace HelpDesk.ProducerService.Application.Services
{
    internal sealed class TicketService : ITicketService
    {
        #region Read-Only Fields

        private readonly IEventBus _eventBus;

        #endregion

        #region Constructors

        public TicketService(IEventBus eventBus)
        {            
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        #endregion

        #region ITicketService Members

        public async Task<int> CreateAsync(int idCategory, string description, int idUserRequester)
        {

            await _eventBus.PublishAsync(new CreateTicketEvent
            {
                IdCategory = idCategory,
                Description = description,
                IdUserRequester = idUserRequester
            });

            return 0;

            //var userRequester = await _userRepository.GetByIdAsync(idUserRequester);
            //if (userRequester is null)
            //    throw new NotFoundException(DomainErrors.User.NotFound);

            //var category = await _categoryRepository.GetByIdAsync(idCategory);
            //if (category is null)
            //    throw new NotFoundException(DomainErrors.Category.NotFound);

            //var ticket = new Ticket(category, description, userRequester);

            //_ticketRepository.Insert(ticket);
            //await _unitOfWork.SaveChangesAsync();

            //return ticket.Id;
        }

        public async Task UpdateAsync(int idTicket, int idCategory, string description, int idUserPerformedAction)
        {
            //var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            //if (userPerformedAction is null)
            //    throw new NotFoundException(DomainErrors.User.NotFound);

            //var category = await _categoryRepository.GetByIdAsync(idCategory);
            //if (category is null)
            //    throw new NotFoundException(DomainErrors.Category.NotFound);

            //var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            //if (ticket is null)
            //    throw new NotFoundException(DomainErrors.Ticket.NotFound);

            //ticket.Update(category, description, userPerformedAction);
            //await _unitOfWork.SaveChangesAsync();

            //TODO: Implementar validações e comunição com MasstTransit
        }

        public async Task CancelAsync(int idTicket, string cancellationReason, int idUserPerformedAction)
        {
            //var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            //if (userPerformedAction is null)
            //    throw new NotFoundException(DomainErrors.User.NotFound);

            //var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            //if (ticket is null)
            //    throw new NotFoundException(DomainErrors.Ticket.NotFound);

            //ticket.Cancel(cancellationReason, userPerformedAction);
            //await _unitOfWork.SaveChangesAsync();

            //TODO: Implementar validações e comunição com MasstTransit
        }

        public async Task ChangeStatusAsync(int idTicket, TicketStatuses changedStatus, int idUserPerformedAction)
        {
            //var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            //if (userPerformedAction is null)
            //    throw new NotFoundException(DomainErrors.User.NotFound);

            //var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            //if (ticket is null)
            //    throw new NotFoundException(DomainErrors.Ticket.NotFound);

            //ticket.ChangeStatus(changedStatus, userPerformedAction);
            //await _unitOfWork.SaveChangesAsync();

            //TODO: Implementar validações e comunição com MasstTransit
        }

        public async Task AssignToUserAsync(int idTicket, int idUserAssigned, int idUserPerformedAction)
        {
            //var userAssigned = await _userRepository.GetByIdAsync(idUserAssigned);
            //if (userAssigned is null)
            //    throw new NotFoundException(DomainErrors.User.NotFound);

            //var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            //if (userPerformedAction is null)
            //    throw new NotFoundException(DomainErrors.User.NotFound);

            //var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            //if (ticket is null)
            //    throw new NotFoundException(DomainErrors.Ticket.NotFound);

            //ticket.AssignTo(userAssigned, userPerformedAction);
            //await _unitOfWork.SaveChangesAsync();

            //TODO: Implementar validações e comunição com MasstTransit
        }

        public async Task CompleteAsync(int idTicket, int idUserPerformedAction)
        {
            //var userPerformedAction = await _userRepository.GetByIdAsync(idUserPerformedAction);
            //if (userPerformedAction is null)
            //    throw new NotFoundException(DomainErrors.User.NotFound);

            //var ticket = await _ticketRepository.GetByIdAsync(idTicket);
            //if (ticket is null)
            //    throw new NotFoundException(DomainErrors.Ticket.NotFound);

            //ticket.Complete(userPerformedAction);
            //await _unitOfWork.SaveChangesAsync();

            //TODO: Implementar validações e comunição com MasstTransit
        }

        #endregion
    }
}
