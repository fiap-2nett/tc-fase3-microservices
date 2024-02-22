using HelpDesk.Core.Domain.ValueObjects;

namespace HelpDesk.Core.Domain.Cryptography
{
    public interface IPasswordHasher
    {
        string HashPassword(Password password);
    }
}
