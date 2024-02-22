using System.Reflection;

namespace HelpDesk.ProducerService.Persistence
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
