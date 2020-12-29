using System.Diagnostics;
using Utilities.Loggers;

namespace Utilities.SystemUtil {
    public static class Shell {
        /// <summary>
        /// Execute a command using system shell, waiting for finish.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="waitforExec">if set to <c>true</c> [waitfor exec].</param>
        public static void ExecuteCommand(string command, bool waitforExec = true) {
            Logger.SUCCESS("Running  Command " + command);
            //var exec = new ProcessStartInfo("cmd.exe /c \"" + command +" \"");
            // var exec = new ProcessStartInfo(command );
            var exec = new ProcessStartInfo();
            exec.FileName = "cmd.exe";
            exec.Arguments = "/c \"" + command + " \"";
            //exec.RedirectStandardError = true;
            //exec.RedirectStandardInput = true;
            exec.UseShellExecute = true;           
            var proc = Process.Start(exec);
            if (waitforExec) {
                proc.WaitForExit();
            }
            Logger.SUCCESS("Finished Command " + command);
        }
    }
}
