namespace HelpDesk.Core.Domain.Abstractions
{
    public interface IPasswordHashChecker
    {
        bool HashesMatch(string passwordHash, string providedPassword);
    }
}
