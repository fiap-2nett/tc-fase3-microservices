using System.Reflection;

namespace HelpDesk.ApiGateway.Application
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
