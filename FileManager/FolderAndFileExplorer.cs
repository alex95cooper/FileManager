using System.IO;
using System.Linq;
using System.Windows;
using FileManager.ViewModels;
using System.Windows.Controls;

namespace FileManager
{
    public class FolderAndFileExplorer
    {
        public static void ShowContentFolder(TextBox addressBar, ListView listBar)
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
                FolderViewModel crrDirShort = new(crrDir.Name, crrDir.FullName);
                listBar.Items.Add(crrDirShort);
            }
        }

        private static void ShowFilteredFilesList(FileInfo[] files, ListView listBar)
        {
            var filteredFiles = files.Where(crrFile => !crrFile.Attributes.HasFlag(FileAttributes.Hidden));

            foreach (FileInfo crrFile in filteredFiles)
            {
                FileViewModel crrFileShort = new(crrFile.Name, crrFile.FullName);
                listBar.Items.Add(crrFileShort);
            }
        }
    }
}
