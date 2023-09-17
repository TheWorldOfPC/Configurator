using Guna.UI2.WinForms;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Configurator.Classes
{
    internal class Utils
    {
        static string productName = string.Empty;
        static string buildNumber = string.Empty;
        internal enum WindowsVersion
        {
            Unsupported = 0,
            Windows7 = 7,
            Windows8 = 8,
            Windows10 = 10,
            Windows11 = 11
        }

        internal static WindowsVersion CurrentWindowsVersion = WindowsVersion.Unsupported;
        
        public static async Task<bool> DownloadFileAsync(string url, string filename)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    // Download the file asynchronously
                    await client.DownloadFileTaskAsync(new Uri(url), filename);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DownloadFile(string url, string filename)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(url), filename);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void CheckRegistryValueAndSetToggleSwitch(RegistryKey key, string valueName, Guna2ToggleSwitch toggleSwitch, ref bool toggleValue)
        {
            object val = key.GetValue(valueName);
            if (val != null)
            {
                toggleSwitch.Checked = true;
                toggleValue = false;
            }
        }

        //Run Command
        public static void RunCommand(string command, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            Process process = new Process { StartInfo = startInfo };

            process.Start();
            process.WaitForExit();
        }

        //get OS Name

        internal static string GetOS()
        {
            productName = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName", "");

            if (productName.Contains("Windows 7"))
            {
                CurrentWindowsVersion = WindowsVersion.Windows7;
            }
            if (productName.Contains("Windows 8") || productName.Contains("Windows 8.1"))
            {
                CurrentWindowsVersion = WindowsVersion.Windows8;
            }
            if (productName.Contains("Windows 10"))
            {
                buildNumber = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CurrentBuild", "");

                if (Convert.ToInt32(buildNumber) >= 22000)
                {
                    productName = productName.Replace("Windows 10", "Windows 11");
                    CurrentWindowsVersion = WindowsVersion.Windows11;
                }
                else
                {
                    CurrentWindowsVersion = WindowsVersion.Windows10;
                }
            }
            return productName;
        }

        internal static string GetBitness()
        {
            string bitness;

            if (Environment.Is64BitOperatingSystem)
            {
                bitness = "Operating System Architecture: 64-bit";
            }
            else
            {
                bitness = "Operating System Architecture: 32-bit";
            }

            return bitness;
        }
    }
}
