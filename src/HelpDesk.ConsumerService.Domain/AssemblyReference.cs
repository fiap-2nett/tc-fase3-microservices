using System.Reflection;

namespace HelpDesk.ConsumerService.Domain
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
