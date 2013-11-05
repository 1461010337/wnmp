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
using System.Xml;

namespace Wnmp
{
    class PHP
    {
        public static Process ps; // Avoid GC
        internal static string pini = Application.StartupPath + "/php/php.ini";
        public static int phpstatus = (int)ProcessStatus.ps.STOPPED;
        public static void startprocess(string p, string args)
        {
            System.Threading.Thread.Sleep(100); //Wait
            ps = new Process(); //Create process
            ps.StartInfo.FileName = p; //p is the path and file name of the file to run
            ps.StartInfo.Arguments = args; //Parameters to pass to program
            ps.StartInfo.UseShellExecute = false;
            ps.StartInfo.RedirectStandardOutput = true; //Set output of program to be written to process output stream
            ps.StartInfo.WorkingDirectory = Application.StartupPath;
            ps.StartInfo.CreateNoWindow = true; //Excute with no window
            ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ps.StartInfo.EnvironmentVariables.Add("PHP_FCGI_MAX_REQUESTS", "2000"); // After 2000 requests php-cgi.exe will kill its process
            ps.EnableRaisingEvents = true;
            ps.Exited += new EventHandler(ps_Exited); // Were going to have to restart PHP after its process is killed.
            ps.Start(); //Start the process
        }
        static void ps_Exited(object sender, EventArgs e)
        {
            /* The Win-PHP developers thought is was smart to kill php after a certain amount of requests (Probably 500). 
               so we have to restart it once it exits or set 'PHP_FCGI_MAX_REQUESTS' variable to 0. I've looked and people are recommending just to restart it. */
            if (PHPStatus == 0) // Check if PHP is set to run
            {
                startprocess(@Application.StartupPath + "/php/php-cgi.exe", String.Format("-b localhost:9000 -c {0}", pini));
            }
        }
        internal static void php_start_Click(object sender, EventArgs e)
        {
            try
            {
                startprocess(@Application.StartupPath + "/php/php-cgi.exe", String.Format("-b localhost:9000 -c {0}", pini));
                Log.wnmp_log_notice("Attempting to start PHP", Log.LogSection.WNMP_PHP);
                Program.formInstance.phprunning.Text = "\u221A";
                Program.formInstance.phprunning.ForeColor = Color.Green;
                phpstatus = (int)ProcessStatus.ps.STARTED;
            }
            catch (Exception ex)
            {
                Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_PHP);
            }
        }

        internal static void php_stop_Click(object sender, EventArgs e)
        {
            try
            {
                Process[] phps = System.Diagnostics.Process.GetProcessesByName("php-cgi");
                foreach (Process currentProc in phps)
                {
                    currentProc.Kill();
                }
                phpstatus = (int)ProcessStatus.ps.STOPPED;
            }
            catch (Exception ex)
            {
                Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_PHP);
            }
            Log.wnmp_log_notice("Attempting to stop PHP", Log.LogSection.WNMP_PHP);
            Program.formInstance.phprunning.Text = "X";
            Program.formInstance.phprunning.ForeColor = Color.DarkRed;
        }
        public static int PHPStatus { get { return phpstatus; } }
        internal static void php_start_MouseHover(object sender, EventArgs e)
        {
            ToolTip mysql_start_Tip = new ToolTip();
            mysql_start_Tip.Show("Start PHP-CGI", Program.formInstance.php_start);
        }

        internal static void php_stop_MouseHover(object sender, EventArgs e)
        {
            ToolTip mysql_stop_Tip = new ToolTip();
            mysql_stop_Tip.Show("Stop PHP-CGI", Program.formInstance.php_stop);
        }
    }
}
