using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileManager
{
    public class DriveExplorer
    {
        public static void ShowDrives(TextBlock addressBar, ListView listBar)
        {
            addressBar.Text = null;

            listBar.Items.Clear();

            DriveInfo[] drives = DriveInfo.GetDrives();

            ShowDriveList(drives, listBar);
        }

        private static void ShowDriveList(DriveInfo[] drives, ListView listBar)
        {
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    listBar.Items.Add(drive);
                }
            }
        }
    }
}



