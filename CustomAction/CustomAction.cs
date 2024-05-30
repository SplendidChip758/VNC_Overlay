using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WixToolset.Dtf.WindowsInstaller;

namespace CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult LaunchApplication(Session session)
        {
            session.Log("Begin Launch Application");
            //Debugger.Launch();

            try
            {
                string installDir = session.CustomActionData["INSTALLFOLDER"];
                string exePath = System.IO.Path.Combine(installDir, "VNCOverlay.exe");

                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C start /D \"{installDir}\" VNCOverlay.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    process.Close(); // Ensure the process is detached
                }

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("Error in LaunchApplication: " + ex.Message);
                return ActionResult.Failure;
            } 
        }
    }
}
