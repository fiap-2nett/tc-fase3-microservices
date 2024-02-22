using System.Reflection;

namespace HelpDesk.ConsumerService.Api
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
