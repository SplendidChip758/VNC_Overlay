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

            try
            {
                string installDir = session["INSTALLFOLDER"];
                string exePath = System.IO.Path.Combine(installDir, "VNCOverlay.exe");

                Process.Start(exePath);
            }
            catch (Exception ex)
            {
                session.Log("Error in LaunchApplication: " + ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }
    }
}
