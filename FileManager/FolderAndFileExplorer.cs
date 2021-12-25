using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class FolderAndFileExplorer
    {
        public static void ShowContentFolder(System.Windows.Controls.TextBlock addressBar, System.Windows.Controls.ListView listBar)
        {
            listBar.Items.Clear();

            DirectoryInfo dir = new(addressBar.Text);
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            ShowFilteredFoldersList(dirs, listBar);

            ShowFilteredFilesList(files, listBar);
        }

        private static void ShowFilteredFoldersList(DirectoryInfo[] dirs, System.Windows.Controls.ListView listBar)
        {
            var filteredDirs = dirs.Where(crrDir => !crrDir.Attributes.HasFlag(FileAttributes.Hidden));

            foreach (DirectoryInfo crrDir in filteredDirs)
            {
                listBar.Items.Add(crrDir);
            }
        }

        private static void ShowFilteredFilesList(FileInfo[] files, System.Windows.Controls.ListView listBar)
        {
            var filteredFiles = files.Where(crrFile => !crrFile.Attributes.HasFlag(FileAttributes.Hidden));

            foreach (FileInfo crrFile in filteredFiles)
            {
                listBar.Items.Add(crrFile);
            }
        }     
    }
}
