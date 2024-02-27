using HelpDesk.Core.Domain.Enumerations;

namespace HelpDesk.ProducerService.Domain.Dtos
{
    public sealed record UserDto(int IdUser, UserRoles UserRole);
}
