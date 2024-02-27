using Xunit;
using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using System.Threading.Tasks;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.ConsumerService.Api.Contracts;
using Microsoft.AspNetCore.TestHost;
using HelpDesk.ConsumerService.Api.Controllers;
using HelpDesk.ConsumerService.Application.Contracts.Common;
using HelpDesk.ConsumerService.Api.IntegrationTests.SeedWork;
using HelpDesk.ConsumerService.Api.IntegrationTests.Fixtures;
using HelpDesk.ConsumerService.Application.Contracts.Tickets;
using HelpDesk.ConsumerService.Api.IntegrationTests.Extensions;
using HelpDesk.Core.Domain.Enumerations;

namespace HelpDesk.ConsumerService.Api.IntegrationTests.Scenarios
{
    [Collection(nameof(ApiCollection))]
    public sealed class TicketsControllerIntegrationTests : IAsyncLifetime
    {
        #region Read-Only Fields

        private readonly TestHostFixture _testHostFixture;
        private readonly Func<Task> _resetDatabase;

        private readonly TicketFixture _ticketFixture;
        private readonly CategoryFixture _categoryFixture;
        private readonly TicketStatusFixture _ticketStatusFixture;

        #endregion

        #region Constructors

        public TicketsControllerIntegrationTests(TestHostFixture fixture)
        {
            _testHostFixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            _resetDatabase = fixture.ResetDatabaseAsync;

            _ticketFixture = new TicketFixture(_testHostFixture);
            _categoryFixture = new CategoryFixture(_testHostFixture);
            _ticketStatusFixture = new TicketStatusFixture(_testHostFixture);
        }

        #endregion

        #region Tests

        #region Get

        [Fact]
        public async Task Get_ReturnsPagedListTicketResponse_WhenAuthenticatedWithUser()
        {
            // Arrange
            var ticketFixtureResult = await _ticketFixture.SetFixtureAsync();

            var parameters = new { Page = 1, PageSize = 10 };
            var userRequester = ticketFixtureResult.Users.FirstOrDefault(x => x.IdRole == (byte)UserRoles.General);
            var userRequesterTickets = ticketFixtureResult.Tickets.Where(x => x.IdUserRequester == userRequester.Id);

            // Act
            var response = await _testHostFixture.Server
                .CreateHttpApiRequest<TicketsController>(controller => controller.Get(parameters.Page, parameters.PageSize))
                .WithIdentity(userRequester)
                .GetAsync();

            // Assert
            await response.IsSuccessStatusCodeOrThrow();
            var responseContent = await response.ReadJsonContentAsAsync<PagedList<TicketResponse>>();

            responseContent.Should().NotBeNull();
            responseContent.Page.Should().Be(parameters.Page);
            responseContent.PageSize.Should().Be(parameters.PageSize);
            responseContent.TotalCount.Should().Be(userRequesterTickets.Count());

            responseContent.Items.Should().NotBeNullOrEmpty();
            responseContent.Items.Should().HaveCountLessThanOrEqualTo(parameters.PageSize);
            responseContent.Items.All(x => x.IdUserRequester == userRequester.Id).Should().BeTrue();
        }

        #endregion

        #region GetById

