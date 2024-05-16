using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNCOverlay
{
    public static class Utilities
    {      
       

        public static string InitializeOverlay()
        {
            // Initialize the overlayU
            try
            {
                string overlay = System.Configuration.ConfigurationManager.AppSettings["Overlay"];
                overlay = overlay.ToLower();
                if (!string.IsNullOrWhiteSpace(overlay) && overlay == "basic" || overlay == "center" || overlay == "loud")
                {
                    return overlay;
                }
                else
                {
                    return "Basic";
                }
            }
            catch(Exception ex)
            {
                return "Basic";
            }
        }
    }
}
