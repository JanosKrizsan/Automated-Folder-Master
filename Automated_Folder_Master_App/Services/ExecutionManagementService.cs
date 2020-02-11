using System;
using System.Diagnostics;

namespace Master_Console.Services
{
    public static class ExecutionManagementService
    {
        private static RunCleanUp _cleanUp = new RunCleanUp();
        private static ParentProcessGetterService _parentGetter = new ParentProcessGetterService();
        public static void ManageProgramRoute(Process process)
        {
            var parent = _parentGetter.GetParentProcess(process);
            var startupTimelapse = DateTime.Now.Subtract(parent.StartTime);
            

            if ((parent.ProcessName.Equals("explorer") && startupTimelapse.TotalMinutes >= 2))
            {
                _cleanUp.ManualSetup();
            }
            else
            {
                _cleanUp.AutomaticSetup();
            }

            _cleanUp.DoCleanup();
        }
    }
}
