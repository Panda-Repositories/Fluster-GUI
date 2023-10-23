using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using Microsoft.Win32;
using System.IO.Compression;
using System.Windows.Forms;


using System.Threading;
using System.Threading.Tasks;
namespace Fluster_GUI
{
    public static class Utilities
    {
        public static bool IsRunAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool RemoveOfficialROBLOX()
        {
            string command = "Get-AppxPackage ROBLOXCORPORATION.ROBLOX.* | Remove-AppPackage";
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command {command}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }


        public static event Action<int> ProgressChanged;

        public static bool ExtractZipFile(string zipFilePath, string extractPath, ProgressBar progressBar)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    int totalEntries = archive.Entries.Count;
                    int extractedEntries = 0;

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        while (true)
                        {
                            try
                            {
                                entry.ExtractToFile(Path.Combine(extractPath, entry.FullName), true);
                                extractedEntries++;
                                int progressPercentage = (int)((double)extractedEntries / totalEntries * 100);
                                progressBar?.Invoke((MethodInvoker)(() => progressBar.Value = progressPercentage));
                                break;
                            }
                            catch (IOException)
                            {
                                break;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false; 
            }
        }
        public static bool AddAppxPackage(string appxManifestPath)
        {
            string addAppxCommand = $"powershell -Command \"Add-AppxPackage -path '{appxManifestPath}' -register\"";
            int resultCode = 0;
            bool result = RunCommand(addAppxCommand, out resultCode);

            if (result && resultCode == 0)
            {
                while (!Directory.Exists("C:\\Fluster\\microsoft.system.package.metadata"))
                {
                    Thread.Sleep(1000);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool RunCommand(string command, out int exitCode)
        {
            using (Process process = new Process())
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process.StartInfo = psi;
                process.Start();

                using (StreamWriter sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine(command);
                        sw.WriteLine("exit");
                    }
                }

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                exitCode = process.ExitCode;

                return exitCode == 0;
            }
        }

        public static bool ToggleDeveloperMode(bool enable)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AppModelUnlock", true);
                if (key != null)
                {
                    // Set the AllowDevelopmentWithoutDevLicense registry value based on the 'enable' parameter
                    int valueToSet = enable ? 1 : 0;
                    key.SetValue("AllowDevelopmentWithoutDevLicense", valueToSet, RegistryValueKind.DWord);
                    key.Close();
                    MessageBox.Show(enable ? "Developer Mode enabled successfully." : "Developer Mode disabled successfully.");
                    return true;
                }
                else
                {
                    MessageBox.Show("Failed to access the registry key.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}

