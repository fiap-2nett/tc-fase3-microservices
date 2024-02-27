using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HelpDesk.Core.Domain.Cryptography;
using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Events;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.ProducerService.Application.Core.Abstractions.EventBus;
using HelpDesk.ProducerService.Application.Services;
using HelpDesk.ProducerService.Application.UnitTests.TestEntities;
using HelpDesk.ProducerService.Domain.Dtos;
using HelpDesk.ProducerService.Domain.Repositories;
using Moq;
using Xunit;

namespace HelpDesk.ProducerService.Application.UnitTests.Scenarios
{
    public sealed class TicketServiceTests
    {
        #region Read-Only Fields

        private readonly IPasswordHasher _passwordHasher;
        private readonly Mock<IEventBus> _eventBusMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITicketRepository> _ticketRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;

        #endregion

        #region Constructors

        public TicketServiceTests()
        {
            _eventBusMock = new();
            _userRepositoryMock = new();
            _ticketRepositoryMock = new();
            _categoryRepositoryMock = new();

            _passwordHasher = new PasswordHasher();
        }

        #endregion

        #region Unit Tests

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_Should_ThrowNotFoundException_WhenInvalidUserRequester()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((UserDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CreateAsync(idCategory: CategoryList().FirstOrDefault().IdCategory,
                description: "Lorem ipsum dolor sit amet.", idUserRequester: int.MaxValue);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CreateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_Should_ThrowNotFoundException_WhenInvalidCategory()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserA);
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CategoryDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CreateAsync(idCategory: int.MaxValue,
                description: "Lorem ipsum dolor sit amet.", idUserRequester: UserA.IdUser);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.Category.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CreateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task CreateAsync_Should_ThrowDomainException_WhenDescriptionIsNullOrWhiteSpace(string description)
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserA);
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(CategoryList().FirstOrDefault());

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CreateAsync(idCategory: CategoryList().FirstOrDefault().IdCategory,
                description, idUserRequester: UserA.IdUser);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.DescriptionIsRequired.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CreateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_Should_ReturnTicketId_WhenValidParameters()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserA);
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(CategoryList().FirstOrDefault());

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            await ticketService.CreateAsync(idCategory: CategoryList().FirstOrDefault().IdCategory,
                description: "Lorem ipsum dolor sit amet.", idUserRequester: UserA.IdUser);

            // Assert
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CreateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_Should_ThrowNotFoundException_WhenInvalidUserPerformedAction()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((UserDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            var targetTicket = TicketList().FirstOrDefault();

            // Act
            var action = () => ticketService.UpdateAsync(
                idTicket: targetTicket.IdTicket,
                idCategory: targetTicket.Category.IdCategory,
                description: targetTicket.Description,
                idUserPerformedAction: targetTicket.IdUserRequester);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<UpdateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_Should_ThrowNotFoundException_WhenInvalidCategory()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserA);
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CategoryDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            var targetTicket = TicketList().FirstOrDefault();

            // Act
            var action = () => ticketService.UpdateAsync(
                idTicket: targetTicket.IdTicket,
                idCategory: targetTicket.Category.IdCategory,
                description: targetTicket.Description,
                idUserPerformedAction: targetTicket.IdUserRequester);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.Category.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<UpdateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_Should_ThrowNotFoundException_WhenInvalidTicket()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserA);
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(CategoryList().FirstOrDefault());
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TicketDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            var targetTicket = TicketList().FirstOrDefault();

