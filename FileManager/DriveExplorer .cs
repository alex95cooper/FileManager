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
        readonly DriveInfo[] drives = DriveInfo.GetDrives();

        public void ShowDriveList(System.Windows.Controls.TextBlock addressBar , System.Windows.Controls.ListView listBar)
        {
            addressBar.Text = null;

            listBar.Items.Clear();

            foreach (DriveInfo drive  in drives)
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    listBar.Items.Add(drive);
                } 
            }
        }

    }
}



