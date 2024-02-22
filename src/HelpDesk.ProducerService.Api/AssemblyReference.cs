using System.Reflection;

namespace HelpDesk.ProducerService.Api
{
    public static class AssemblyReference
    {
        public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}
