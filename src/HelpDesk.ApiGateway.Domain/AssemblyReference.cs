using System.Reflection;

namespace HelpDesk.ApiGateway.Domain
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
