﻿/*
Copyright (C) Kurt Cancemi

This file is part of Wnmp.

    Wnmp is free software: you can redistribute it and/or modify
    it under the terms of the GNU Wnmp Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Wnmp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Wnmp Public License for more details.

    You should have received a copy of the GNU Wnmp Public License
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
using System.Xml;
using Wnmp.Programs;
using Wnmp.Helpers;

namespace Wnmp.Helpers
{
    /// <summary>
    /// Additional functions for the Main form
    /// </summary>
    class MainHelper
    {
        #region checkforapps
        /// <summary>
        /// Checks if Nginx, MariaDB, and PHP exist in the Wnmp directory
        /// </summary>
        internal static void checkforapps()
        {
            Log.wnmp_log_notice("Checking for applications", Log.LogSection.WNMP_MAIN);
            if (!File.Exists(@Application.StartupPath + "/nginx.exe"))
                Log.wnmp_log_error("Error: Nginx Not Found", Log.LogSection.WNMP_NGINX);

            if (!Directory.Exists(@Application.StartupPath + @"/mariadb"))
                Log.wnmp_log_error("Error: MariaDB Not Found", Log.LogSection.WNMP_MARIADB);

            if (!Directory.Exists(@Application.StartupPath + @"/php"))
                Log.wnmp_log_error("Error: PHP Not Found", Log.LogSection.WNMP_PHP);
        }
        #endregion checkforapps

        internal static void DoStartup()
        {
            Log.wnmp_log_notice("Control Panel Version: " + Program.formInstance.GetCPVER, Log.LogSection.WNMP_MAIN);
            Log.wnmp_log_notice("Wnmp Version: " + Program.formInstance.ProductVersion, Log.LogSection.WNMP_MAIN);
            Log.wnmp_log_notice(OSVersionInfo.WindowsVersionString(), Log.LogSection.WNMP_MAIN);
            Log.wnmp_log_notice("Wnmp Directory: " + Application.StartupPath, Log.LogSection.WNMP_MAIN);
            checkforapps();
            CheckIfAppsAreRunning();
            Log.wnmp_log_notice("Wnmp ready to go!", Log.LogSection.WNMP_MAIN);

            if (Wnmp.Properties.Settings.Default.startallappsatlaunch == true)
            {
                General.start_Click(null, null);
            }

            DirFiles("/conf", "*", Nginx.cms);
            DirFiles("/mariadb", "my.ini", MariaDB.cms);
            DirFiles("/php", "php.ini", PHP.cms);
            DirFiles("/logs", "*.log", Nginx.lms);
            DirFiles("/mariadb/data", "*.log", MariaDB.lms);
            DirFiles("/php/logs", "*.log", PHP.lms);

            Updater.DoAutoCheckForUpdate();
        }
        /// <summary>
        /// Adds configuration files to the Config buttons context menu strip
        /// </summary>
        internal static void DirFiles(string path, string GetFiles, ContextMenuStrip cms)
        {
            try
            {
                DirectoryInfo dinfo = new DirectoryInfo(@Application.StartupPath + path);
                FileInfo[] Files = dinfo.GetFiles(GetFiles);
                foreach (FileInfo file in Files)
                {
                    cms.Items.Add(file.Name, null);
                }
            }
            catch { }
        }

        internal static void timer1_Tick()
        {
            CheckIfAppsAreRunning();
        }

        #region CheckIfRunning
        /// <summary>
        /// Checks if Nginx, MariaDB or PHP is running
        /// </summary>
        internal static void CheckIfAppsAreRunning()
        {
            if (check_if_running("nginx"))
            {
                Program.formInstance.nginxrunning.Text = "\u221A";
                Program.formInstance.nginxrunning.ForeColor = Color.Green;
            }
            else
            {
                Program.formInstance.nginxrunning.Text = "X";
                Program.formInstance.nginxrunning.ForeColor = Color.DarkRed;
            }
            if (check_if_running("mysqld"))
            {
                Program.formInstance.mariadbrunning.Text = "\u221A";
                Program.formInstance.mariadbrunning.ForeColor = Color.Green;
            }
            else
            {
                Program.formInstance.mariadbrunning.Text = "X";
                Program.formInstance.mariadbrunning.ForeColor = Color.DarkRed;
            }
            if (check_if_running("php-cgi"))
            {
                Program.formInstance.phprunning.Text = "\u221A";
                Program.formInstance.phprunning.ForeColor = Color.Green;
            }
            else
            {
                Program.formInstance.phprunning.Text = "X";
                Program.formInstance.phprunning.ForeColor = Color.DarkRed;
            }
        }

        private static bool check_if_running(string application)
        {
            Process[] _Process = Process.GetProcessesByName(application);
            if (_Process.Length != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}