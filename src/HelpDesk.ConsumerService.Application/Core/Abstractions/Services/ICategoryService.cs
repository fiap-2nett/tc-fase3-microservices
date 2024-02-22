using System.Threading.Tasks;
using System.Collections.Generic;
using HelpDesk.ConsumerService.Application.Contracts.Category;

namespace HelpDesk.ConsumerService.Application.Core.Abstractions.Services
{
    public interface ICategoryService
    {
        #region ICategoryService Members

        Task<IEnumerable<CategoryResponse>> GetAsync();
        Task<DetailedCategoryResponse> GetByIdAsync(int idCategory);

        #endregion
    }
}
