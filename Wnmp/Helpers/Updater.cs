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
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.Diagnostics;
using System.IO;

using Wnmp.Internals;

namespace Wnmp.Helpers
{
    /// <summary>
    /// Updater for Wnmp and the control panel
    /// </summary>
    class Updater
    {
        private static Uri Wnmp_Upgrade_URL; // Wnmp upgrade installer url
        private static Version NEW_WNMP_VERSION = null; // Wnmp version in the XML
        private static Version NEW_CP_VERSION = null; // Control panel version in the XML
        private static Uri CP_UPDATE_URL; // Control panel url (link to CP exe)
        private static Version WNMP_VER = new Version(Application.ProductVersion); // Current program version
        private static string UpdateExe = Application.StartupPath + "/Wnmp-Upgrade-Installer.exe";
        private static string WNMP_NEW = Application.StartupPath + "/Wnmp_new.exe";
        private static string UPDATER = Application.StartupPath + "/updater.exe";
        private static WebClient webClient;

        #region ReadUpdateXML
        /// <summary>
        /// Fetches and reads the update xml
        /// </summary>
        /// <returns>True on sucess and False on failure</returns>
        private static bool ReadUpdateXML()
        {
            string xmlUrl = Main.UpdateXMLURL;
            XmlTextReader reader;
            string elementName = "";

            int returnvalue;
            if (!NativeMethods.InternetGetConnectedState(out returnvalue, 0))
            {
                MessageBox.Show("No active network connection detected", "Can't Check For Updates");
                return false;
            }

            try
            {
                reader = new XmlTextReader(xmlUrl);
                reader.MoveToContent();

                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "appinfo"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            elementName = reader.Name;
                        }
                        else
                        {
                            if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                                switch (elementName)
                                {
                                    case "version":
                                        NEW_WNMP_VERSION = new Version(reader.Value);
                                        break;
                                    case "upgradeurl":
                                        Wnmp_Upgrade_URL = new Uri(reader.Value);
                                        break;
                                    case "cpversion":
                                        NEW_CP_VERSION = new Version(reader.Value);
                                        break;
                                    case "cpupdateurl":
                                        CP_UPDATE_URL = new Uri(reader.Value);
                                        break;
                                }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
        #endregion
        /// <summary>
        /// Downloads the update for Wnmp
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="path"></param>
        private static void DownloadWnmpUpdate(Uri uri, string path)
        {
            UpdateProgress frm = new UpdateProgress();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
            Program.formInstance.Enabled = false;

            webClient = new WebClient();

            frm.FormClosed += (s, e) =>
            {
                Program.formInstance.Enabled = true;
                webClient.CancelAsync();
            };

            webClient.DownloadProgressChanged += (s, e) =>
            {
                frm.progressBar1.Value = e.ProgressPercentage;
                frm.label2.Text = e.ProgressPercentage.ToString() + "%";
            };

            webClient.DownloadFileCompleted += (s, e) =>
            {
                if (!e.Cancelled)
                {
                    webClient.Dispose();
                    frm.Close();
                    Process.Start(UpdateExe);
                    KillProcesses();
                    DoBackUp();
                    Application.Exit();
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    webClient.Dispose();
                }
            };

            webClient.DownloadFileAsync(uri, path);

            webClient.Dispose();
        }

        /// <summary>
        /// Downloads the update for the control panel
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="path"></param>
        private static void DownloadCPUpdate(Uri uri, string path)
        {
            webClient = new WebClient();
            UpdateProgress frm = new UpdateProgress();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
            Program.formInstance.Enabled = false;

            frm.FormClosed += (s, e) =>
            {
                Program.formInstance.Enabled = true;
                webClient.CancelAsync();
            };

            webClient.DownloadProgressChanged += (s, e) =>
            {
                frm.progressBar1.Value = e.ProgressPercentage;
                frm.label2.Text = e.ProgressPercentage.ToString() + "%";
            };

            webClient.DownloadFileCompleted += (s, e) =>
            {
                if (!e.Cancelled)
                {
                    webClient.Dispose();
                    frm.Close();
                    File.WriteAllBytes(UPDATER, Properties.Resources.updater);
                    Process.Start(UPDATER);
                    Application.Exit();
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    webClient.Dispose();
                }
            };

            webClient.DownloadFileAsync(uri, path);

            webClient.Dispose();
        }

        /// <summary>
        /// Checks for updates
        /// </summary>
        public static void CheckForUpdates()
        {
            bool FoundWnmpUpdate = false; // Since were checking for two updates we have to check if it found the main one.

            if (ReadUpdateXML())
            {

                if (WNMP_VER.CompareTo(NEW_WNMP_VERSION) < 0) // If it returns less than 0 than theres a new version
                {
                    ChangelogViewer CV = new ChangelogViewer();
                    CV.StartPosition = FormStartPosition.CenterScreen;
                    CV.cversion.Text = WNMP_VER.ToString();
                    CV.newversion.Text = NEW_WNMP_VERSION.ToString();
                    if (CV.ShowDialog() == DialogResult.Yes)
                    {
                        FoundWnmpUpdate = true;
                        DownloadWnmpUpdate(Wnmp_Upgrade_URL, UpdateExe);
                    }
                }
                else
                {
                    Log.wnmp_log_notice("Your version: " + WNMP_VER + " is up to date.", Log.LogSection.WNMP_MAIN);
                }

                if (FoundWnmpUpdate != true)
                {
                    if (Main.GetCPVER.CompareTo(NEW_CP_VERSION) < 0)
                    {
                        ChangelogViewer CV = new ChangelogViewer();
                        CV.StartPosition = FormStartPosition.CenterScreen;
                        CV.cversion.Text = Main.GetCPVER.ToString();
                        CV.newversion.Text = NEW_CP_VERSION.ToString();

                        if (CV.ShowDialog() == DialogResult.Yes)
                        {
                            DownloadCPUpdate(CP_UPDATE_URL, WNMP_NEW);
                        }
                    }
                    else
                    {
                        Log.wnmp_log_notice("Your control panel version: " + Main.GetCPVER + " is up to date.", Log.LogSection.WNMP_MAIN);
                    }
                }
                Options.settings.lastcheckforupdate = DateTime.Now;
                Options.settings.UpdateSettings();
            }
        }

        /// <summary>
        /// Backs up the configuration files for Nginx, MariaDB, and PHP
        /// </summary>
        private static void DoBackUp()
        {
            string wd = Main.StartupPath;
            string[] files = { wd + "/php/php.ini", wd + "/conf/nginx.conf", wd + "/mariadb/my.ini" };
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    string dest = String.Format("{0}.old", file);
                    File.Copy(file, dest, true);
                    Log.wnmp_log_notice("Backed up " + file + " to " + dest, Log.LogSection.WNMP_MAIN);
                }
            }
        }

