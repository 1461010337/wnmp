/*
Copyright (C) Kurt Cancemi

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace Wnmp
{
    public partial class Main : Form
    {
        public static string getappsupath = Application.StartupPath;
        internal Version CPVER = new Version("2.1.1");
        internal const string UpdateXMLURL = "https://s3.amazonaws.com/wnmp/update.xml";

        public Main()
        {
            InitializeComponent();
            setevents();
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

        #region Wnmp Stuff

        #region MenuStripItems
        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string xmlUrl = UpdateXMLURL;
            Updater _Updater = new Updater(xmlUrl, CPVER);
        }

        private void wnmpOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options form = new Options();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
            form.Focus();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void SupportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://groups.google.com/group/windows-nginx-mysql-php-discuss");
        }
        private void Report_BugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string desktoppath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            try
            {
                foreach (string file in Directory.GetFiles(Application.StartupPath + @"\logs", "*error*"))
                {
                    if (!Directory.Exists(desktoppath + @"\Wnmpissuefiles"))
                        Directory.CreateDirectory(desktoppath + @"\Wnmpissuefiles");
                    if (File.Exists(file))
                        File.Copy(file, desktoppath + @"\Wnmpissuefiles\" + Path.GetFileName(file), true);
                }
                if (!Directory.Exists(desktoppath + @"\Wnmpissuefiles"))
                    Directory.CreateDirectory(desktoppath + @"\Wnmpissuefiles");
                File.Copy(Application.StartupPath + "/php/logs/sys.log", desktoppath + @"\Wnmpissuefiles\sys.log", true);
                MessageBox.Show(String.Format("Attach the error log inside the {0} folder to the issue report that is associated with the problem you are facing.", desktoppath + @"\Wnmpissuefiles"));
                Process.Start("https://github.com/wnmp/wnmp/issues/new");
            }
            catch (Exception ex) { Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_MAIN); }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Wnmp.Forms.About aboutfrm = new Wnmp.Forms.About();
            aboutfrm.StartPosition = FormStartPosition.CenterParent;
            aboutfrm.ShowDialog(this);
            aboutfrm.Focus();
        }
        private void websiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://wnmp.x64Architecture.com");
        }
        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=P7LAQRRNF6AVE");
        }

        private void localhostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://localhost");
        }

        #endregion

        #region Functions

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

        private void DoChristmas()
        {
            this.BackgroundImage = Wnmp.Properties.Resources.background;
            Font font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold);
            Font Rfont = new Font("Microsoft Sans Serif", 16f, FontStyle.Bold);
            ToolStripMenuItem item = new ToolStripMenuItem("Merry Christmas From Kurt!", null);
            item.Font = new Font("Segoe Script", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            item.ForeColor = Color.DarkGreen;
            menuStrip1.Items.Add(item);
            this.menuStrip1.Font = font;
            this.label4.Font = font;
            this.label7.Font = font;
            this.label8.Font = font;
            this.nginxrunning.Font = Rfont;
            this.phprunning.Font = Rfont;
            this.mariadbrunning.Font = Rfont;
            this.log_rtb.BackColor = Color.LightGreen;
            Log.wnmp_log_notice("Merry Christmas From Kurt!", Log.LogSection.WNMP_MAIN);
        }

        #endregion

        #region FormEvents

        private void Main_Resize(object sender, EventArgs e)
        {
            if (Wnmp.Properties.Settings.Default.mwttbs == true)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                    icon.BalloonTipTitle = "Wnmp";
                    icon.BalloonTipText = "Wnmp has been minimized to the taskbar.";
                    icon.ShowBalloonTip(3000);
                }
            }
            else { }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            DeleteFile(@Application.StartupPath + "/updater.exe");
            DeleteFile(@Application.StartupPath + "/Wnmp-Upgrade-Installer.exe");

            DateTime now = DateTime.Now;
            if (now.Month == 12 && now.Day == 25)
                DoChristmas();

            timer1.Enabled = true;
            WnmpFunctions.DoStartup();
        }

        private void icon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void wnmpdir_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", @Application.StartupPath);
        }

        #region events
        private void setevents()
        {
            Log.setLogComponent(log_rtb);
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
            //mdb_log.Click += WnmpFunctions.mdb_log_Click;
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

        #endregion

        #endregion Wnmp Stuff
    }
}