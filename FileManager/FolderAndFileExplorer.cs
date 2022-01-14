using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileManager
{
    public class FolderAndFileExplorer
    {
        public static void ShowContentFolder(TextBlock addressBar, ListView listBar)
        {
            listBar.Items.Clear();

            DirectoryInfo dir = new(addressBar.Text);
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            ShowFilteredFoldersList(dirs, listBar);

            ShowFilteredFilesList(files, listBar);
        }

        private static void ShowFilteredFoldersList(DirectoryInfo[] dirs, ListView listBar)
        {
            var filteredDirs = dirs.Where(crrDir => !crrDir.Attributes.HasFlag(FileAttributes.Hidden));

            foreach (DirectoryInfo crrDir in filteredDirs)
            {
                listBar.Items.Add(crrDir);
            }
        }

        private static void ShowFilteredFilesList(FileInfo[] files, ListView listBar)
        {
            var filteredFiles = files.Where(crrFile => !crrFile.Attributes.HasFlag(FileAttributes.Hidden));

            foreach (FileInfo crrFile in filteredFiles)
            {
                listBar.Items.Add(crrFile);
            }
        }
    }
}
