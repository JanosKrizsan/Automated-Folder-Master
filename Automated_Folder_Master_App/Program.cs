using Master_Console.Services;
using System.Diagnostics;

namespace Master_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecutionManagementService.ManageProgramRoute(Process.GetCurrentProcess());
        }
    }
}
