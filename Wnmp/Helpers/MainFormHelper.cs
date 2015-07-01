﻿/*
 * Copyright (c) 2012 - 2015, Kurt Cancemi (kurt@x64architecture.com)
 *
 * This file is part of Wnmp.
 *
 *  Wnmp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Wnmp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Wnmp.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

using Wnmp.Forms;
using Wnmp.Programs;
using Wnmp.Internals;
namespace Wnmp.Helpers
{
    /// <summary>
    /// Additional functions for the Main form
    /// </summary>
    class MainHelper
    {
        public Main form;

        /// <summary>
        /// Checks if Nginx, MariaDB, and PHP exist in the Wnmp directory
        /// </summary>
        public void checkforapps()
        {
            Log.wnmp_log_notice("Checking for applications", Log.LogSection.WNMP_MAIN);
            if (!File.Exists(Application.StartupPath + "/nginx.exe"))
                Log.wnmp_log_error("Error: Nginx Not Found", Log.LogSection.WNMP_NGINX);

            if (!Directory.Exists(Application.StartupPath + "/mariadb"))
                Log.wnmp_log_error("Error: MariaDB Not Found", Log.LogSection.WNMP_MARIADB);

            if (!Directory.Exists(Application.StartupPath + "/php"))
                Log.wnmp_log_error("Error: PHP Not Found", Log.LogSection.WNMP_PHP);
        }

        /// <summary>
        /// Adds configuration files to the Config buttons context menu strip
        /// </summary>
        public void DirFiles(string path, string GetFiles, ContextMenuStrip cms)
        {
            DirectoryInfo dinfo = new DirectoryInfo(Main.StartupPath + path);

            if (!dinfo.Exists)
                return;

            FileInfo[] Files = dinfo.GetFiles(GetFiles);
            foreach (FileInfo file in Files)
                cms.Items.Add(file.Name, null);
        }

        /// <summary>
        /// Sets up the timer to check if the applications are running
        /// </summary>
        public void DoTimer()
        {
            CheckIfAppsAreRunning(); // First we check at startup
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += CheckIfAppsAreRunningTimer_Tick;
            timer.Start();
        }

        private void CheckIfAppsAreRunningTimer_Tick(object sender, System.EventArgs e)
        {
            CheckIfAppsAreRunning();
        }

        /// <summary>
        /// Checks if Nginx, MariaDB or PHP is running
        /// </summary>
        public void CheckIfAppsAreRunning()
        {
            check_if_running("nginx", form.nginxrunning);
            check_if_running("mysqld", form.mariadbrunning);
            check_if_running("php-cgi", form.phprunning);
        }

        private void check_if_running(string application, Label label)
        {
            Process[] process = Process.GetProcessesByName(application);
            if (process.Length != 0)
                Common.ToStartedLabel(label);
            else
                Common.ToStoppedLabel(label);
        }

        private bool IsFirstRun()
        {
            return (Options.settings.FirstRun);
        }

        /// <summary>
        /// Generates a public and private keypair the first time Wnmp is launched
        /// </summary>
        public void FirstRun()
        {
            if (IsFirstRun() == false)
                return;

            if (!Directory.Exists(Main.StartupPath + "/conf"))
                Directory.CreateDirectory(Main.StartupPath + "/conf");
            else if (!File.Exists(Main.StartupPath + "/bin/CertGen.exe"))
                return; // CertGen.exe doesn't exist. (FAILURE)

            using (Process ps = new Process()) {
                ps.StartInfo.FileName = Main.StartupPath + "/bin/CertGen.exe";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                Options.settings.FirstRun = false;
                Options.settings.UpdateSettings();
            }
        }
    }
}