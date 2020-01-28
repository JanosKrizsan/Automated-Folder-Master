using System;
using System.Diagnostics;

namespace Master_Console.Services
{
    public static class ExecutionManagementService
    {
        public static void ManageProgramRoute(Process process)
        {
            var parent = ParentProcessGetterService.GetParentProcess(process);
            var startupTimelapse = DateTime.Now.Subtract(parent.StartTime);
            
            //TODO take out vs code starter condition on release

            if ((parent.ProcessName.Equals("explorer") && startupTimelapse.TotalMinutes >= 2))
            {
                RunCleanUp.ManualSetup();
            }
            else if (parent.ProcessName.Equals("VsDebugConsole"))
            {
                RunCleanUp.ManualSetup();
            }
            else
            {
                RunCleanUp.AutomaticSetup();
            }

            //RunCleanUp.DoCleanup();
        }
    }
}
