using System.IO;
using FileManager.ViewModels;
using System.Windows.Controls;

namespace FileManager
{
    public class DriveExplorer
    {
        public static void ShowDrives(TextBox addressBar, ListView listBar)
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
                    DriveViewModel crrDriveShort = new(drive.Name, drive.Name);
                    listBar.Items.Add(crrDriveShort);
                }
            }
        }
    }
}



