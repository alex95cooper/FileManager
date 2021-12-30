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

        private string filePath, fileName, dirForCopyPath, dirForCopyName, dirForMovePath, dirForMoveName;

        private int dirFileOperSelector;

        private string[] files;

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
                OpenFile(AddressBarLeft, ListBarLeft);
            }
        }

        private void OpenFile(TextBlock addressBar, ListView listBar)
        {
            var openAnyFile = new Process
            {
                StartInfo = new ProcessStartInfo(Path.Combine(addressBar.Text, listBar.SelectedItem.ToString()))
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
                ListBarLeft.ContextMenu.Items.Add(MenuItemRemove);
                ListBarLeft.ContextMenu.Items.Add(MenuItemRename);
                ListBarLeft.ContextMenu.Items.Add(MenuSeparator);
                ListBarLeft.ContextMenu.Items.Add(MenuItemProperties);
            }
            else if (ListBarLeft.SelectedItem is null) { }
        }

        private void MenuItemCut_Click(object sender, RoutedEventArgs e)
        {
            if (ListBarLeft.SelectedItem is DirectoryInfo)
            {                
                dirForMovePath = ListBarLeft.SelectedItem.ToString();

                dirForMoveName = new DirectoryInfo(dirForMovePath).Name;

                dirFileOperSelector = 1;
            }
            else if (ListBarLeft.SelectedItem is FileInfo)
            {
                fileName = Path.GetFileName(ListBarLeft.SelectedItem.ToString());

                filePath = Path.Combine(AddressBarLeft.Text, fileName);

                dirFileOperSelector = 2;
            }
        }

        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            if (ListBarLeft.SelectedItem is DirectoryInfo)
            {
                dirForCopyPath = ListBarLeft.SelectedItem.ToString();

                dirForCopyName = new DirectoryInfo(dirForCopyPath).Name;

                files = Directory.GetFiles(dirForCopyPath);

                dirFileOperSelector = 3;
            }
            else if (ListBarLeft.SelectedItem is FileInfo)
            {
                fileName = Path.GetFileName(ListBarLeft.SelectedItem.ToString());

                filePath = Path.Combine(AddressBarLeft.Text, fileName);

                dirFileOperSelector = 4;
            }

        }

        private void MenuItemInsert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dirFileOperSelector == 1)
                {
                    string destinationDirectory = Path.Combine(ListBarLeft.SelectedItem.ToString(), dirForMoveName);

                    Directory.Move(dirForMovePath, destinationDirectory);
                }
                else if (dirFileOperSelector == 2)
                {
                    string dirToMoveName = new DirectoryInfo(ListBarLeft.SelectedItem.ToString()).Name;
                    string destnationFile = Path.Combine(AddressBarLeft.Text, dirToMoveName, fileName);
                    File.Move(filePath, destnationFile);
                    ListBarLeft.Items.Remove(ListBarLeft.SelectedItem);
                }
                else if (dirFileOperSelector == 3)
                {
                    string createdDirPath = Path.Combine(ListBarLeft.SelectedItem.ToString(), dirForCopyName);
                    Directory.CreateDirectory(createdDirPath);
                    foreach (string el in files)
                    {
                        fileName = Path.GetFileName(el);
                        string destnationFiles = Path.Combine(createdDirPath, fileName);
                        File.Copy(el, destnationFiles);
                    }
                }
                else if (dirFileOperSelector == 4)
                {
                    string dirToCopyName = new DirectoryInfo(ListBarLeft.SelectedItem.ToString()).Name;
                    string destnationFile = Path.Combine(AddressBarLeft.Text, dirToCopyName, fileName);
                    File.Copy(filePath, destnationFile);
                }
                else { }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListBarLeft.SelectedItem is DirectoryInfo)
                {
                    string query = "Are you sure you want to remove the folder? \n" + ListBarLeft.SelectedItem + " ?";
                    if (MessageBox.Show(query, "Remove the folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Directory.Delete(ListBarLeft.SelectedItem.ToString(), true);
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
            catch (Exception ex)
            {
                MessageBox.Show("Unable to delete file: " + ex.Message);
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
                        ReturnToParentFolder(AddressBarLeft, ListBarLeft);
                    }
                    else if (AddressBarLeft.Text[^1] == '\\')
                    {
                        DriveExplorer.ShowDrives(AddressBarLeft, ListBarLeft);
                    }
                }
                else if (numberOfSlash > 1)
                {
                    ReturnToParentFolder(AddressBarLeft, ListBarLeft);
                }
            }
        }

        private void ReturnToParentFolder(TextBlock addressBar, ListView listBar)
        {
            addressBar.Text = Path.GetDirectoryName(addressBar.Text);

            FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
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

            Searcher.Search(AddressBarLeft, ListBarLeft, SearchBarLeft);
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


    }
}


