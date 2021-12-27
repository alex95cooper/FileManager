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

        private void ListBarLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (result.VisualHit.GetType() != typeof(ListBoxItem))
                ListBarLeft.UnselectAll();
        }

        private void ListBarLeft_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListBarLeft.ContextMenu.Items.Clear();

            if (ListBarLeft.SelectedItem is DriveInfo)
            {
                ListBarLeft.ContextMenu.Items.Add(MenuItemProperties);
            }
            else if (ListBarLeft.SelectedItem is DirectoryInfo)
            {
                ListBarLeft.ContextMenu.Items.Add(MenuItemCut);
                ListBarLeft.ContextMenu.Items.Add(MenuItemCopy);
                ListBarLeft.ContextMenu.Items.Add(MenuItemInsert);
                ListBarLeft.ContextMenu.Items.Add(MenuItemRemove);
                ListBarLeft.ContextMenu.Items.Add(MenuItemRename);
                ListBarLeft.ContextMenu.Items.Add(MenuSeparator);
                ListBarLeft.ContextMenu.Items.Add(MenuItemProperties);
            }
            else if (ListBarLeft.SelectedItem is FileInfo)
            {
                ListBarLeft.ContextMenu.Items.Add(MenuItemCut);
                ListBarLeft.ContextMenu.Items.Add(MenuItemCopy);
                ListBarLeft.ContextMenu.Items.Add(MenuItemInsert);
                ListBarLeft.ContextMenu.Items.Add(MenuItemRemove);
                ListBarLeft.ContextMenu.Items.Add(MenuItemRename);
                ListBarLeft.ContextMenu.Items.Add(MenuSeparator);
                ListBarLeft.ContextMenu.Items.Add(MenuItemProperties);
            }
            else if (ListBarLeft.SelectedItem is null) { }
        }

        private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ListBarLeft.SelectedItem is DirectoryInfo)
            {
                string query = "Are you sure you want to remove the folder? \n" + ListBarLeft.SelectedItem + " ?";
                if (MessageBox.Show(query, "Remove the folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Directory.Delete(ListBarLeft.SelectedItem.ToString());
                    ListBarLeft.Items.Remove(ListBarLeft.SelectedItem);
                }

            }
            else if (ListBarLeft.SelectedItem is FileInfo)
            {
                string query = "Are you sure you want to remove the file? \n" + ListBarLeft.SelectedItem + " ?";
                if (MessageBox.Show(query, "Remove the file?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    File.Delete(ListBarLeft.SelectedItem.ToString());
                    ListBarLeft.Items.Remove(ListBarLeft.SelectedItem);
                }

            }
        }

        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            InputBox inputBox = new();

            if (inputBox.ShowDialog() == true)
            {
                string newName = inputBox.InputTextBox.Text;

                if (newName != "")
                {
                    if (newName.Contains("\\") || newName.Contains("/") || newName.Contains(":") || 
                        newName.Contains("*") || newName.Contains("?") || newName.Contains("\"") || 
                        newName.Contains("<") || newName.Contains(">") || newName.Contains("|"))
                    {
                        MessageBox.Show("The name must not contain the following characters: \\/:*?\"<>|");
                    }
                    else
                    {
                        if (ListBarLeft.SelectedItem is DirectoryInfo)
                        {
                            Directory.Move(ListBarLeft.SelectedItem.ToString(), Path.Combine(AddressBarLeft.Text, newName));
                        }
                        else if (ListBarLeft.SelectedItem is FileInfo)
                        {
                            File.Move(ListBarLeft.SelectedItem.ToString(), Path.Combine(AddressBarLeft.Text, newName));
                        }
                    }
                }
            }
            else { }
        }

        private void ReturnButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (!AddressBarLeft.Text.Contains('\\')) { }
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

        private void SearchEverywhere(DirectoryInfo dir)
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
            DirectoryInfo[] subsSearchedDirectories = searchFolder.GetDirectories($"*{SearchBarLeft.Text}*", SearchOption.TopDirectoryOnly);
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


