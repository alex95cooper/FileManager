using System;
using System.IO;
using System.Windows;
using FileManager.ViewModels;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace FileManager
{
    public partial class Property : Window
    {
        private readonly int bytesInGygabytesValue = 1_073_741_824;
        private readonly int bytesInMegabytesValue = 1_048_576;

        private readonly string notExistenceMessage = "Current folder or file no longer exists!";

        public Property()
        {
            InitializeComponent();
        }

        public void CategorySelector(ListView listBar, TextBox addressBar)
        {
            if (listBar.SelectedItem is DriveViewModel driveViewModel)
            {
                DriveInfo drive = new(driveViewModel.Name);
                ShowDriveProperties(drive);
            }
            else if (listBar.SelectedItem is FolderViewModel folderViewModel && Directory.Exists(folderViewModel.Path))
            {
                if (!Directory.Exists(folderViewModel.Path))
                {
                    MessageBox.Show(notExistenceMessage);
                }
                else
                {
                    DirectoryInfo crrDir = new(folderViewModel.Path);
                    ShowFolderProperties(crrDir);
                }
            }
            else if (listBar.SelectedItem is FileViewModel fileViewModel && File.Exists(fileViewModel.Path))
            {
                if (!File.Exists(fileViewModel.Path))
                    MessageBox.Show(notExistenceMessage);
                else
                    ShowFileProperties(fileViewModel);
            }
            else
            {
                if (Path.GetPathRoot(addressBar.Text) == addressBar.Text)
                {
                    DriveInfo drive = new(addressBar.Text);
                    ShowDriveProperties(drive);
                }
                else
                {
                    DirectoryInfo crrDir = new(addressBar.Text);
                    ShowFolderProperties(crrDir);
                }
            }
        }

        private void ShowDriveProperties(DriveInfo drive)
        {
            FolderandFileGrid.Visibility = Visibility.Collapsed;
            propertyIcon.Source = new BitmapImage(new Uri(@"Images\drive_icon.png", UriKind.Relative));
            NameBar.Text = drive.Name;
            TypeBar.Text = drive.DriveType.ToString();
            FileSystemBar.Text = drive.DriveFormat.ToString();
            BusyBar.Text = $"{drive.TotalSize - drive.TotalFreeSpace:#,#} Bytes  or  {(double)(drive.TotalSize - drive.TotalFreeSpace) / bytesInGygabytesValue:0.0} Gb";
            FreeBar.Text = $"{drive.TotalFreeSpace:#,#} Bytes  or  {(double)drive.TotalFreeSpace / bytesInGygabytesValue:0.0} Gb";
            CapacityBar.Text = $"{drive.TotalSize:#,#} Bytes  or  {(double)drive.TotalSize / bytesInGygabytesValue:0.0} Gb";
        }

        private void ShowFolderProperties(DirectoryInfo crrDir)
        {
            DriveGrid.Visibility = Visibility.Collapsed;
            propertyIcon.Source = new BitmapImage(new Uri(@"Images\folder_icon.png", UriKind.Relative));
            NameBar.Text = crrDir.Name;
            PathBar.Text = crrDir.FullName;
            ShowFolderSize(crrDir);
            CreatedBar.Text = Directory.GetCreationTime(crrDir.FullName).ToString();
            ChangedBar.Text = Directory.GetLastWriteTime(crrDir.FullName).ToString();
            AccessibleBar.Text = Directory.GetLastAccessTime(crrDir.FullName).ToString();

            if (crrDir.Attributes.HasFlag(FileAttributes.ReadOnly))
                ReadOnlyBox.IsChecked = true;
            else if (crrDir.Attributes.HasFlag(FileAttributes.Hidden))
                HiddenBox.IsChecked = true;
        }

        private void ShowFileProperties(FileViewModel fileViewModel)
        {
            FileInfo crrFile = new(fileViewModel.Path);
            DriveGrid.Visibility = Visibility.Collapsed;
            propertyIcon.Source = new BitmapImage(new Uri(@"Images\file_icon.png", UriKind.Relative));
            NameBar.Text = crrFile.Name;
            PathBar.Text = crrFile.FullName;
            ShowFileSize(crrFile);
            CreatedBar.Text = File.GetCreationTime(crrFile.FullName).ToString();
            ChangedBar.Text = File.GetLastWriteTime(crrFile.FullName).ToString();
            AccessibleBar.Text = File.GetLastAccessTime(crrFile.FullName).ToString();

            if (crrFile.Attributes.HasFlag(FileAttributes.ReadOnly))
                ReadOnlyBox.IsChecked = true;
            else if (crrFile.Attributes.HasFlag(FileAttributes.Hidden))
                HiddenBox.IsChecked = true;
        }

        private void ShowFolderSize(DirectoryInfo crrDir)
        {
            if (DirSize(crrDir) > bytesInGygabytesValue)
                SizeBar.Text = $"{(double)DirSize(crrDir) / bytesInGygabytesValue:0.0} Gb ({DirSize(crrDir):#,#} Bytes)";
            else
                SizeBar.Text = $"{(double)DirSize(crrDir) / bytesInMegabytesValue:0.0} Mb ({DirSize(crrDir):#,#} Bytes)";
        }

        private void ShowFileSize(FileInfo crrFile)
        {
            if (crrFile.Length > bytesInGygabytesValue)
                SizeBar.Text = $"{(double)crrFile.Length / bytesInGygabytesValue:0.0} Gb ({crrFile.Length:#,#} Bytes)";
            else
                SizeBar.Text = $"{(double)crrFile.Length / bytesInMegabytesValue:0.0} Mb ({crrFile.Length:#,#} Bytes)";
        }

        private static long DirSize(DirectoryInfo crrDir)
        {
            long size = 0;

            try
            {
                FileInfo[] files = crrDir.GetFiles();
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }

                DirectoryInfo[] folder = crrDir.GetDirectories();
                foreach (DirectoryInfo subDir in folder)
                {
                    size += DirSize(subDir);
                }
            }
            catch (Exception) { }

            return size;
        }
    }
}
