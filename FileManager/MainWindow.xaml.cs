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
using System.Windows.Media;

namespace FileManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DriveExplorer.ShowDrives(AddressBarLeft, ListBarLeft);
        }

        private void ListBarLeft_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBarLeft.SelectedItem == null) { }
            else if (Path.GetExtension(Path.Combine(AddressBarLeft.Text, ListBarLeft.SelectedItem.ToString())) == "")
            {
                AddressBarLeft.Text = Path.Combine(AddressBarLeft.Text, ListBarLeft.SelectedItem.ToString());

                FolderAndFileExplorer.ShowContentFolder(AddressBarLeft, ListBarLeft);
            }
            else
            {
                OpenFile();
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

        private void ListBarLeft_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListBarLeft.ContextMenu.Items.Clear();

            if (ListBarLeft.SelectedItem is DriveInfo)
            {
                ListBarLeft.ContextMenu.Items.Add(mi7);
            }
            else if (ListBarLeft.SelectedItem is DirectoryInfo)
            {
                ListBarLeft.ContextMenu.Items.Add(mi1);
                ListBarLeft.ContextMenu.Items.Add(mi2);
                ListBarLeft.ContextMenu.Items.Add(mi3);
                ListBarLeft.ContextMenu.Items.Add(mi4);
                ListBarLeft.ContextMenu.Items.Add(mi5);
                ListBarLeft.ContextMenu.Items.Add(mi6);
                ListBarLeft.ContextMenu.Items.Add(mi7);
            }
            else if (ListBarLeft.SelectedItem is FileInfo)
            {
                ListBarLeft.ContextMenu.Items.Add(mi1);
                ListBarLeft.ContextMenu.Items.Add(mi2);
                ListBarLeft.ContextMenu.Items.Add(mi3);
                ListBarLeft.ContextMenu.Items.Add(mi4);
                ListBarLeft.ContextMenu.Items.Add(mi5);
                ListBarLeft.ContextMenu.Items.Add(mi6);
                ListBarLeft.ContextMenu.Items.Add(mi7);
            }
            else if (ListBarLeft.SelectedItem is null)
            {
                ListBarLeft.ContextMenu.Items.Clear();
            }
        }

        private void ListBarLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (result.VisualHit.GetType() != typeof(ListBoxItem))
                ListBarLeft.UnselectAll();
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
                    if (AddressBarLeft.Text[^1] != '\\')
                    {
                        CleanUpToSlash();

                        FolderAndFileExplorer.ShowContentFolder(AddressBarLeft, ListBarLeft);
                    }
                    else if (AddressBarLeft.Text[^1] == '\\')
                    {
                        DriveExplorer.ShowDrives(AddressBarLeft, ListBarLeft);
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

            FolderAndFileExplorer.ShowContentFolder(AddressBarLeft, ListBarLeft);
        }

        private void CleanUpToSlash()
        {
            while (AddressBarLeft.Text[^1] != '\\')
            {
                AddressBarLeft.Text = AddressBarLeft.Text.Remove(AddressBarLeft.Text.Length - 1, 1);
            }
        }

        private void UpdateButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (!AddressBarLeft.Text.Contains('\\'))            
                DriveExplorer.ShowDrives(AddressBarLeft, ListBarLeft);            
            else if ((AddressBarLeft.Text.Contains('\\')))            
                FolderAndFileExplorer.ShowContentFolder(AddressBarLeft, ListBarLeft);           
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

                SearchInCurrentFolder(pathRoot, dir);
            }
        }

        private void SearchButtonLeft_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (AddressBarLeft.Text == "")
            {
                SearchButtonLeft.ToolTip = "Search Everywhere";
            }
            else
            {
                SearchButtonLeft.ToolTip = "Search in Current folder";
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

                    SearchInCurrentFolder(pathRoot, dir);
                }
            }
        }

        private void SearchInCurrentFolder(string pathRoot, DirectoryInfo dir)
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

                            SearchInCurrentFolder(path, subDir);
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


