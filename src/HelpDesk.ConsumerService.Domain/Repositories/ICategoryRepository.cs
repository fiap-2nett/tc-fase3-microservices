using System.Threading.Tasks;
using HelpDesk.ConsumerService.Domain.Entities;

namespace HelpDesk.ConsumerService.Domain.Repositories
{
    public interface ICategoryRepository
    {
        #region ICategoryRepository Members

        Task<Category> GetByIdAsync(int idCategory);

        #endregion
    }
}
