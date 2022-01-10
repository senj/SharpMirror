using System;
using System.Diagnostics;

namespace SmartMirror.HostConnection
{
    public static class ScriptRunner
    {
        public static string RunScript()
        {
            ProcessStartInfo processInfo = new()
            {
                UseShellExecute = false,
                FileName = "sh",   // 'sh' for bash
                Arguments = "echo \"standby 0\" | sudo cec-client -s -d 1"    // The Script name 
            };

            try
            {
                var process = Process.Start(processInfo);   // Start that process.
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }
    }
}