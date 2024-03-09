using Guna.UI2.WinForms;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Configurator.Classes;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Management;
using System;
using System.Linq;

namespace SapphireTool
{
    public partial class Main : Form
    {

        public static RegistryKey SapphireTool = Registry.CurrentUser.CreateSubKey(@"Software\SapphireTool", RegistryKeyPermissionCheck.ReadWriteSubTree);
        private string DownloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";

        //Registry Keys

        string dwmRegistryPath = @"SOFTWARE\Policies\Microsoft\Windows\DWM";
        string windowMetricsRegistryPath = @"Control Panel\Desktop\WindowMetrics";
        string explorerAdvancedRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
        string visualEffectsRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects";
        string desktopRegistryPath = @"Control Panel\Desktop";
        string gameBarRegistryPath = @"Software\Microsoft\GameBar";
        string gameConfigStoreRegistryPath = @"System\GameConfigStore";
        string gameDVRRegistryPath = @"SOFTWARE\Policies\Microsoft\Windows\GameDVR";
        string currentVersionGameDVRRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR";
        string bcastDVRUserServiceRegistryPath = @"SYSTEM\CurrentControlSet\Services\BcastDVRUserService";
        string sessionManagerEnvironmentRegistryPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

        public Main()
        {
            InitializeComponent();
            ReadSettings();
            GetOSInfo();
        }

