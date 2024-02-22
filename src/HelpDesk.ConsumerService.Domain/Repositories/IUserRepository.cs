using System.Threading.Tasks;
using HelpDesk.Core.Domain.Entities;
using HelpDesk.Core.Domain.ValueObjects;

namespace HelpDesk.ConsumerService.Domain.Repositories
{
    public interface IUserRepository
    {
        #region IUserRepository Members

        Task<User> GetByIdAsync(int idUser);
        Task<User> GetByEmailAsync(Email email);
        Task<bool> IsEmailUniqueAsync(Email email);
        void Insert(User user);

        #endregion
    }
}
