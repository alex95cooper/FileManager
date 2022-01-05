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

            OpenListBarItem(listBar, addressBar);
        }

        private void ListBarRight_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            OpenListBarItem(listBar, addressBar);
        }

        private void OpenListBarItem(ListView listBar, TextBlock addressBar)
        {
            if (listBar.SelectedItem == null) { }
            else if (Path.GetExtension(Path.Combine(addressBar.Text, listBar.SelectedItem.ToString())) == "")
            {
                addressBar.Text = Path.Combine(addressBar.Text, listBar.SelectedItem.ToString());

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

        private void MenuItemCutLeft_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            Cut(listBar, addressBar);
        }

        private void MenuItemCutRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            Cut(listBar, addressBar);
        }

        private void Cut(ListView listBar, TextBlock addressBar)
        {
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

            Copy(listBar, addressBar);
        }

        private void MenuItemCopyRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            Copy(listBar, addressBar);
        }

        private void Copy(ListView listBar, TextBlock addressBar)
        {
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
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            Insert(listBar, addressBar);
        }

        private void MenuItemInsertRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            Insert(listBar, addressBar);
        }

        private void Insert(ListView listBar, TextBlock addressBar)
        {
            try
            {
                if (dirFileOperSelector == 1)
                {
                    string destinationDirectory = Path.Combine(listBar.SelectedItem.ToString(), dirForMoveName);

                    Directory.Move(dirForMovePath, destinationDirectory);
                }
                else if (dirFileOperSelector == 2)
                {
                    string dirToMoveName = new DirectoryInfo(listBar.SelectedItem.ToString()).Name;

                    string destnationFile = Path.Combine(addressBar.Text, dirToMoveName, fileName);

                    File.Move(filePath, destnationFile);

                    listBar.Items.Remove(listBar.SelectedItem);
                }
                else if (dirFileOperSelector == 3)
                {
                    string createdDirPath = Path.Combine(listBar.SelectedItem.ToString(), dirForCopyName);

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
                    string dirToCopyName = new DirectoryInfo(listBar.SelectedItem.ToString()).Name;

                    string destnationFile = Path.Combine(addressBar.Text, dirToCopyName, fileName);

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
            ListView listBar = ListBarLeft;

            Remove(listBar);
        }

        private void MenuItemRemoveRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;

            Remove(listBar);
        }

        private void Remove(ListView listBar)
        {
            try
            {
                if (listBar.SelectedItem is DirectoryInfo)
                {
                    string query = "Are you sure you want to remove the folder? \n" + listBar.SelectedItem + " ?";
                    if (MessageBox.Show(query, "Remove the folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Directory.Delete(listBar.SelectedItem.ToString(), true);
                        listBar.Items.Remove(listBar.SelectedItem);
                    }
                }
                else if (listBar.SelectedItem is FileInfo)
                {
                    string query = "Are you sure you want to remove the file? \n" + listBar.SelectedItem + " ?";
                    if (MessageBox.Show(query, "Remove the file?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        File.Delete(listBar.SelectedItem.ToString());
                        listBar.Items.Remove(listBar.SelectedItem);
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
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            Rename(listBar, addressBar);
        }

        private void MenuItemRenameRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            Rename(listBar, addressBar);
        }

        private void Rename(ListView listBar, TextBlock addressBar)
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
                            Directory.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
                        }
                        else if (listBar.SelectedItem is FileInfo)
                        {
                            File.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
                        }
                    }
                }
            }
            else { }
        }

        private void MenuItemPropertiesLeft_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarLeft;

            ShowProperties(listBar);
        }

        private void MenuItemPropertiesRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;

            ShowProperties(listBar);
        }

        private void ShowProperties(ListView listBar)
        {
            Property properties = new();
            properties.CategorySelector(listBar);
            properties.ShowDialog();
        }

        private void ReturnButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            SelectPath(listBar, addressBar);
        }

        private void ReturnButtonRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            SelectPath(listBar, addressBar);
        }

        private void SelectPath(ListView listBar, TextBlock addressBar)
        {
            if (!addressBar.Text.Contains('\\')) { }
            else if (addressBar.Text.Contains('\\'))
            {
                int numberOfSlash = addressBar.Text.Count('\\'.Equals);

                if (numberOfSlash == 1)
                {
                    if (addressBar.Text[^1] != '\\')
                    {
                        ReturnToParentFolder(addressBar, listBar);
                    }
                    else if (addressBar.Text[^1] == '\\')
                    {
                        DriveExplorer.ShowDrives(addressBar, listBar);
                    }
                }
                else if (numberOfSlash > 1)
                {
                    ReturnToParentFolder(addressBar, listBar);
                }
            }
        }

        private void ReturnToParentFolder(TextBlock addressBar, ListView listBar)
        {
            addressBar.Text = Path.GetDirectoryName(addressBar.Text);

            FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
        }

        private void SyncButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarRight;

            Update(listBar, addressBar);
        }

        private void SyncButtonRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarLeft;

            Update(listBar, addressBar);
        }

        private void UpdateButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            Update(listBar, addressBar);
        }

        private void UpdateButtonRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            Update(listBar, addressBar);
        }

        private void Update(ListView listBar, TextBlock addressBar)
        {
            if (!addressBar.Text.Contains('\\'))
                DriveExplorer.ShowDrives(addressBar, listBar);
            else if ((addressBar.Text.Contains('\\')))
                FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
        }

        private void SearchButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            Searcher.Search(AddressBarLeft, ListBarLeft, SearchBarLeft);
        }

        private void SearchButtonRight_Click(object sender, RoutedEventArgs e)
        {
            Searcher.Search(AddressBarRight, ListBarRight, SearchBarRight);
        }

        private void SearchButtonLeft_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            TextBlock addressBar = AddressBarLeft;

            SelectToolTip(addressBar);
        }

        private void SearchButtonRight_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            TextBlock addressBar = AddressBarRight;

            SelectToolTip(addressBar);
        }

        private void SelectToolTip(TextBlock addressBar)
        {
            if (addressBar.Text == "")
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


