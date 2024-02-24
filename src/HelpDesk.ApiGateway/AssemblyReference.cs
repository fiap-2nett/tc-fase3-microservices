using System.Reflection;

namespace HelpDesk.ApiGateway
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
