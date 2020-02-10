using System.Diagnostics;
using System.Management;

namespace Master_Console.Services
{
    public class ParentProcessGetterService
    {
        public Process GetParentProcess(Process process)
        {
            var thisId = process.Id;
            var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", thisId);
            var search = new ManagementObjectSearcher("root\\CIMV2", query);
            var results = search.Get().GetEnumerator();
            results.MoveNext();

            var queryObject = results.Current;
            var parentId = (uint)queryObject["ParentProcessId"];

            search.Dispose();
            return Process.GetProcessById((int)parentId);
        }
    }
}
