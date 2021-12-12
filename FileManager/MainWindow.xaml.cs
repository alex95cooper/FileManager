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

            ShowDriveList();
        }

        private void ShowDriveList()
        {
            ListBarLeft.Items.Clear();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    ListBarLeft.Items.Add(drive);
                }
            }
        }

        private void ListBarLeft_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Path.GetExtension(Path.Combine(AddressBarLeft.Text, ListBarLeft.SelectedItem.ToString())) == "")
            {
                AddressBarLeft.Text = Path.Combine(AddressBarLeft.Text, ListBarLeft.SelectedItem.ToString());

                ShowContentFolder();
            }
            else
            {
                OpenFile();
            }
        }

        private void ShowContentFolder()
        {
            ListBarLeft.Items.Clear();

            DirectoryInfo dir = new(AddressBarLeft.Text);

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
                ListBarLeft.Items.Add(crrDir.Name);
            }
        }

        private void ShowFilteredFiles(FileInfo[] files)
        {
            var filteredFiles = files.Where(crrFile => !crrFile.Attributes.HasFlag(FileAttributes.Hidden));

            foreach (FileInfo crrFile in filteredFiles)
            {
                ListBarLeft.Items.Add(crrFile.Name);
            }
        }

        private void OpenFile()
        {
            var openAnyFile = new Process
            {
                StartInfo = new ProcessStartInfo(Path.Combine(AddressBarLeft.Text, ListBarLeft.SelectedItem.ToString()))
                {
                    UseShellExecute = true
                }
            };
            openAnyFile.Start();
        }

        private void ReturnButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (!AddressBarLeft.Text.Contains('\\'))
            {
                MessageBox.Show("There is nowhere to return");
            }
            else if (AddressBarLeft.Text.Contains('\\'))
            {
                int numberOfSlash = AddressBarLeft.Text.Count('\\'.Equals);

                if (numberOfSlash == 1)
                {
                    if (AddressBarLeft.Text[AddressBarLeft.Text.Length - 1] != '\\')
                    {
                        ReturnToDriveContent();
                    }
                    else if (AddressBarLeft.Text[AddressBarLeft.Text.Length - 1] == '\\')
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
            CleanUpToSlash();

            ShowContentFolder();
        }

        private void RetrnToDriveList()
        {
            AddressBarLeft.Text = null;

            ShowDriveList();
        }

        private void ReturnToParentFolder()
        {
            CleanUpToSlash();

            AddressBarLeft.Text = AddressBarLeft.Text.Remove(AddressBarLeft.Text.Length - 1, 1);

            ShowContentFolder();
        }

        private void CleanUpToSlash()
        {
            while (AddressBarLeft.Text[AddressBarLeft.Text.Length - 1] != '\\')
            {
                AddressBarLeft.Text = AddressBarLeft.Text.Remove(AddressBarLeft.Text.Length - 1, 1);
            }
        }

        private void UpdateButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (!AddressBarLeft.Text.Contains('\\'))
                ShowDriveList();
            else if ((AddressBarLeft.Text.Contains('\\')))
                ShowContentFolder();
        }

        private void SearchButtonLeft_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
