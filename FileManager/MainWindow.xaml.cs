using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;

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

            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }

            DriveExplorer.ShowDrives(AddressBarLeft, ListBarLeft);

            DriveExplorer.ShowDrives(AddressBarRight, ListBarRight);
        }

        private void ListBarLeft_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            if (listBar.SelectedItem == null) { }
            else if (Path.GetExtension(Path.Combine(addressBar.Text, listBar.SelectedItem.ToString())) == "")
            {
                AddressBarLeft.Text = Path.Combine(addressBar.Text, listBar.SelectedItem.ToString());

                FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
            }
            else
            {
                OpenFile(AddressBarLeft, listBar);
            }
        }

        private void ListBarRight_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            if (listBar.SelectedItem == null) { }
            else if (Path.GetExtension(Path.Combine(addressBar.Text, listBar.SelectedItem.ToString())) == "")
            {
                AddressBarLeft.Text = Path.Combine(addressBar.Text, listBar.SelectedItem.ToString());

                FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
            }
            else
            {
                OpenFile(AddressBarLeft, listBar);
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

        private void ListBarRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (result.VisualHit.GetType() != typeof(ListBoxItem))
                ListBarRight.UnselectAll();
        }

        private void ListBarLeft_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListView listBar = ListBarLeft;

            listBar.ContextMenu.Items.Clear();

            if (listBar.SelectedItem is DriveInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemPropertiesLeft);
            }
            else if (listBar.SelectedItem is DirectoryInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemCutLeft);
                listBar.ContextMenu.Items.Add(MenuItemCopyLeft);
                listBar.ContextMenu.Items.Add(MenuItemInsertLeft);
                listBar.ContextMenu.Items.Add(MenuItemRemoveLeft);
                listBar.ContextMenu.Items.Add(MenuItemRenameLeft);
                listBar.ContextMenu.Items.Add(MenuSeparatorLeft);
                listBar.ContextMenu.Items.Add(MenuItemPropertiesLeft);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemCutLeft);
                listBar.ContextMenu.Items.Add(MenuItemCopyLeft);
                listBar.ContextMenu.Items.Add(MenuItemRemoveLeft);
                listBar.ContextMenu.Items.Add(MenuItemRenameLeft);
                listBar.ContextMenu.Items.Add(MenuSeparatorLeft);
                listBar.ContextMenu.Items.Add(MenuItemPropertiesLeft);
            }
            else if (listBar.SelectedItem is null) { }
        }

        private void MenuItemCutLeft_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            if (listBar.SelectedItem is DirectoryInfo)
            {
                dirForMovePath = listBar.SelectedItem.ToString();

                dirForMoveName = new DirectoryInfo(dirForMovePath).Name;

                dirFileOperSelector = 1;
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                fileName = Path.GetFileName(listBar.SelectedItem.ToString());

                filePath = Path.Combine(addressBar.Text, fileName);

                dirFileOperSelector = 2;
            }
        }

        private void MenuItemCopyLeft_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            if (listBar.SelectedItem is DirectoryInfo)
            {
                dirForCopyPath = listBar.SelectedItem.ToString();

                dirForCopyName = new DirectoryInfo(dirForCopyPath).Name;

                files = Directory.GetFiles(dirForCopyPath);

                dirFileOperSelector = 3;
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                fileName = Path.GetFileName(listBar.SelectedItem.ToString());

                filePath = Path.Combine(addressBar.Text, fileName);

                dirFileOperSelector = 4;
            }
        }

        private void MenuItemInsertLeft_Click(object sender, RoutedEventArgs e)
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

        private void MenuItemRemoveLeft_Click(object sender, RoutedEventArgs e)
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

        private void MenuItemRenameLeft_Click(object sender, RoutedEventArgs e)
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

        private void MenuItemPropertiesLeft_Click(object sender, RoutedEventArgs e)
        {
            Property properties = new();
            properties.CategorySelector(ListBarLeft);
            properties.ShowDialog();
        }

        private void ListBarRight_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListView listBar = ListBarRight;

            listBar.ContextMenu.Items.Clear();

            if (listBar.SelectedItem is DriveInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemPropertiesRight);
            }
            else if (listBar.SelectedItem is DirectoryInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemCutRight);
                listBar.ContextMenu.Items.Add(MenuItemCopyRight);
                listBar.ContextMenu.Items.Add(MenuItemInsertRight);
                listBar.ContextMenu.Items.Add(MenuItemRemoveRight);
                listBar.ContextMenu.Items.Add(MenuItemRenameRight);
                listBar.ContextMenu.Items.Add(MenuSeparatorRight);
                listBar.ContextMenu.Items.Add(MenuItemPropertiesRight);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemCutRight);
                listBar.ContextMenu.Items.Add(MenuItemCopyRight);
                listBar.ContextMenu.Items.Add(MenuItemRemoveRight);
                listBar.ContextMenu.Items.Add(MenuItemRenameRight);
                listBar.ContextMenu.Items.Add(MenuSeparatorRight);
                listBar.ContextMenu.Items.Add(MenuItemPropertiesRight);
            }
            else if (listBar.SelectedItem is null) { }
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();
        }
    }
}