            // Act
            var action = () => ticketService.UpdateAsync(
                idTicket: int.MaxValue,
                idCategory: targetTicket.Category.IdCategory,
                description: targetTicket.Description,
                idUserPerformedAction: targetTicket.IdUserRequester);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.Ticket.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<UpdateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task UpdateAsync_Should_ThrowDomainException_WhenDescriptionIsNullOrWhiteSpace(string description)
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserA);
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(CategoryList().FirstOrDefault());
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.UpdateAsync(
                idTicket: targetTicket.IdTicket,
                idCategory: targetTicket.Category.IdCategory,
                description: description,
                idUserPerformedAction: targetTicket.IdUserRequester);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.DescriptionIsRequired.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<UpdateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_Should_ThrowDomainException_WhenUserPerformedActionIsNotTheUserRequester()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserB);
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(CategoryList().FirstOrDefault());
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.UpdateAsync(
                idTicket: targetTicket.IdTicket,
                idCategory: targetTicket.Category.IdCategory,
                description: targetTicket.Description,
                idUserPerformedAction: UserB.IdUser);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.User.InvalidPermissions.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<UpdateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(10_102 /*Completed Ticket*/)]
        [InlineData(10_103 /*Cancelled Ticket*/)]
        public async Task UpdateAsync_Should_ThrowDomainException_WhenTicketStatusIdCompletedOrCancelled(int idTicket)
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.IdTicket == idTicket);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserA);
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(CategoryList().FirstOrDefault());
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.UpdateAsync(
                idTicket: targetTicket.IdTicket,
                idCategory: targetTicket.Category.IdCategory,
                description: targetTicket.Description,
                idUserPerformedAction: targetTicket.IdUserRequester);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<UpdateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_WhenValidParameters()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var changedCategory = CategoryList().LastOrDefault();
            var updatedTicketDescription = "Updated ticket description";
            var userPerformedAction = UserList().FirstOrDefault(x => x.IdUser == targetTicket.IdUserRequester);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(userPerformedAction);
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(changedCategory);
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            await ticketService.UpdateAsync(
                idTicket: targetTicket.IdTicket,
                idCategory: changedCategory.IdCategory,
                description: updatedTicketDescription,
                idUserPerformedAction: userPerformedAction.IdUser);

            // Assert
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<UpdateTicketEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region ChangeStatusAsync

        [Fact]
        public async Task ChangeStatusAsync_Should_ThrowNotFoundException_WhenInvalidUserPerformedAction()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((UserDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            var targetTicket = TicketList().FirstOrDefault();

            // Act
            var action = () => ticketService.ChangeStatusAsync(
                idTicket: targetTicket.IdTicket,
                changedStatus: TicketStatuses.OnHold,
                idUserPerformedAction: targetTicket.IdUserRequester);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<ChangeStatusTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ChangeStatusAsync_Should_ThrowNotFoundException_WhenInvalidTicket()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserA);
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TicketDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            var targetTicket = TicketList().FirstOrDefault();

            // Act
            var action = () => ticketService.ChangeStatusAsync(
                idTicket: targetTicket.IdTicket,
                changedStatus: TicketStatuses.OnHold,
                idUserPerformedAction: targetTicket.IdUserRequester);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.Ticket.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<ChangeStatusTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(3 /*General UserRole*/)]
        [InlineData(4 /*Analyst UserRole*/)]
        public async Task ChangeStatusAsync_Should_ThrowInvalidPermissionException_WhenInvalidPerformedUserToThisAction(int idUserPerformedAction)
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserList().FirstOrDefault(x => x.IdUser == idUserPerformedAction));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.ChangeStatusAsync(
                idTicket: targetTicket.IdTicket,
                changedStatus: TicketStatuses.OnHold,
                idUserPerformedAction: targetTicket.IdUserRequester);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.Ticket.StatusCannotBeChangedByThisUser.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<ChangeStatusTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ChangeStatusAsync_Should_ThrowDomainException_WhenChangeStatusToNew()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(UserList().FirstOrDefault(x => x.IdUser == Admin.IdUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.ChangeStatusAsync(
                idTicket: targetTicket.IdTicket,
                changedStatus: TicketStatuses.New,
                idUserPerformedAction: Admin.IdUser);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.CannotChangeStatusToNew.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<ChangeStatusTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(10_102 /*Completed Ticket*/)]
        [InlineData(10_103 /*Cancelled Ticket*/)]
        public async Task ChangeStatusAsync_Should_ThrowDomainException_WhenHasAlreadyBeenCompletedOrCancelled(int idTicket)
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.IdTicket == idTicket);
            var userAssigned = UserList().FirstOrDefault(x => x.IdUser == targetTicket.IdUserAssigned);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(userAssigned);
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.ChangeStatusAsync(
                idTicket: targetTicket.IdTicket,
                changedStatus: TicketStatuses.OnHold,
                idUserPerformedAction: userAssigned.IdUser);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<ChangeStatusTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(TicketStatuses.Assigned)]
        [InlineData(TicketStatuses.Cancelled)]
        public async Task ChangeStatusAsync_Should_ThrowDomainException_WhenHasStatusNotAllowed(TicketStatuses changedStatus)
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.Status.IdStatus == (byte)TicketStatuses.Assigned);
            var userAssigned = UserList().FirstOrDefault(x => x.IdUser == targetTicket.IdUserAssigned);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(userAssigned);
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.ChangeStatusAsync(
                idTicket: targetTicket.IdTicket,
                changedStatus: changedStatus,
                idUserPerformedAction: userAssigned.IdUser);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.StatusNotAllowed.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<ChangeStatusTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ChangeStatusAsync_Should_Return_WhenValidParameters()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.Status.IdStatus == (byte)TicketStatuses.Assigned);
            var userAssigned = UserList().FirstOrDefault(x => x.IdUser == targetTicket.IdUserAssigned);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(userAssigned);
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            await ticketService.ChangeStatusAsync(
                idTicket: targetTicket.IdTicket,
                changedStatus: TicketStatuses.InProgress,
                idUserPerformedAction: userAssigned.IdUser);

            // Assert
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<ChangeStatusTicketEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region AssignToUserAsync

        [Fact]
        public async Task AssignToUserAsync_Should_ThrowNotFoundException_WhenInvalidUserAssigned()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserAssigned = int.MaxValue;
            var idUserPerformedAction = Admin.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.AssignToUserAsync(targetTicket.IdTicket, idUserAssigned, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<AssignToUserTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AssignToUserAsync_Should_ThrowNotFoundException_WhenInvalidUserPerformedAction()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserAssigned = AnalystA.IdUser;
            var idUserPerformedAction = int.MaxValue;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.AssignToUserAsync(targetTicket.IdTicket, idUserAssigned, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<AssignToUserTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AssignToUserAsync_Should_ThrowNotFoundException_WhenInvalidTicketId()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserAssigned = AnalystA.IdUser;
            var idUserPerformedAction = AnalystA.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TicketDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.AssignToUserAsync(targetTicket.IdTicket, idUserAssigned, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.Ticket.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<AssignToUserTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AssignToUserAsync_Should_ThrowInvalidPermissionException_WhenAssignedUserIsNotAnalyst()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserAssigned = UserB.IdUser;
            var idUserPerformedAction = Admin.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.AssignToUserAsync(targetTicket.IdTicket, idUserAssigned, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.Ticket.CannotBeAssignedToThisUser.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<AssignToUserTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AssignToUserAsync_Should_ThrowInvalidPermissionException_WhenUserPerformedActionIsGeneral()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserAssigned = AnalystA.IdUser;
            var idUserPerformedAction = targetTicket.IdUserRequester;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.AssignToUserAsync(targetTicket.IdTicket, idUserAssigned, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.User.InvalidPermissions.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<AssignToUserTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(10_102 /*Completed Ticket*/)]
        [InlineData(10_103 /*Cancelled Ticket*/)]
        public async Task AssignToUserAsync_Should_ThrowInvalidPermissionException_WhenTicketHasAlreadyBeenCompletedOrCancelledl(int idTicket)
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.IdTicket == idTicket);
            var idUserAssigned = AnalystA.IdUser;
            var idUserPerformedAction = AnalystA.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.AssignToUserAsync(targetTicket.IdTicket, idUserAssigned, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<AssignToUserTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AssignToUserAsync_Should_ThrowInvalidPermissionException_WhenTicketStatusIsNewAndUserPerformedActionIsNotAdministratorOrAssignedUser()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserAssigned = AnalystA.IdUser;
            var idUserPerformedAction = AnalystB.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.AssignToUserAsync(targetTicket.IdTicket, idUserAssigned, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.User.InvalidPermissions.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<AssignToUserTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AssignToUserAsync_Should_ThrowInvalidPermissionException_WhenTicketStatusIsAssignedAndUserPerformedActionIsNotAdministratorOrAssignedUser()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.Status.IdStatus == (byte)TicketStatuses.Assigned);
            var idUserAssigned = AnalystB.IdUser;
            var idUserPerformedAction = AnalystB.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.AssignToUserAsync(targetTicket.IdTicket, idUserAssigned, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.User.InvalidPermissions.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<AssignToUserTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AssignToUserAsync_Should_Return_WhenValidParameters()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserAssigned = AnalystA.IdUser;
            var idUserPerformedAction = Admin.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            await ticketService.AssignToUserAsync(targetTicket.IdTicket, idUserAssigned, idUserPerformedAction);

            // Assert
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<AssignToUserTicketEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region CompleteAsync

        [Fact]
        public async Task CompleteAsync_Should_ThrowNotFoundException_WhenInvalidUserPerformendAction()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserPerformedAction = int.MaxValue;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CompleteAsync(targetTicket.IdTicket, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CompleteTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteAsync_Should_ThrowNotFoundException_WhenInvalidTicketId()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserPerformedAction = AnalystA.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TicketDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CompleteAsync(targetTicket.IdTicket, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.Ticket.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CompleteTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteAsync_Should_ThrowInvalidPermissionException_WhenUserPerformedActionIsGeneral()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserPerformedAction = targetTicket.IdUserRequester;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CompleteAsync(targetTicket.IdTicket, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.Ticket.CannotBeCompletedByThisUser.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CompleteTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteAsync_Should_ThrowInvalidPermissionException_WhenUserPerformedActionIsNotAssignedUser()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.Status.IdStatus == (byte)TicketStatuses.Assigned);
            var idUserPerformedAction = AnalystB.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CompleteAsync(targetTicket.IdTicket, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.Ticket.CannotBeCompletedByThisUser.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CompleteTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteAsync_Should_ThrowDomainException_WhenTicketStatusIsNew()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserPerformedAction = Admin.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CompleteAsync(targetTicket.IdTicket, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.HasNotBeenAssignedToAUser.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CompleteTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(10_102 /*Completed Ticket*/)]
        [InlineData(10_103 /*Cancelled Ticket*/)]
        public async Task CompleteAsync_Should_ThrowDomainException_WhenTicketStatusIsCompletedOrCancelled(int idTicket)
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.IdTicket == idTicket);
            var idUserPerformedAction = AnalystA.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CompleteAsync(targetTicket.IdTicket, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CompleteTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompleteAsync_Should_Return_WhenValidParameters()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.Status.IdStatus == (byte)TicketStatuses.Assigned);
            var idUserPerformedAction = AnalystA.IdUser;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            await ticketService.CompleteAsync(targetTicket.IdTicket, idUserPerformedAction);

            // Assert
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CompleteTicketEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region CancelAsync

        [Fact]
        public async Task CancelAsync_Should_ThrowNotFoundException_WhenInvalidUserPerformendAction()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserPerformedAction = int.MaxValue;
            var cancellationReason = "Lorem ipsum dolor sit amet.";

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CancelAsync(targetTicket.IdTicket, cancellationReason, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.User.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CancelTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelAsync_Should_ThrowNotFoundException_WhenInvalidTicketId()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserPerformedAction = AnalystA.IdUser;
            var cancellationReason = "Lorem ipsum dolor sit amet.";

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((TicketDto)null);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CancelAsync(targetTicket.IdTicket, cancellationReason, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage(DomainErrors.Ticket.NotFound.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CancelTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelAsync_Should_ThrowInvalidPermissionException_WhenUserPerformedActionIsGeneralAndNotUserRequester()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault();
            var idUserPerformedAction = UserB.IdUser;
            var cancellationReason = "Lorem ipsum dolor sit amet.";

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CancelAsync(targetTicket.IdTicket, cancellationReason, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.Ticket.CannotBeCancelledByThisUser.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CancelTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelAsync_Should_ThrowInvalidPermissionException_WhenUserPerformedActionIsAnalystAndNotUserRequesterOrUserAssigned()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.Status.IdStatus == (byte)TicketStatuses.Assigned);
            var idUserPerformedAction = AnalystB.IdUser;
            var cancellationReason = "Lorem ipsum dolor sit amet.";

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CancelAsync(targetTicket.IdTicket, cancellationReason, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<InvalidPermissionException>()
                .WithMessage(DomainErrors.Ticket.CannotBeCancelledByThisUser.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CancelTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task CancelAsync_Should_ThrowDomainException_WhenCancellationReasonIsNullOrWhiteSpace(string cancellationReason)
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.Status.IdStatus == (byte)TicketStatuses.Assigned);
            var idUserPerformedAction = targetTicket.IdUserRequester;

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CancelAsync(targetTicket.IdTicket, cancellationReason, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.CancellationReasonIsRequired.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CancelTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(10_102 /*Completed Ticket*/)]
        [InlineData(10_103 /*Cancelled Ticket*/)]
        public async Task CancelAsync_Should_ThrowDomainException_WhenTicketStatusIsCompletedOrCancelled(int idTicket)
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.IdTicket == idTicket);
            var idUserPerformedAction = targetTicket.IdUserRequester;
            var cancellationReason = "Lorem ipsum dolor sit amet.";

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            var action = () => ticketService.CancelAsync(targetTicket.IdTicket, cancellationReason, idUserPerformedAction);

            // Assert
            await action.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(DomainErrors.Ticket.HasAlreadyBeenCompletedOrCancelled.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CancelTicketEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CancelAsync_Should_Return_WhenValidParameters()
        {
            // Arrange
            var targetTicket = TicketList().FirstOrDefault(x => x.Status.IdStatus == (byte)TicketStatuses.Assigned);
            var idUserPerformedAction = AnalystA.IdUser;
            var cancellationReason = "Lorem ipsum dolor sit amet.";

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int idUser) => UserList().FirstOrDefault(x => x.IdUser == idUser));
            _ticketRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(targetTicket);

            var ticketService = new TicketService(_eventBusMock.Object, _ticketRepositoryMock.Object,
                _userRepositoryMock.Object, _categoryRepositoryMock.Object);

            // Act
            await ticketService.CancelAsync(targetTicket.IdTicket, cancellationReason, idUserPerformedAction);

            // Assert
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _ticketRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _eventBusMock.Verify(x => x.PublishAsync(It.IsAny<CancelTicketEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #endregion

        #region Private Methods

        private IEnumerable<CategoryDto> CategoryList()
        {
            yield return new CategoryDto(IdCategory: 1, Name: "Indisponibilidade");
            yield return new CategoryDto(IdCategory: 2, Name: "Lentidão");
            yield return new CategoryDto(IdCategory: 3, Name: "Requisição");
            yield return new CategoryDto(IdCategory: 4, Name: "Dúvida");
        }

        private IEnumerable<TicketDto> TicketList()
        {
            yield return TicketTestDto.Create(10_100, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: UserA);
            yield return TicketTestDto.Create(10_101, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: UserA, userAssigned: AnalystA);
            yield return TicketTestDto.Create(10_102, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: UserA, userAssigned: AnalystA, toComplete: true);
            yield return TicketTestDto.Create(10_103, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: UserA, userAssigned: AnalystA, toCancelled: true);

            yield return TicketTestDto.Create(10_200, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: UserB);
            yield return TicketTestDto.Create(10_201, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: UserB, userAssigned: AnalystB);

            yield return TicketTestDto.Create(10_300, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: AnalystA);
            yield return TicketTestDto.Create(10_301, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: AnalystA, userAssigned: AnalystB);

            yield return TicketTestDto.Create(10_400, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: AnalystB);
            yield return TicketTestDto.Create(10_401, category: CategoryList().FirstOrDefault(), description: "Lorem ipsum dolor sit amet.", userRequester: AnalystB, userAssigned: AnalystA);
        }

        private UserDto Admin
            => new UserDto(1, UserRoles.Administrator);

        private UserDto UserA
            => new UserDto(2, UserRoles.General);

        private UserDto UserB
            => new UserDto(3, UserRoles.General);

        private UserDto AnalystA
            => new UserDto(4, UserRoles.Analyst);

        private UserDto AnalystB
            => new UserDto(5, UserRoles.Analyst);

        private IEnumerable<UserDto> UserList()
        {
            yield return Admin;
            yield return UserA;
            yield return UserB;
            yield return AnalystA;
            yield return AnalystB;
        }

        #endregion
    }
}
