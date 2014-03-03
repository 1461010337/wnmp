﻿/*
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
using System.Security.Permissions;
using Wnmp.Helpers;

namespace Wnmp.Programs
{
    /// <summary>
    /// Functions/Handlers releated to Nginx
    /// </summary>
    class Nginx
    {
        public static Process ps; // Avoid GC
        public static ContextMenuStrip cms = new ContextMenuStrip(); // Config button context menu
        public static ContextMenuStrip lms = new ContextMenuStrip(); // Log button context menu
        private static ToolTip nginx_start_Tip = new ToolTip(); // Start button ToolTip
        private static ToolTip nginx_stop_Tip = new ToolTip(); // Stop button ToolTip
        private static ToolTip nginx_reload_Tip = new ToolTip(); // Reload button ToolTip

        private static string NginxExe = Application.StartupPath.Replace(@"\", "/") + "/nginx.exe";

        /// <summary>
        /// Starts an executable file
        /// </summary>
        public static void startprocess(string p, string args, bool wfe)
        {
            System.Threading.Thread.Sleep(100); // Wait
            ps = new Process(); // Create process
            ps.StartInfo.FileName = p; // p is the path and file name of the file to run
            ps.StartInfo.Arguments = args; // Parameters to pass to program
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.RedirectStandardOutput = true; // Set output of program to be written to process output stream
            ps.StartInfo.WorkingDirectory = Application.StartupPath;
            ps.StartInfo.CreateNoWindow = true; // Excute with no window
            ps.Start(); // Start the process
            if (wfe)
            {
                ps.WaitForExit();
            }
        }

        internal static void ngx_start_Click(object sender, EventArgs e)
        {
            try
            {
                startprocess(NginxExe, "", false);
                Log.wnmp_log_notice("Attempting to start Nginx", Log.LogSection.WNMP_NGINX);
                Program.formInstance.nginxrunning.Text = "\u221A";
                Program.formInstance.nginxrunning.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_NGINX);
            }
        }

        internal static void ngx_stop_Click(object sender, EventArgs e)
        {
            try
            {
                startprocess(NginxExe, "-s stop", true);
                System.Threading.Thread.Sleep(300); // Lets give nginx 300 miliseconds to stop
                /* Ensure Nginx gets killed (No leftover useless proccess) */
                Process[] ngx = System.Diagnostics.Process.GetProcessesByName("nginx");
                foreach (Process currentProc in ngx)
                {
                    currentProc.Kill();
                }
                Log.wnmp_log_notice("Attempting to stop Nginx", Log.LogSection.WNMP_NGINX);
                Program.formInstance.nginxrunning.Text = "X";
                Program.formInstance.nginxrunning.ForeColor = Color.DarkRed;
            }
            catch (Exception ex)
            {
                Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_NGINX);
            }
        }

        internal static void ngx_reload_Click(object sender, EventArgs e)
        {
            try
            {
                startprocess(NginxExe, "-s reload", false);
                Log.wnmp_log_notice("Attempting to reload Nginx", Log.LogSection.WNMP_NGINX);
            }
            catch (Exception ex)
            {
                Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_NGINX);
            }
        }

        internal static void ngx_stop_MouseHover(object sender, EventArgs e)
        {
            nginx_stop_Tip.Show("Stop Nginx", Program.formInstance.ngx_stop);
        }

        internal static void ngx_start_MouseHover(object sender, EventArgs e)
        {
            nginx_start_Tip.Show("Start Nginx", Program.formInstance.ngx_start);
        }

        internal static void ngx_reload_MouseHover(object sender, EventArgs e)
        {
            nginx_reload_Tip.Show("Reloads Nginx configuration without restart", Program.formInstance.ngx_reload);
        }

        internal static void ngx_cfg_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            cms.Show(ptLowerLeft);
            cms.ItemClicked -= cms_ItemClicked;
            cms.ItemClicked += cms_ItemClicked;
        }

        static void cms_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Process.Start(Options.settings.editor, Application.StartupPath + "/conf/" + e.ClickedItem.Text);
        }

        internal static void ngx_log_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            lms.Show(ptLowerLeft);
            lms.ItemClicked -= lms_ItemClicked;
            lms.ItemClicked += lms_ItemClicked;
        }

        static void lms_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Process.Start(Options.settings.editor, Application.StartupPath + "/logs/" + e.ClickedItem.Text);
        }
    }
}
