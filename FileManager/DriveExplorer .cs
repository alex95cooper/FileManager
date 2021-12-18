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

        public void ShowDriveList(System.Windows.Controls.ListView ListBar)
        {
            ListBar.Items.Clear();

            foreach (DriveInfo drive  in drives)
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    ListBar.Items.Add(drive);
                } 
            }
        }

    }
}



