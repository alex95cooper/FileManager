using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System.Windows.Data;

namespace FileManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ListBar_View();
        }

        private void ListBar_View()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    ListBar.Items.Add(drive);
                }
            }
        }

        private void ListBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Path.GetExtension(Path.Combine(AddressBar.Text, ListBar.SelectedItem.ToString())) == "")
            {
                AddressBar.Text = Path.Combine(AddressBar.Text, ListBar.SelectedItem.ToString());

                ShowContentFolder();
            }
            else
            {
                OpenFile();
            }
        }

        private void ShowContentFolder()
        {
            ListBar.Items.Clear();

            DirectoryInfo dir = new(AddressBar.Text);

            ShowFolders(dir);

            ShowFiles(dir);
        }

        private void ShowFolders(DirectoryInfo dir)
        {
            DirectoryInfo[] dirs = dir.GetDirectories();

            ShowFilteredFolders(dirs);
        }

        private void ShowFiles(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles();

            ShowFilteredFiles(files);
        }

        private void ShowFilteredFolders(DirectoryInfo[] dirs)
        {
            var filteredDirs = dirs.Where(crrDir => !crrDir.Attributes.HasFlag(FileAttributes.Hidden));

            foreach (DirectoryInfo crrDir in filteredDirs)
            {
                ListBar.Items.Add(crrDir.Name);
            }
        }

        private void ShowFilteredFiles(FileInfo[] files)
        {
            var filteredFiles = files.Where(crrFile => !crrFile.Attributes.HasFlag(FileAttributes.Hidden));

            foreach (FileInfo crrFile in filteredFiles)
            {
                ListBar.Items.Add(crrFile.Name);
            }
        }

        private void OpenFile()
        {
            var openAnyFile = new Process
            {
                StartInfo = new ProcessStartInfo(Path.Combine(AddressBar.Text, ListBar.SelectedItem.ToString()))
                {
                    UseShellExecute = true
                }
            };
            openAnyFile.Start();
        }

        private void ReturnButton1_Click(object sender, RoutedEventArgs e)
        {
            if (!AddressBar.Text.Contains('\\'))
            {
                MessageBox.Show("There is nowhere to return");
            }
            else if (AddressBar.Text.Contains('\\'))
            {
                int numberOfSlash = AddressBar.Text.Count('\\'.Equals);

                if (numberOfSlash == 1)
                {
                    if (AddressBar.Text[AddressBar.Text.Length - 1] != '\\')
                    {
                        ReturnToDriveContent();
                    }
                    else if (AddressBar.Text[AddressBar.Text.Length - 1] == '\\')
                    {
                        RetrnToDriveList();
                    }
                }
                else if (numberOfSlash > 1)
                {
                    ReturnToParentFolder();
                }
            }
        }

        private void ReturnToDriveContent()
        {
            CleanToSlash();

            ShowContentFolder();
        }

        private void RetrnToDriveList()
        {
            AddressBar.Text = null;

            ListBar.Items.Clear();

            ListBar_View();
        }

        private void ReturnToParentFolder()
        {
            CleanToSlash();

            AddressBar.Text = AddressBar.Text.Remove(AddressBar.Text.Length - 1, 1);

            ShowContentFolder();
        }

        private void CleanToSlash()
        {
            while (AddressBar.Text[AddressBar.Text.Length - 1] != '\\')
            {
                AddressBar.Text = AddressBar.Text.Remove(AddressBar.Text.Length - 1, 1);
            }
        }

        private void UpdateButton1_Click(object sender, RoutedEventArgs e)
        {
            ShowContentFolder();
        }

    }
}
