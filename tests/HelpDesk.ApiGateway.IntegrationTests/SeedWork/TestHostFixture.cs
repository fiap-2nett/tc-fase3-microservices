using System;
using System.Threading.Tasks;
using HelpDesk.ApiGateway.IntegrationTests.Extensions;
using HelpDesk.ApiGateway.Persistence;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;
using Xunit;

namespace HelpDesk.ApiGateway.IntegrationTests.SeedWork
{
    public sealed class TestHostFixture : WebApplicationFactory<TestStartup>, IAsyncLifetime
    {
        #region Read-Only Fields

        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .WithCleanUp(true)
            .Build();

        #endregion

        #region Public Methods

        public async Task ResetDatabaseAsync()
        {
            await using var _dbConnection = new SqlConnection(_dbContainer.GetConnectionString());
            await _dbConnection.OpenAsync();

            var _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore = new Table[]
                {
                    "__EFMigrationsHistory"
                },
                WithReseed = true
            });

            await _respawner.ResetAsync(_dbConnection);
        }

        public async Task ExecuteDbContextAsync(Func<EFContext, Task> action)
            => await ExecuteScopeAsync(sp => action(sp.GetService<EFContext>()));

        #endregion

        #region Overriden Methods

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<EFContext>();
                services.AddDbContext<EFContext>(options => options.UseSqlServer(_dbContainer.GetConnectionString()));
            });

            builder.UseStartup<TestStartup>()
                .UseSolutionRelativeContentRoot("tests")
                .UseTestServer();
        }

        #endregion

        #region IAsyncLifetime Members

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }

        #endregion

        #region Private Methods

        private async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = Server.Host.Services.GetService<IServiceScopeFactory>().CreateScope();
            await action(scope.ServiceProvider);
        }

        #endregion
    }

    [CollectionDefinition(nameof(ApiCollection))]
    public class ApiCollection : ICollectionFixture<TestHostFixture>
    { }
}
