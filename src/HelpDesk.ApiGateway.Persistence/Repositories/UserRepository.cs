using System.Threading.Tasks;
using HelpDesk.ApiGateway.Application.Core.Abstractions.Data;
using HelpDesk.ApiGateway.Domain.Repositories;
using HelpDesk.ApiGateway.Persistence.Core.Primitives;
using HelpDesk.Core.Domain.Entities;
using HelpDesk.Core.Domain.ValueObjects;

namespace HelpDesk.ApiGateway.Persistence.Repositories
{
    internal sealed class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        #region Constructors

        public UserRepository(IDbContext dbContext)
            : base(dbContext)
        { }

        #endregion

        #region IUserRepository Members

        public async Task<User> GetByEmailAsync(Email email)
            => await FirstOrDefaultAsync(user => user.Email.Value == email);

        public async Task<bool> IsEmailUniqueAsync(Email email)
            => !await AnyAsync(user => user.Email.Value == email);

        #endregion
    }
}
