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

            DriveExplorer driveExplorer = new();
            driveExplorer.ShowDriveList(AddressBarLeft, ListBarLeft);
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
                ListBarLeft.Items.Add(crrDir);
            }
        }

        private void ShowFilteredFiles(FileInfo[] files)
        {
            var filteredFiles = files.Where(crrFile => !crrFile.Attributes.HasFlag(FileAttributes.Hidden));

            foreach (FileInfo crrFile in filteredFiles)
            {
                ListBarLeft.Items.Add(crrFile);
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
                        CleanUpToSlash();

                        ShowContentFolder();
                    }
                    else if (AddressBarLeft.Text[AddressBarLeft.Text.Length - 1] == '\\')
                    {
                        DriveExplorer driveExplorer = new();
                        driveExplorer.ShowDriveList(AddressBarLeft, ListBarLeft);
                    }
                }
                else if (numberOfSlash > 1)
                {
                    ReturnToParentFolder();
                }
            }
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
            {
                DriveExplorer driveExplorer = new();
                driveExplorer.ShowDriveList(AddressBarLeft, ListBarLeft);
            }
            else if ((AddressBarLeft.Text.Contains('\\')))
                ShowContentFolder();
        }

        private void SearchButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            ListBarLeft.Items.Clear();

            string pathRoot;

            DirectoryInfo dir = null;

            if (AddressBarLeft.Text == "")
            {
                SearchEverywhere(dir);
            }
            else
            {
                pathRoot = AddressBarLeft.Text;
                
                RecursionSearch(pathRoot, dir);
            }
        }

        private void SearchEverywhere (DirectoryInfo dir)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    string pathRoot = drive.Name;

                    RecursionSearch(pathRoot, dir);
                }
            }
        }

        private void RecursionSearch(string pathRoot, DirectoryInfo dir)
        {
            DirectoryInfo searchFolder = new DirectoryInfo(pathRoot);
            DirectoryInfo[] subsSearchedDirectories = searchFolder.GetDirectories( $"*{SearchBarLeft.Text}*", SearchOption.TopDirectoryOnly);
            FileInfo[] SearchedFiles = searchFolder.GetFiles($"*{SearchBarLeft.Text}*", SearchOption.TopDirectoryOnly);
            DirectoryInfo[] subDirectoriesForSearch = searchFolder.GetDirectories();
            try
            {
                foreach (DirectoryInfo subDir in subsSearchedDirectories)
                {
                    ListBarLeft.Items.Add(subDir);
                }

                foreach (FileInfo file in SearchedFiles)
                {
                    ListBarLeft.Items.Add(file);
                }

                foreach (DirectoryInfo subDir in subDirectoriesForSearch)
                {
                    try
                    {
                        if (subDirectoriesForSearch.Length > 0)
                        {
                            string path = (Path.Combine(pathRoot, subDir.Name));

                            RecursionSearch(path, subDir);
                        }                       
                    }
                    catch (Exception) { }                    
                }
            }
            catch (Exception) { }

            return;
        }
    }
}


