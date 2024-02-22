using HelpDesk.ConsumerService.Application.Core.Abstractions.Data;
using HelpDesk.ConsumerService.Domain.Entities;
using HelpDesk.ConsumerService.Domain.Repositories;
using HelpDesk.ConsumerService.Persistence.Core.Primitives;

namespace HelpDesk.ConsumerService.Persistence.Repositories
{
    internal sealed class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        #region Constructors

        public CategoryRepository(IDbContext dbContext)
            : base(dbContext)
        { }

        #endregion
    }
}