        #region AutoCheckForUpdates
        /// <summary>
        /// Checks if a string is empty
        /// </summary>
        /// <param name="s"></param>
        /// <returns>True if the datetime is set else it returns false</returns>
        public static bool IsSet(DateTime dt)
        {
            if (dt != DateTime.MinValue)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the curent date if greater than the selected update freqency
        /// and excutes the updater if true.
        /// </summary>
        /// <param name="days"></param>
        private static void DoDateEclasped(double days)
        {
            try
            {
                if (IsSet(Options.settings.lastcheckforupdate))
                {
                    DateTime LastCheckForUpdate = Options.settings.lastcheckforupdate;
                    DateTime expiryDate = LastCheckForUpdate.AddDays(days);
                    if (DateTime.Now > expiryDate)
                    {
                        CheckForUpdates();
                    }
                }
                else
                {
                    Options.settings.lastcheckforupdate = DateTime.Now;
                    Options.settings.UpdateSettings();
                }
            }
            catch (Exception ex)
            {
                Log.wnmp_log_error(ex.Message, Log.LogSection.WNMP_MAIN);
            }
        }
        /// <summary>
        /// Tells the Updater the selected update frequency
        /// </summary>
        public static void DoAutoCheckForUpdate()
        {
            if (Options.settings.autocheckforupdates == true)
            {
                switch (Options.settings.checkforupdatefrequency)
                {
                    case 1:
                        DoDateEclasped(1);
                        break;
                    case 7:
                        DoDateEclasped(7);
                        break;
                    case 30:
                        DoDateEclasped(30);
                        break;
                    default:
                        DoDateEclasped(7); /* Default: To check for updates every week. */
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Kills Nginx, MariaDB, and PHP
        /// </summary>
        private static void KillProcesses()
        {
            string[] processtokill = { "php-cgi", "nginx", "mysqld" };
            Process[] processes = Process.GetProcesses();

            for (int i = 0; i < processes.Length; i++)
            {
                for (int j = 0; j < processtokill.Length; j++)
                {
                    try
                    {
                        string tempProcess = processes[i].ProcessName;

                        if (tempProcess == processtokill[j]) // If the proccess is the proccess we want to kill
                        {
                            processes[i].Kill(); break; // Kill the proccess
                        }
                    }
                    catch { }
                }
            }
        } // End of KillProcesses()
    }
}