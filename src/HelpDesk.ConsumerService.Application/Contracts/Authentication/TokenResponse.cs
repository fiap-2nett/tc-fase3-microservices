namespace HelpDesk.ConsumerService.Application.Contracts.Authentication
{
    public sealed class TokenResponse
    {
        public string Token { get; }

        public TokenResponse(string token) => Token = token;
    }
}
