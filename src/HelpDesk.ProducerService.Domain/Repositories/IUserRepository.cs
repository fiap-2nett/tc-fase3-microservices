using System.Threading.Tasks;
using HelpDesk.ProducerService.Domain.Dtos;

namespace HelpDesk.ProducerService.Domain.Repositories
{
    public interface IUserRepository
    {
        #region IUserRepository Members

        Task<UserDto> GetByIdAsync(int idUser);

        #endregion
    }
}