        [Fact]
        public async Task GetById_ReturnsUnathorized_WhenAnonymousUser()
        {
            // Arrange
            var parameters = new { Page = 1, PageSize = 10 };

            // Act
            var response = await _testHostFixture.Server
                .CreateHttpApiRequest<TicketsController>(controller => controller.Get(parameters.Page, parameters.PageSize))
                .GetAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetById_ReturnsDetailedTicketResponse_WhenAuthenticatedWithUser()
        {
            // Arrange
            var ticketFixtureResult = await _ticketFixture.SetFixtureAsync();

            var targetTicket = ticketFixtureResult.Tickets.FirstOrDefault(x => x.IdStatus == (byte)TicketStatuses.Completed);
            var ticketUserRequester = ticketFixtureResult.Users.FirstOrDefault(x => x.Id == targetTicket.IdUserRequester);
            var ticketStatus = (await _ticketStatusFixture.SetFixtureAsync()).FirstOrDefault(x => x.Id == targetTicket.IdStatus);
            var ticketCategory = (await _categoryFixture.SetFixtureAsync()).Categories.FirstOrDefault(x => x.Id == targetTicket.IdCategory);

            // Act
            var response = await _testHostFixture.Server
                .CreateHttpApiRequest<TicketsController>(controller => controller.GetById(targetTicket.Id))
                .WithIdentity(ticketUserRequester)
                .GetAsync();

            // Assert
            await response.IsSuccessStatusCodeOrThrow();
            var responseContent = await response.ReadJsonContentAsAsync<DetailedTicketResponse>();

            responseContent.Should().NotBeNull();
            responseContent.IdTicket.Should().Be(targetTicket.Id);
            responseContent.Description.Should().Be(targetTicket.Description);
            responseContent.CreatedAt.Should().Be(targetTicket.CreatedAt);
            responseContent.LastUpdatedAt.Should().Be(targetTicket.LastUpdatedAt);
            responseContent.LastUpdatedBy.Should().Be(targetTicket.LastUpdatedBy);
            responseContent.CancellationReason.Should().Be(targetTicket.CancellationReason);
            responseContent.IdUserRequester.Should().Be(targetTicket.IdUserRequester);
            responseContent.IdUserAssigned.Should().Be(targetTicket.IdUserAssigned);

            responseContent.Category.Should().NotBeNull();
            responseContent.Category.IdCategory.Should().Be(ticketCategory.Id);
            responseContent.Category.Name.Should().Be(ticketCategory.Name);

            responseContent.Status.Should().NotBeNull();
            responseContent.Status.IdStatus.Should().Be(ticketStatus.Id);
            responseContent.Status.Name.Should().Be(ticketStatus.Name);
        }

        [Fact]
        public async Task GetById_ReturnsUnauthorized_WhenAnonymousUser()
        {
            // Act
            var response = await _testHostFixture.Server
                .CreateHttpApiRequest<TicketsController>(controller => controller.GetById(int.MaxValue))
                .GetAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenInvalidTicket()
        {
            // Arrange
            var ticketFixtureResult = await _ticketFixture.SetFixtureAsync();

            var userRequester = ticketFixtureResult.Users.FirstOrDefault(x => x.IdRole == (byte)UserRoles.General);

            // Act
            var response = await _testHostFixture.Server
                .CreateHttpApiRequest<TicketsController>(controller => controller.GetById(int.MaxValue))
                .WithIdentity(userRequester)
                .GetAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseContent = await response.ReadContentAsAsync<ApiErrorResponse>();

            responseContent.Should().NotBeNull();
            responseContent.Errors.Should().Contain(DomainErrors.Ticket.NotFound);
        }

        [Fact]
        public async Task GetById_ReturnsForbidden_WhenGetTicketFromAnotherUsersWithGeneralUser()
        {
            // Arrange
            var ticketFixtureResult = await _ticketFixture.SetFixtureAsync();

            var userRequester = ticketFixtureResult.Users.FirstOrDefault(x => x.IdRole == (byte)UserRoles.General);
            var targetTicket = ticketFixtureResult.Tickets.FirstOrDefault(x => x.IdUserRequester != userRequester.Id);

            // Act
            var response = await _testHostFixture.Server
                .CreateHttpApiRequest<TicketsController>(controller => controller.GetById(targetTicket.Id))
                .WithIdentity(userRequester)
                .GetAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            var responseContent = await response.ReadContentAsAsync<ApiErrorResponse>();

            responseContent.Should().NotBeNull();
            responseContent.Errors.Should().Contain(DomainErrors.User.InvalidPermissions);
        }

        [Fact]
        public async Task GetById_ReturnsForbidden_WhenGetTicketFromAnotherUsersWithAnalystUser()
        {
            // Arrange
            var ticketFixtureResult = await _ticketFixture.SetFixtureAsync();

            var userRequester = ticketFixtureResult.Users.FirstOrDefault(x => x.IdRole == (byte)UserRoles.Analyst);
            var targetTicket = ticketFixtureResult.Tickets.FirstOrDefault(x => x.IdUserAssigned != userRequester.Id);

            // Act
            var response = await _testHostFixture.Server
                .CreateHttpApiRequest<TicketsController>(controller => controller.GetById(targetTicket.Id))
                .WithIdentity(userRequester)
                .GetAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            var responseContent = await response.ReadContentAsAsync<ApiErrorResponse>();

            responseContent.Should().NotBeNull();
            responseContent.Errors.Should().Contain(DomainErrors.User.InvalidPermissions);
        }

        #endregion
        
        #endregion

        #region IAsyncLifetime Members

        public Task InitializeAsync()
            => Task.CompletedTask;

        public Task DisposeAsync()
            => _resetDatabase();

        #endregion
    }
}
