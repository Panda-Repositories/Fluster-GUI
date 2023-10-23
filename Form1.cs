using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading;
using System.Net;

namespace Fluster_GUI
{
    public partial class Form1 : Form
    {

        /*
         * -----------------------------------------------------------------------------------------
         * Fluster Graphical User Interface
         * Contributed by SkieHacker ( GUI / C# ) - https://discord.gg/Gpum3q9kSZ
         * 
         * Inspired from ROBLOX Official Installer lmfao
         * 
         * -----------------------------------------------------------------------------------------
         * C++ Installer made by Nano and edited by Pixeluted - discord.gg/runesoftware
         * Big thanks to cereal for making uwp update bypasser!
         */

        private static string url = "https://github.com/cerealwithmilk/uwp/releases/download/test/Fluster.msix";
        private static string fileName = "Windows10Universal.exe";
        private static string destinationDir = "C:\\Fluster";

        ErrorBox errorBox = new ErrorBox(); 
        public Form1()
        {
            InitializeComponent();
        }

        #region WebClient_Download
        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        bool DownloadComplete = false;
        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)            
                errorBox.Error(e.Error.Message);            
            else            
                DownloadComplete = true;            
        }

        #endregion
        public event EventHandler<bool> DownloadCompleted;
        private async void Install_Fluster()
        {
            msgbox.Text = "Enabling Developer Mode";
            await Task.Delay(1000);
            if (Utilities.ToggleDeveloperMode(true) == false)
            {
                errorBox.Error("Failed to Enable Developer Mode");
                Environment.Exit(0);
            }
            msgbox.Text = "Downloading Fluster ...";
            await Task.Delay(1000);
            if (!Utilities.RemoveOfficialROBLOX() )
            {
                errorBox.Warning("Failed to remove existing fluster application, Make sure Fluster isn't running / closed then try again.");
                Environment.Exit(0);
            }
            progressBar1.Style = ProgressBarStyle.Blocks;
           
            if (Directory.Exists(destinationDir))            
                Directory.Delete(destinationDir, true);   
            Directory.CreateDirectory(destinationDir);

            // ----------------------------------------
            // Download Fluster
            // ----------------------------------------
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += (sender, e) =>
                {
                    progressBar1.Value = e.ProgressPercentage;
                };
                webClient.DownloadFileCompleted += (sender, e) => DownloadCompleted?.Invoke(this, e.Error == null);

                // Start the download synchronously using a while loop
                while (true)
                {
                    try
                    {
                        webClient.DownloadFile(new Uri(url), destinationDir + "Fluster.zip");
                        DownloadCompleted?.Invoke(this, true);
                        break; // Exit the loop on successful download
                    }
                    catch (Exception)
                    {
                        // Handle download failures here (retry logic can be added if necessary)
                        DownloadCompleted?.Invoke(this, false);
                        break;
                    }
                }
            }
            // ----------------------------------------
            // Extraint Fluster
            // ----------------------------------------
            progressBar1.Value = 0;
            msgbox.Text = "Extracting Fluster ...";
            await Task.Delay(1000);
            if (!Utilities.ExtractZipFile(destinationDir + "Fluster.zip", destinationDir, progressBar1))
            {
                errorBox.Error("Failed to Extract Fluster, Please Try Again.");
                Environment.Exit(0);
            }
            // ----------------------------------------
            // Installing Fluster
            // ----------------------------------------
            progressBar1.Value = 0;
            msgbox.Text = "Installing Fluster ...";
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
            if (!Utilities.AddAppxPackage(destinationDir + "\\AppxManifest.xml"))
            {
                errorBox.Error("Failed to install Fluster, Something goes Kaboom!");
                Environment.Exit(0);
            }
            if (!Utilities.ToggleDeveloperMode(false))
            {
                errorBox.Warning("Failed to Disable Developer Mode. The Installation is Success but the Developer Mode will stay Open leaving your device vulnerable to side-loading attack.");
                Environment.Exit(0);
            }


        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            msgbox.Text = "Please Wait ...";
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
            await Task.Delay(3000);
            if (!Utilities.IsRunAsAdministrator())
            {
                errorBox.Warning("Please run this installer as administrator!");
                Environment.Exit(0);
            }
            if (Directory.Exists(destinationDir))
            {
                if (MessageBox.Show("Fluster is already installed. Reinstall?", errorBox.application_name, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) 
                    Install_Fluster();
                else 
                    Environment.Exit(0);
            }
            else
            {
                Install_Fluster();
            }
        }
    }
}
