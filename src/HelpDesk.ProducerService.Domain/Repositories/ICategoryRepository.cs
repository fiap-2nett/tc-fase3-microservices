using System.Threading.Tasks;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Domain.Repositories
{
    public interface ICategoryRepository
    {
        #region ICategoryRepository Members

        Task<CategoryDto> GetByIdAsync(int idCategory);

        #endregion
    }
}
