/*
Copyright (c) Kurt Cancemi 2012-2014

This file is part of Wnmp.

    Wnmp is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Wnmp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Wnmp.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using Wnmp.Programs;
using Wnmp.Helpers;

using Wnmp.Internals;
using Wnmp.Forms;
namespace Wnmp
{
    /// <summary>
    /// Main form of Wnmp
    /// </summary>
    public partial class Main : Form
    {
        public static string StartupPath { get { return Application.StartupPath; } }

        private Version CPVER = new Version("2.2.2");
        public Version GetCPVER { get { return CPVER; } }

        internal const string UpdateXMLURL = "https://s3.amazonaws.com/wnmp/update.xml";

        private NotifyIcon WnmpTrayIcon = new NotifyIcon();

        public Main()
        {
            InitializeComponent();
            setevents();
            Options.settings.ReadSettings();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.Style = myCp.Style & ~Declarations.WS_THICKFRAME; // Remove WS_THICKFRAME (Disables resizing)
                return myCp;
            }
        }
		
		#region functions
		
        /// <summary>
        /// Checks if the size of Wnmp.log is greator than 1MB and 
        /// if it is it gzips it
        /// </summary>
        private void DoLogSizeCheck(string path)
        {
            try
            {
                var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                if (file.Length > 1048576) // 1048576 = 1MB
                {
                    Log.wnmp_log_notice("Wnmp.log is greator than 1MB compressing it to Wnmp.log.gz...", Log.LogSection.WNMP_MAIN);
                    CompressFile(path);
                    file.Dispose();
                    File.Delete(path);
                }
                else
                {
                    file.Dispose();
                }
            }
            catch { }
        }
        /// <summary>
        /// Saves the current Wnmp log to Wnmp.log
        /// </summary>
        private void DoSaveLog()
        {
            try
            {
                string path = Application.StartupPath + "/Wnmp.log";

                DoLogSizeCheck(path);

                StreamWriter sw = new StreamWriter(path, true);
                sw.WriteLine(DateTime.Now.ToLongDateString());
                foreach (string str in log_rtb.Lines)
                {
                    sw.WriteLine(str);
                }
                sw.Close();
            }
            catch (Exception ex) { Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_MAIN); }
        }
        /// <summary>
        /// Compresses a file using gzip
        /// </summary>
        private void CompressFile(string path)
        {
            FileStream sourceFile = File.OpenRead(path);
            FileStream destinationFile = File.Create(path + ".gz");

            byte[] buffer = new byte[sourceFile.Length];
            sourceFile.Read(buffer, 0, buffer.Length);

            using (GZipStream output = new GZipStream(destinationFile,
                CompressionMode.Compress))
            {
                Console.WriteLine("Compressing {0} to {1}.", sourceFile.Name,
                    destinationFile.Name, false);

                output.Write(buffer, 0, buffer.Length);
            }

            // Close the files.
            sourceFile.Close();
            destinationFile.Close();
        }
        /// <summary>
        /// Shows the current Wnmp log in notepad
        /// </summary>
        private void DoTempLog()
        {
            try
            {
                string temppath = Path.GetTempPath() + "Wnmp";
                if (!Directory.Exists(temppath))
                    Directory.CreateDirectory(temppath);
                StreamWriter sw = new StreamWriter(temppath + @"\Wnmp.log");
                foreach (string str in log_rtb.Lines)
                {
                    sw.WriteLine(str);
                }
                sw.Close();
                Process.Start("notepad.exe ", temppath + @"\Wnmp.log");
            }
            catch (Exception ex) { Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_MAIN); }
        }
        /// <summary>
        /// Deletes a file
        /// </summary>
        private void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex) { Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_MAIN); }
            }
        }
		
        /// <summary>
        /// Takes a form and displays it
        /// </summary>
		private void ShowForm(Form form)
        {
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
            form.Focus();
        }
		
		        private bool IsFirstRun()
        {
            if (Options.settings.firstrun)
                return true;
            else
                return false;
        }

        private void FirstRun()
        {
            if (IsFirstRun())
            {
                try
                {
                    if (!Directory.Exists(Application.StartupPath + "/conf"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "/conf");
                    }
                    File.WriteAllBytes(Application.StartupPath + "/CertGen.exe", Wnmp.Properties.Resources.CertGen);
                    using (Process ps = new Process())
                    {
                        ps.StartInfo.FileName = Application.StartupPath + "/CertGen.exe";
                        ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        ps.Start();
                        ps.WaitForExit();
                        DeleteFile(Application.StartupPath + "/CertGen.exe");
                        Options.settings.firstrun = false;
                        Options.settings.UpdateSettings();
                    }
                }
                catch { }
            }
        }
		
		#endregion

        #region MenuStripItems
        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Updater _Updater = new Updater(UpdateXMLURL, GetCPVER);
        }

        private void wnmpOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options form = new Options();
            ShowForm(form);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SupportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://mailman.getwnmp.org/mailman/listinfo/wnmp");
        }

        private void Report_BugToolStripMenuItem_Click(object sender, EventArgs e)
        {
                Process.Start("https://github.com/wnmp/wnmp/issues/new");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutfrm = new About();
            ShowForm(aboutfrm);
        }

        private void websiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://getwnmp.org");
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=P7LAQRRNF6AVE");
        }

        private void localhostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://localhost");
        }

        private void hostToIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HostToIPForm form = new HostToIPForm();
            ShowForm(form);
        }

        private void getHTTPHeadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HttpHeaders form = new HttpHeaders();
            ShowForm(form);
        }

        #endregion

        #region FormEvents

        private void Main_Resize(object sender, EventArgs e)
        {
            if (Options.settings.minimizewnmptotray == true)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                    WnmpTrayIcon.BalloonTipTitle = "Wnmp";
                    WnmpTrayIcon.BalloonTipText = "Wnmp has been minimized to the taskbar.";
                    WnmpTrayIcon.ShowBalloonTip(4000);
                }
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            DeleteFile(Application.StartupPath + "/updater.exe");
            DeleteFile(Application.StartupPath + "/Wnmp-Upgrade-Installer.exe");

            WnmpTrayIcon.Icon = Wnmp.Properties.Resources.logo;
            WnmpTrayIcon.Visible = true;

            MainHelper.DoStartup();

            System.Threading.Thread worker = new System.Threading.Thread(FirstRun);
            worker.Start();
        }

        private void WnmpTrayIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void wnmpdir_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath);
        }

        private void log_rtb_DoubleClick(object sender, EventArgs e)
        {
            DoTempLog();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoSaveLog();
        }

        #endregion

        #region events
        private void setevents()
        {
            Log.setLogComponent(log_rtb);
            WnmpTrayIcon.Click += WnmpTrayIcon_Click;
            // General Events Start
            start.Click += General.start_Click;
            stop.Click += General.stop_Click;
            // End
            // Nginx Events Start
            ngx_start.Click += Nginx.ngx_start_Click;
            ngx_stop.Click += Nginx.ngx_stop_Click;
            ngx_reload.Click += Nginx.ngx_reload_Click;
            ngx_config.Click += Nginx.ngx_cfg_Click;
            ngx_log.Click += Nginx.ngx_log_Click;
            ngx_start.MouseHover += Nginx.ngx_start_MouseHover;
            ngx_stop.MouseHover += Nginx.ngx_stop_MouseHover;
            ngx_reload.MouseHover += Nginx.ngx_reload_MouseHover;
            // End
            // MariaDB Events Start
            mdb_start.Click += MariaDB.mdb_start_Click;
            mdb_stop.Click += MariaDB.mdb_stop_Click;
            mdb_help.Click += MariaDB.mdb_help_Click;
            mdb_shell.Click += MariaDB.mdb_shell_Click;
            mdb_cfg.Click += MariaDB.mdb_cfg_Click;
            // mdb_log.Click += WnmpFunctions.mdb_log_Click;
            mdb_start.MouseHover += MariaDB.mdb_start_MouseHover;
            mdb_stop.MouseHover += MariaDB.mdb_stop_MouseHover;
            mdb_shell.MouseHover += MariaDB.mdb_shell_MouseHover;
            // End
            // PHP Events Start
            php_start.Click += PHP.php_start_Click;
            php_stop.Click += PHP.php_stop_Click;
            php_cfg.Click += PHP.php_cfg_Click;
            php_log.Click += PHP.php_log_Click;
            php_start.MouseHover += PHP.php_start_MouseHover;
            php_stop.MouseHover += PHP.php_stop_MouseHover;
            // End
        }
        #endregion
    }
}