using System.Reflection;

namespace HelpDesk.ConsumerService.Application
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
