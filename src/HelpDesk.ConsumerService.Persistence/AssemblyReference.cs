using System.Reflection;

namespace HelpDesk.ConsumerService.Persistence
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
