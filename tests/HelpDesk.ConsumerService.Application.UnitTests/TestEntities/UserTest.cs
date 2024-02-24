using HelpDesk.Core.Domain.Entities;
using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.ValueObjects;

namespace HelpDesk.ConsumerService.Application.UnitTests.TestEntities
{
    internal class UserTest : User
    {
        public UserTest(int idUser, string name, string surname, Email email, UserRoles userRole, string passwordHash)
            : base(name, surname, email, userRole, passwordHash)
        {
            Id = idUser;
        }
    }
}
