using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace FileManager
{
    public partial class Property : Window
    {
        public Property()
        {
            InitializeComponent();
        }

        public void CategorySelector(ListView listBar)
        {
            if (listBar.SelectedItem is DriveInfo drive)
            {
                FolderandFileGrid.Visibility = Visibility.Collapsed;
                propertyIcon.Source = (DrawingImage)Application.Current.TryFindResource("driveDrawingImage");
                NameBar.Text = drive.Name;
                TypeBar.Text = drive.DriveType.ToString();
                FileSystemBar.Text = drive.DriveFormat.ToString();
                BusyBar.Text = $"{drive.TotalSize - drive.TotalFreeSpace:#,#} Bytes  or  {(double)(drive.TotalSize - drive.TotalFreeSpace) / 1073741824:0.0} Gb";
                FreeBar.Text = $"{drive.TotalFreeSpace:#,#} Bytes  or  {(double)drive.TotalFreeSpace / 1073741824:0.0} Gb";
                CapacityBar.Text = $"{drive.TotalSize:#,#} Bytes  or  {(double)drive.TotalSize / 1073741824:0.0} Gb";
            }
            else if (listBar.SelectedItem is DirectoryInfo crrDir)
            {
                DriveGrid.Visibility = Visibility.Collapsed;
                propertyIcon.Source = (DrawingImage)Application.Current.TryFindResource("directoryDrawingImage");
                NameBar.Text = crrDir.Name;
                PathBar.Text = crrDir.FullName;

                if (DirSize(crrDir) > 1073741824)                
                    SizeBar.Text = $"{(double)DirSize(crrDir) / 1073741824:0.0} Gb ({DirSize(crrDir):#,#} Bytes)"; 
                else
                    SizeBar.Text = $"{(double)DirSize(crrDir) / 1048576:0.0} Mb ({DirSize(crrDir):#,#} Bytes)";

                CreatedBar.Text = Directory.GetCreationTime(crrDir.FullName).ToString();
                ChangedBar.Text = Directory.GetLastWriteTime(crrDir.FullName).ToString();
                AccessibleBar.Text = Directory.GetLastAccessTime(crrDir.FullName).ToString();
            }
            else if (listBar.SelectedItem is FileInfo crrFile)
            {
                DriveGrid.Visibility = Visibility.Collapsed;                
                propertyIcon.Source = (DrawingImage)Application.Current.TryFindResource("fileDrawingImage");
                NameBar.Text = crrFile.Name;
                PathBar.Text = crrFile.FullName;

                if (crrFile.Length > 1073741824)
                    SizeBar.Text = $"{(double)crrFile.Length / 10737418240:0.0} Gb ({crrFile.Length:#,#} Bytes)";
                else
                    SizeBar.Text = $"{(double)crrFile.Length / 1048576:0.0} Mb ({crrFile.Length:#,#} Bytes)";

                CreatedBar.Text = File.GetCreationTime(crrFile.FullName).ToString();
                ChangedBar.Text = File.GetLastWriteTime(crrFile.FullName).ToString();
                AccessibleBar.Text = File.GetLastAccessTime(crrFile.FullName).ToString();

            }
        }

        public static long DirSize(DirectoryInfo crrDir)
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
