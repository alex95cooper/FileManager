using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



namespace FileManager
{
    public partial class Property : Window
    {
        private readonly int bytesInGygabytesValue = 1_073_741_824;

        private readonly int bytesInMegabytesValue = 1_048_576;

        public Property()
        {
            InitializeComponent();
        }

        public void CategorySelector(ListView listBar, TextBox addressBar)
        {
            if (listBar.SelectedItem is DriveInfo)
            {
                DriveInfo drive = new(listBar.SelectedItem.ToString());
                ShowDriveProperties(drive);
            }
            else if (listBar.SelectedItem is DirectoryInfo && Directory.Exists(listBar.SelectedItem.ToString()))
            {
                DirectoryInfo crrDir = new(listBar.SelectedItem.ToString());
                ShowFolderProperties(crrDir);
            }
            else if (listBar.SelectedItem is FileInfo && File.Exists(listBar.SelectedItem.ToString()))
            {
                ShowFileProperties(listBar);
            }
            else if (!Directory.Exists(listBar.SelectedItem.ToString()) || !File.Exists(listBar.SelectedItem.ToString()))
            {
                MessageBox.Show("Current folder or file no longer exists!");
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
            propertyIcon.Source = (DrawingImage)Application.Current.TryFindResource("driveDrawingImage");
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
            propertyIcon.Source = (DrawingImage)Application.Current.TryFindResource("directoryDrawingImage");
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

        private void ShowFileProperties(ListView listBar)
        {
            FileInfo crrFile = new(listBar.SelectedItem.ToString());
            DriveGrid.Visibility = Visibility.Collapsed;
            propertyIcon.Source = (DrawingImage)Application.Current.TryFindResource("fileDrawingImage");
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
            if (DirSize(crrDir) > 1073741824)
                SizeBar.Text = $"{(double)DirSize(crrDir) / bytesInGygabytesValue:0.0} Gb ({DirSize(crrDir):#,#} Bytes)";
            else
                SizeBar.Text = $"{(double)DirSize(crrDir) / bytesInMegabytesValue:0.0} Mb ({DirSize(crrDir):#,#} Bytes)";
        }

        private void ShowFileSize(FileInfo crrFile)
        {
            if (crrFile.Length > 1073741824)
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
