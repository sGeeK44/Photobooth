using System.Diagnostics;
using System.Threading;

namespace CabineParty.Core
{
    public static class ProcessHelper
    {
        public static void KillAll(string processName)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                }
            }
            catch
            {
                // ignored
            }
        }

        public static Process Start(string execPath)
        {
            var result = Process.Start(execPath);
            Thread.Sleep(5000); // Ensure started
            return result;
        }
    }
}