        //Read Settings
        private void ReadSettings()
        {
            RegistryKey key = SapphireTool;

            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisableBluetooth", tsDisableBluetooth);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisableFSO", tsDisableFSO);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisablePrefetch", tsDisablePrefetch);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisableHyperV", tsDisableHyperV);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisablePrinter", tsDisablePrintSpooler);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisableVPN", tsDisableVPN);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisableWiFi", tsDisableWiFI);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "EnableHAGS", tsEnableHAGS);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisableMPO", tsDisableMPO);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisablePreemption", tsDisablePreemption);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "EnableClipboardSvc", tsEnableClipboardSvc);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "RevertNetworkTweaks", tsRevertNetworkTweaks);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "DisableHDCP", tsDisableHDCP);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "SvcHost", tsSvcHost);
            Utils.CheckRegistryValueAndSetToggleSwitch(key, "EnableNX", tsEnableNX);

            OSName.Text = Utils.GetOS();
            OSArch.Text = Utils.GetBitness();
            Welcome.Text = "Welcome " + Environment.UserName;

        }

        private void GetOSInfo()
        {
            if (SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery)
            {
                Platform.Text = ($"Desktop ({Environment.MachineName})");
            }
            else
            {
                Platform.Text = ($"Laptop ({Environment.MachineName})");
            }
        }

        //please wait
        //Workstation Services
        private void WorkstationEnable()
        {
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\rdbss", "Start", 2, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\KSecPkg", "Start", 2, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\mrxsmb20", "Start", 2, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\mrxsmb", "Start", 2, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\srv2", "Start", 2, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\LanmanWorkstation", "Start", 2, RegistryValueKind.DWord);
            Utils.RunCommand("DISM", "/Online /Enable-Feature /FeatureName:SmbDirect /NoRestart");
        }
        private void WorkstationDiasble()
        {
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\rdbss", "Start", 4, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\KSecPkg", "Start", 4, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\mrxsmb20", "Start", 4, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\mrxsmb", "Start", 4, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\srv2", "Start", 4, RegistryValueKind.DWord);
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\LanmanWorkstation", "Start", 4, RegistryValueKind.DWord);
            Utils.RunCommand("DISM", "/Online /Disable-Feature /FeatureName:SmbDirect /NoRestart");
        }

        //Checkboxes

        private void tsDisableBluetooth_CheckedChanged(object sender, EventArgs e)
        {
            if (tsDisableBluetooth.Checked)
            {
                object aVal = SapphireTool.GetValue("DisableBluetooth");
                if (null != aVal)
                {
                }
                else
                {
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\BthAvctpSvc", "Start", 4, RegistryValueKind.DWord);
                    SapphireTool.SetValue("DisableBluetooth", 1);
                }
            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\BthAvctpSvc", "Start", 3, RegistryValueKind.DWord);
                SapphireTool.DeleteValue("DisableBluetooth");
            }
        }

        private void tsDisableFSO_CheckedChanged(object sender, EventArgs e)
        {
            if (tsDisableFSO.Checked)
            {
                object aVal = SapphireTool.GetValue("DisableFSO");
                if (null != aVal)
                {
                }
                else
                {
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameBarRegistryPath, "ShowStartupPanel", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameBarRegistryPath, "GamePanelStartupTipIndex", 3, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameBarRegistryPath, "AllowAutoGameMode", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameBarRegistryPath, "AutoGameModeEnabled", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameBarRegistryPath, "UseNexusForGameBarEnabled", 0, RegistryValueKind.DWord);

                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameConfigStoreRegistryPath, "GameDVR_Enabled", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameConfigStoreRegistryPath, "GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameConfigStoreRegistryPath, "GameDVR_FSEBehavior", 2, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameConfigStoreRegistryPath, "GameDVR_HonorUserFSEBehaviorMode", 1, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameConfigStoreRegistryPath, "GameDVR_DXGIHonorFSEWindowsCompatible", 1, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameConfigStoreRegistryPath, "GameDVR_EFSEFeatureFlags", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_CURRENT_USER\" + gameConfigStoreRegistryPath, "GameDVR_DSEBehavior", 2, RegistryValueKind.DWord);

                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + gameDVRRegistryPath, "AllowGameDVR", 0, RegistryValueKind.DWord);

                    Registry.SetValue(@"HKEY_CURRENT_USER\" + currentVersionGameDVRRegistryPath, "AppCaptureEnabled", 0, RegistryValueKind.DWord);

                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + bcastDVRUserServiceRegistryPath, "Start", 4, RegistryValueKind.DWord);

                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + sessionManagerEnvironmentRegistryPath, "__COMPAT_LAYER", "~ DISABLEDXMAXIMIZEDWINDOWEDMODE", RegistryValueKind.String);

                    SapphireTool.SetValue("DisableFSO", 1);
                }
            }
            else
            {
                RegistryKey currentUserKey = Registry.CurrentUser;

                currentUserKey.OpenSubKey(gameBarRegistryPath, true)?.DeleteValue("GamePanelStartupTipIndex", false);
                currentUserKey.OpenSubKey(gameBarRegistryPath, true)?.DeleteValue("AllowAutoGameMode", false);
                currentUserKey.OpenSubKey(gameBarRegistryPath, true)?.DeleteValue("AutoGameModeEnabled", false);
                currentUserKey.OpenSubKey(gameBarRegistryPath, true)?.SetValue("UseNexusForGameBarEnabled", 1, RegistryValueKind.DWord);
                currentUserKey.OpenSubKey(gameBarRegistryPath, true)?.SetValue("ShowStartupPanel", 1, RegistryValueKind.DWord);

                currentUserKey.OpenSubKey(gameConfigStoreRegistryPath, true)?.SetValue("GameDVR_Enabled", 1, RegistryValueKind.DWord);
                currentUserKey.OpenSubKey(gameConfigStoreRegistryPath, true)?.SetValue("GameDVR_FSEBehavior", 0, RegistryValueKind.DWord);
                currentUserKey.OpenSubKey(gameConfigStoreRegistryPath, true)?.SetValue("GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);
                currentUserKey.OpenSubKey(gameConfigStoreRegistryPath, true)?.SetValue("GameDVR_HonorUserFSEBehaviorMode", 0, RegistryValueKind.DWord);
                currentUserKey.OpenSubKey(gameConfigStoreRegistryPath, true)?.SetValue("GameDVR_DXGIHonorFSEWindowsCompatible", 0, RegistryValueKind.DWord);
                currentUserKey.OpenSubKey(gameConfigStoreRegistryPath, true)?.SetValue("GameDVR_EFSEFeatureFlags", 1, RegistryValueKind.DWord);
                currentUserKey.OpenSubKey(gameConfigStoreRegistryPath, true)?.DeleteValue("GameDVR_DSEBehavior", false);

                Registry.LocalMachine.OpenSubKey(gameDVRRegistryPath, true)?.DeleteValue("AllowGameDVR", false);

                currentUserKey.OpenSubKey(currentVersionGameDVRRegistryPath, true)?.SetValue("AppCaptureEnabled", 1, RegistryValueKind.DWord);

                Registry.LocalMachine.OpenSubKey(bcastDVRUserServiceRegistryPath, true)?.SetValue("Start", 3, RegistryValueKind.DWord);

                Registry.LocalMachine.OpenSubKey(sessionManagerEnvironmentRegistryPath, true)?.DeleteValue("__COMPAT_LAYER", false);

                SapphireTool.DeleteValue("DisableFSO");
            }
        }

        private void tsDisablePrefetch_CheckedChanged(object sender, EventArgs e)
        {
            if (tsDisablePrefetch.Checked)
            {
                object aVal = SapphireTool.GetValue("DisablePrefetch");
                if (null != aVal)
                {
                }
                else
                {
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\SysMain", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\FontCache", "Start", 4, RegistryValueKind.DWord);

                    SapphireTool.SetValue("DisablePrefetch", 1);
                }
            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\SysMain", "Start", 2, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\FontCache", "Start", 2, RegistryValueKind.DWord);

                SapphireTool.DeleteValue("DisablePrefetch");
            }
        }

        private void tsDisableHyperV_CheckedChanged(object sender, EventArgs e)
        {
            if (tsDisableHyperV.Checked)
            {
                object aVal = SapphireTool.GetValue("DisableHyperV");
                if (null != aVal)
                {
                }
                else
                {
                    Utils.RunCommand("bcdedit", "/set hypervisorlaunchtype off");
                    Utils.RunCommand("bcdedit", "/set vm no");
                    Utils.RunCommand("bcdedit", "/set vsmlaunchtype Off");
                    Utils.RunCommand("bcdedit", "/set loadoptions DISABLE-LSA-ISO,DISABLE-VBS");

                    Utils.RunCommand("DISM", "/Online /Disable-Feature:Microsoft-Hyper-V-All /Quiet /NoRestart");

                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", "EnableVirtualizationBasedSecurity", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", "RequirePlatformSecurityFeatures", 1, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", "HypervisorEnforcedCodeIntegrity", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", "HVCIMATRequired", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", "LsaCfgFlags", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", "ConfigureSystemGuardLaunch", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard", "RequireMicrosoftSignedBootChain", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "WasEnabledBy", 0, RegistryValueKind.DWord);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 0, RegistryValueKind.DWord);

                    SapphireTool.SetValue("DisableHyperV", 1);
                }
            }
            else
            {
                Utils.RunCommand("bcdedit", "/set hypervisorlaunchtype auto");
                Utils.RunCommand("bcdedit", "/deletevalue vm");
                Utils.RunCommand("bcdedit", "/deletevalue loadoptions");

                // Enable Hyper-V with DISM
                Utils.RunCommand("DISM", "/Online /Enable-Feature:Microsoft-Hyper-V-All /Quiet /NoRestart");

                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", true)?.DeleteValue("EnableVirtualizationBasedSecurity", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", true)?.DeleteValue("RequirePlatformSecurityFeatures", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", true)?.DeleteValue("HypervisorEnforcedCodeIntegrity", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", true)?.DeleteValue("HVCIMATRequired", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", true)?.DeleteValue("LsaCfgFlags", false);
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DeviceGuard", true)?.DeleteValue("ConfigureSystemGuardLaunch", false);

                // Set registry values
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard", "RequireMicrosoftSignedBootChain", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard", "EnableVirtualizationBasedSecurity", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard", "RequirePlatformSecurityFeatures", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard", "Locked", 0, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Locked", 0, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "WasEnabledBy", 1, RegistryValueKind.DWord);

                SapphireTool.DeleteValue("DisableHyperV");
            }
        }
        private void tsDisablePrintSpooler_CheckedChanged(object sender, EventArgs e)
        {
            if (tsDisablePrintSpooler.Checked)
            {
                object aVal = SapphireTool.GetValue("DisablePrinter");
                if (null != aVal)
                {
                }
                else
                {
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Spooler", "Start", 4, RegistryValueKind.DWord);
                    SapphireTool.SetValue("DisablePrinter", 1);
                }
            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Spooler", "Start", 2, RegistryValueKind.DWord);
                SapphireTool.DeleteValue("DisablePrinter");
            }
        }

        private void tsDisableVPN_CheckedChanged(object sender, EventArgs e)
        {
            if (tsDisableVPN.Checked)
            {
                object aVal = SapphireTool.GetValue("DisableVPN");
                if (null != aVal)
                {
                }
                else
                {
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\IKEEXT", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\WinHttpAutoProxySvc", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\RasMan", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\SstpSvc", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\iphlpsvc", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\NdisVirtualBus", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Eaphost", "Start", 4, RegistryValueKind.DWord);
                    SapphireTool.SetValue("DisableVPN", 1);
                }

            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\IKEEXT", "Start", 3, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\BFE", "Start", 2, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\WinHttpAutoProxySvc", "Start", 3, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\RasMan", "Start", 3, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\SstpSvc", "Start", 3, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\iphlpsvc", "Start", 3, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\NdisVirtualBus", "Start", 3, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Eaphost", "Start", 3, RegistryValueKind.DWord);
                SapphireTool.DeleteValue("DisableVPN");
            }
        }

        private void tsDisableWiFI_CheckedChanged(object sender, EventArgs e)
        {
            if (tsDisableWiFI.Checked)
            {
                object aVal = SapphireTool.GetValue("DisableWiFi");
                if (null != aVal)
                {
                }
                else
                {
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\WlanSvc", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\vwififlt", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\netprofm", "Start", 4, RegistryValueKind.DWord);
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\NlaSvc  ", "Start", 4, RegistryValueKind.DWord);
                    if (OSName.Text.Contains("Windows 11"))
                    {
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\netprofm", "Start", 3, RegistryValueKind.DWord);
                    }
                    SapphireTool.SetValue("DisableWiFi", 1);
                }
            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\WlanSvc", "Start", 2, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\vwififlt", "Start", 1, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\netprofm", "Start", 3, RegistryValueKind.DWord);
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\NlaSvc  ", "Start", 2, RegistryValueKind.DWord);
                SapphireTool.DeleteValue("DisableWiFi");
            }
        }

        private void tsEnableHAGS_CheckedChanged(object sender, EventArgs e)
        {
            if (tsEnableHAGS.Checked)
            {
                object aVal = SapphireTool.GetValue("EnableHAGS");
                if (null != aVal)
                {
                }
                else
                {
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers", "HwSchMode", 1, RegistryValueKind.DWord);
                    SapphireTool.SetValue("EnableHAGS", 1);
                }
            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers", "HwSchMode", 2, RegistryValueKind.DWord);
                SapphireTool.DeleteValue("EnableHAGS");
            }
        }
        private void btnBrave_Click(object sender, EventArgs e)
        {
            if (Utils.DownloadFile(
"https://brave-browser-downloads.s3.brave.com/latest/BraveBrowserSetup.exe",
DownloadsFolder + "\\BraveBrowserSetup.exe"
) == true)
            {
                Process.Start(DownloadsFolder + "\\BraveBrowserSetup.exe");
            }
        }

        private void btnChrome_Click(object sender, EventArgs e)
        {
            if (Utils.DownloadFile(
"http://dl.google.com/chrome/install/chrome_installer.exe",
DownloadsFolder + "\\chrome_installer.exe"
) == true)
            {
                Process.Start(DownloadsFolder + "\\chrome_installer.exe");
            }
        }

        private void btnEdge_Click(object sender, EventArgs e)
        {

            if (Utils.DownloadFile(
"https://cdn.cloudflare.steamstatic.com/client/installer/SteamSetup.exe",
DownloadsFolder + "\\SteamSetup.exe"
) == true)
            {

                Process.Start(DownloadsFolder + "\\SteamSetup.exe");
            }
        }

        private void btnFirefox_Click(object sender, EventArgs e)
        {

            if (Utils.DownloadFile(
"https://download.mozilla.org/?product=firefox-stub&os=win&lang=en-US",
DownloadsFolder + "\\MozillaFirefoxSetup.exe"
) == true)
            {

                Process.Start(DownloadsFolder + "\\MozillaFirefoxSetup.exe");
            }
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (Utils.DownloadFile(
"https://net.geo.opera.com/opera/stable/windows?utm_tryagain=yes&utm_source=google&utm_medium=ose&utm_campaign=(none)&http_referrer=https%3A%2F%2Fwww.google.com%2F&utm_site=opera_com&&utm_lastpage=opera.com/",
DownloadsFolder + "\\OperaSetup.exe"
) == true)
            {
                Process.Start(DownloadsFolder + "\\OperaSetup.exe");
            }
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {

            if (Utils.DownloadFile(
"https://github.com/Vencord/Installer/releases/latest/download/VencordInstaller.exe",
DownloadsFolder + "\\Vencord.exe"
) == true)
            {

                Process.Start(DownloadsFolder + "\\Vencord.exe");
            }
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }
        private void btnOperaGX_Click(object sender, EventArgs e)
        {

            if (Utils.DownloadFile(
"https://cdn.discordapp.com/attachments/905447438238773259/934389036800409660/OperaGXSetup.exe",
DownloadsFolder + "\\OperaGXSetup.exe"
) == true)
            {

                Process.Start(DownloadsFolder + "\\OperaGXSetup.exe");
            }
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            if (Utils.DownloadFile(
"https://discord.com/api/downloads/distributions/app/installers/latest?channel=stable&platform=win&arch=x86",
DownloadsFolder + "\\DiscordSetup.exe"
) == true)
            {

                Process.Start(DownloadsFolder + "\\DiscordSetup.exe");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/TheWorldOfPC/Configurator");
        }

        private void label10_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/TheWorldOfPC/Configurator");
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void tsDisablePreemption_CheckedChanged(object sender, EventArgs e)
        {
            if (tsDisablePreemption.Checked)
            {
                object aVal = SapphireTool.GetValue("DisablePreemption");
                if (null != aVal)
                {
                }
                else
                {
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers\\Scheduler", "EnablePreemption", 0, RegistryValueKind.DWord);
                    SapphireTool.SetValue("DisablePreemption", 1);
                }
            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers\\Scheduler", "EnablePreemption", 1, RegistryValueKind.DWord);
                SapphireTool.DeleteValue("DisablePreemption");
            }
        }

        private void tsDisableMPO_CheckedChanged(object sender, EventArgs e)
        {
            if (tsDisableMPO.Checked)
            {
                object aVal = SapphireTool.GetValue("DisableMPO");
                if (null != aVal)
                {
                }
                else
                {
                    Process.Start("C:\\PostInstall\\GPU\\Nvidia\\mpo disable.bat");
                    SapphireTool.SetValue("DisableMPO", 1);
                }
            }
            else
            {
                Process.Start("C:\\PostInstall\\GPU\\Nvidia\\mpo enable.bat");
                SapphireTool.DeleteValue("DisableMPO");
            }
        }
        private void tsEnableClipboardSvc_CheckedChanged(object sender, EventArgs e)
        {
            if (tsEnableClipboardSvc.Checked)
            {
                object aVal = SapphireTool.GetValue("EnableClipboardSvc");
                if (null != aVal)
                {
                }
                else
                {
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\cbdhsvc", "Start", 2, RegistryValueKind.DWord);
                SapphireTool.SetValue("EnableClipboardSvc", 1);
                }
            }
            else
            {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\cbdhsvc", "Start", 4, RegistryValueKind.DWord);
                SapphireTool.DeleteValue("EnableClipboardSvc");
            }
        }
        private void tsRevertNetworkTweaks_CheckedChanged(object sender, EventArgs e)
        {
            if (tsRevertNetworkTweaks.Checked)
            {
                object aVal = SapphireTool.GetValue("RevertNetworkTweaks");
                if (null != aVal)
                {
                }
                else
                {
                    Process.Start("C:\\PostInstall\\Others\\Network\\Revert Network Tweaks.bat");
                    SapphireTool.SetValue("RevertNetworkTweaks", 1);
                }
            }
            else
            {
                Process.Start("C:\\PostInstall\\Others\\Network\\SapphireOS Default Network Settings");
                SapphireTool.DeleteValue("RevertNetworkTweaks");
            }
        }
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Process.Start("powershell.exe", $"-Command \"iwr 'https://raw.githubusercontent.com/Vencord/Installer/main/install.ps1' -UseBasicParsing | iex\"");
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            if (Utils.DownloadFile(
"https://go.microsoft.com/fwlink/?linkid=2108834&Channel=Stable&language=en&brand=M100",
DownloadsFolder + "\\MicrosoftEdgeSetup.exe"
) == true)
            {

                Process.Start(DownloadsFolder + "\\MicrosoftEdgeSetup.exe");
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (Utils.DownloadFile(
"https://app.prntscr.com/build/setup-lightshot.exe",
DownloadsFolder + "\\setup-lightshot.exe"
) == true)
            {

                Process.Start(DownloadsFolder + "\\setup-lightshot.exe");
            }
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (Utils.DownloadFile(
"https://go.microsoft.com/fwlink/?linkid=2124701",
DownloadsFolder + "\\MicrosoftEdgeWebView2RuntimeInstallerX64.exe"
) == true)
            {

                Process.Start(DownloadsFolder + "\\MicrosoftEdgeWebView2RuntimeInstallerX64.exe");
            }
        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\GPU\\AMD\\AMD Dwords by imribiy.bat");
        }

        private void tsDisableHDCP_CheckedChanged(object sender, EventArgs e)
            {
                if (tsDisableHDCP.Checked)
                {
                    object aVal = SapphireTool.GetValue("DisableHDCP");
                    if (null != aVal)
                    {
                    }
                    else
                    {
                        Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e968-e325-11ce-bfc1-08002be10318}\\0000", "RMHdcpKeyglobZero", 1, RegistryValueKind.DWord);
                        SapphireTool.SetValue("DisableHDCP", 1);
                    }
                }
                else
                {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e968-e325-11ce-bfc1-08002be10318}\\0000", "RMHdcpKeyglobZero", 0, RegistryValueKind.DWord);
                SapphireTool.DeleteValue("DisableHDCP");
                }
            }

        private void tsSvcHost_CheckedChanged(object sender, EventArgs e)
        {
            if (tsSvcHost.Checked)
            {
                object aVal = SapphireTool.GetValue("SvcHost");
                if (null != aVal)
                {
                }
                else
                {
                    Process.Start("C:\\PostInstall\\Others\\SvcHostSplitThresholdInKB\\Set SvcHostSplitThresholdInKB To ffffffff.bat");
                    SapphireTool.SetValue("SvcHost", 1);
                }
            }
            else
            {
                Process.Start("C:\\PostInstall\\Others\\SvcHostSplitThresholdInKB\\Set SvcHostSplitThresholdInKB To Default Value.bat");
                SapphireTool.DeleteValue("SvcHost");
            }
        }

        private void tsEnableNX_CheckedChanged(object sender, EventArgs e)
        {
            if (tsEnableNX.Checked)
            {
                object aVal = SapphireTool.GetValue("SvcHost");
                if (null != aVal)
                {
                }
                else
                {
                    Utils.RunCommand("bcdedit", "/set NX OptIn");
                    SapphireTool.SetValue("EnableNX", 1);
                }
            }
            else
            {
                Utils.RunCommand("bcdedit", "/set NX AlwaysOff");
                SapphireTool.DeleteValue("EnableNX");
            }
        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\GPU\\AMD\\radeon software slimmer\\RadeonSoftwareSlimmer.exe");
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            Utils.RunCommand("C:\\PostInstall\\GPU\\Nvidia\\NIP\\nvidiaProfileInspector.exe", "/s C:\\PostInstall\\GPU\\Nvidia\\NIP\\Settings.nip");
        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Mitigations\\InSpectre.exe");
        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\GPU\\Nvidia\\!P-State 0.bat");
        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\DevManView.exe");
        }

        private void guna2Button14_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\NSudo.exe");
        }

        private void guna2Button15_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\serviwin.exe");
        }

        private void guna2Button16_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\DeviceCleanup.exe");
        }

        private void guna2Button17_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\CRU\\CRU.exe");
        }

        private void guna2Button18_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\Auto DSCP & FSE.bat");
        }

        private void guna2Button19_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\Autoruns.exe");
        }

        private void guna2Button20_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\MeasureSleep.exe");
        }

        private void guna2Button21_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\MSI Mode Utility.exe");
        }

        private void guna2Button22_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\Tweaks\\Interrupt Affinity Policy Tool.exe");
        }

        private void guna2Button23_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\PostInstall\\GPU\\Nvidia\\NVCleanstall_1.16.0.exe");
        }
        private void label20_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/TheWorldOfPC/Configurator");
        }
    }
}
