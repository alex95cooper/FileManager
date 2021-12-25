using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class DriveExplorer 
    {
        public static void ShowDrives(System.Windows.Controls.TextBlock addressBar , System.Windows.Controls.ListView listBar)
        {
            addressBar.Text = null;

            listBar.Items.Clear();

            DriveInfo[] drives = DriveInfo.GetDrives();

            ShowDriveList(drives, listBar);
        }

        private static void ShowDriveList(DriveInfo[] drives, System.Windows.Controls.ListView listBar)
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



