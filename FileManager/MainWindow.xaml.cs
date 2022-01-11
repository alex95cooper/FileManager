using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace FileManager
{
    public partial class MainWindow : Window
    {
        private string sourceFilePath, sourceFileName, dirToMoveName, sourceDirPath, sourceDirName, destinationPath;

        private int dirFileOperSelector;

        private bool isDragging, cancelIsDone;

        private Point clickPoint;

        public MainWindow()
        {
            InitializeComponent();

            SetUserPreferences();

            DriveExplorer.ShowDrives(AddressBarLeft, ListBarLeft);

            DriveExplorer.ShowDrives(AddressBarRight, ListBarRight);
        }

        private void SetUserPreferences()
        {
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
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

        private static void OpenListBarItem(ListView listBar, TextBlock addressBar)
        {
            if (listBar.SelectedItem is DriveInfo | listBar.SelectedItem is DirectoryInfo)
            {
                if (Directory.Exists(addressBar.Text) | listBar.SelectedItem is DriveInfo)
                    OpenDriveOrFolder(listBar, addressBar);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                if (File.Exists(listBar.SelectedItem.ToString()))
                    OpenFile(listBar, addressBar);
            }
            else { }
        }

        private static void OpenDriveOrFolder(ListView listBar, TextBlock addressBar)
        {
            addressBar.Text = listBar.SelectedItem.ToString();

            FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
        }

        private static void OpenFile(ListView listBar, TextBlock addressBar)
        {
            var openAnyFile = new Process
            {
                StartInfo = new ProcessStartInfo(listBar.SelectedItem.ToString())
                {
                    UseShellExecute = true
                }
            };

            openAnyFile.Start();
        }

        private void ListBarLeft_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListView listBar = ListBarLeft;

            GetMousePosition(listBar, e);
        }

        private void ListBarRight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListView listBar = ListBarRight;

            GetMousePosition(listBar, e);
        }

        private void GetMousePosition(ListView listBar, MouseButtonEventArgs e)
        {
            clickPoint = e.GetPosition(this);

            HitTestResult result = VisualTreeHelper.HitTest(this, clickPoint);
            if (result.VisualHit.GetType() != typeof(ListBoxItem))
                listBar.UnselectAll();
        }

        private void ListBarLeft_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

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
                listBar.ContextMenu.Items.Add(MenuItemDeleteLeft);
                listBar.ContextMenu.Items.Add(MenuItemRenameLeft);
                listBar.ContextMenu.Items.Add(MenuSeparatorLeft);
                listBar.ContextMenu.Items.Add(MenuItemPropertiesLeft);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemCutLeft);
                listBar.ContextMenu.Items.Add(MenuItemCopyLeft);
                listBar.ContextMenu.Items.Add(MenuItemDeleteLeft);
                listBar.ContextMenu.Items.Add(MenuItemRenameLeft);
                listBar.ContextMenu.Items.Add(MenuSeparatorLeft);
                listBar.ContextMenu.Items.Add(MenuItemPropertiesLeft);
            }
            else if (listBar.SelectedItem is null && addressBar.Text != "")
            {
                if (dirFileOperSelector != 0)
                    listBar.ContextMenu.Items.Add(MenuItemInsertLeft);
            }
        }

        private void ListBarRight_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            listBar.ContextMenu.Items.Clear();

            if (listBar.SelectedItem is DriveInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemPropertiesRight);
            }
            else if (listBar.SelectedItem is DirectoryInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemCutRight);
                listBar.ContextMenu.Items.Add(MenuItemCopyRight);
                listBar.ContextMenu.Items.Add(MenuItemDeleteRight);
                listBar.ContextMenu.Items.Add(MenuItemInsertRight);
                listBar.ContextMenu.Items.Add(MenuItemRenameRight);
                listBar.ContextMenu.Items.Add(MenuSeparatorRight);
                listBar.ContextMenu.Items.Add(MenuItemPropertiesRight);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                listBar.ContextMenu.Items.Add(MenuItemCutRight);
                listBar.ContextMenu.Items.Add(MenuItemCopyRight);
                listBar.ContextMenu.Items.Add(MenuItemDeleteRight);
                listBar.ContextMenu.Items.Add(MenuItemRenameRight);
                listBar.ContextMenu.Items.Add(MenuSeparatorRight);
                listBar.ContextMenu.Items.Add(MenuItemPropertiesRight);
            }
            else if (listBar.SelectedItem is null && addressBar.Text != "")
            {
                if (dirFileOperSelector != 0)
                    listBar.ContextMenu.Items.Add(MenuItemInsertRight);
            }
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
                GetFolderInformation(listBar, 1);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                GetFileInformation(listBar, addressBar, 2);
            }
        }

        private void GetFolderInformation(ListView listBar, int count)
        {
            sourceDirPath = listBar.SelectedItem.ToString();

            sourceDirName = new DirectoryInfo(sourceDirPath).Name;

            dirFileOperSelector = count;
        }

        private void GetFileInformation(ListView listBar, TextBlock addressBar, int count)
        {
            sourceFileName = Path.GetFileName(listBar.SelectedItem.ToString());

            sourceFilePath = Path.Combine(addressBar.Text, sourceFileName);

            dirFileOperSelector = count;
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
                GetFolderInformation(listBar, 3);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                GetFileInformation(listBar, addressBar, 4);
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
            if (addressBar.Text == string.Empty) { }
            else
            {
                if (dirFileOperSelector == 1 | dirFileOperSelector == 3)
                {
                    if (listBar.SelectedItem == null)
                        destinationPath = Path.Combine(addressBar.Text, sourceDirName);
                    else
                        destinationPath = Path.Combine(listBar.SelectedItem.ToString(), sourceDirName);

                    if (sourceDirPath == destinationPath)
                        MessageBox.Show($"Current folder already exict contains folder(s) named {sourceDirName}");
                    else
                        SelectFolderOperation(listBar, addressBar);
                }
                else if (dirFileOperSelector == 2 | dirFileOperSelector == 4)
                {
                    if (listBar.SelectedItem == null)
                        destinationPath = Path.Combine(addressBar.Text, sourceFileName);
                    else
                    {
                        dirToMoveName = new DirectoryInfo(listBar.SelectedItem.ToString()).Name;

                        destinationPath = Path.Combine(addressBar.Text, dirToMoveName, sourceFileName);
                    }

                    if (sourceFilePath == destinationPath)
                        MessageBox.Show($"Current folder already exict contains folder(s) named {sourceFileName}");
                    else
                        SelectFileOperation(listBar, addressBar);
                }
                else { }
            }         
        }

        private void SelectFolderOperation(ListView listBar, TextBlock addressBar)
        {
            try
            {
                if (dirFileOperSelector == 1)
                    Directory.Move(sourceDirPath, destinationPath);
                else if (dirFileOperSelector == 3)
                    DirectoryCopy(sourceDirPath, destinationPath);
            }
            catch (IOException)
            {
                MergeConflictingFolders(listBar, addressBar, sourceDirName);

                if (dirFileOperSelector == 1)
                    Directory.Delete(sourceDirPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectFileOperation(ListView listBar, TextBlock addressBar)
        {
            try
            {
                if (dirFileOperSelector == 2)
                    File.Move(sourceFilePath, destinationPath);
                else if (dirFileOperSelector == 4)
                    File.Copy(sourceFilePath, destinationPath);
            }
            catch (IOException)
            {
                ReplaceConflictingFile(listBar, addressBar, sourceFileName);

                SelectFileOperation(listBar, addressBar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DirectoryCopy(string dirForCopyPath, string destinationPath)
        {
            if (cancelIsDone) return;

            DirectoryInfo dir = new(dirForCopyPath);

            foreach (DirectoryInfo crrDir in dir.GetDirectories())
            {
                if (cancelIsDone) return;

                if (Directory.Exists(Path.Combine(destinationPath, crrDir.Name)) != true)
                    Directory.CreateDirectory(Path.Combine(destinationPath, crrDir.Name));

                DirectoryCopy(crrDir.FullName, Path.Combine(destinationPath, crrDir.Name));
            }

            foreach (string file in Directory.GetFiles(dirForCopyPath))
            {
                string fileName = Path.GetFileName(file);

                try
                {
                    File.Copy(file, Path.Combine(destinationPath, fileName));
                }
                catch (IOException)
                {
                    if (cancelIsDone) return;

                    MessageBoxResult result = MessageBox.Show($"Destination folder already contains file (s) named \"{fileName}\", would you like to replace it?",
                        "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            File.Delete(Path.Combine(destinationPath, fileName));
                            File.Copy(file, Path.Combine(destinationPath, fileName));
                            break;
                        case MessageBoxResult.No:
                            File.Delete(file);
                            break;
                        case MessageBoxResult.Cancel:
                            cancelIsDone = true;
                            break;
                    }
                }
            }
        }

        private void MergeConflictingFolders(ListView listBar, TextBlock addressBar, string sourseDirName)
        {
            string query = $"A folder named \"{sourseDirName}\" already exists, you want to merge conflicting folders?";
            if (MessageBox.Show(query, "Marge conflicting folders?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                destinationPath = Path.Combine(addressBar.Text, sourseDirName);

                DirectoryCopy(listBar.SelectedItem.ToString(), destinationPath);
                if (dirFileOperSelector == 1 | dirFileOperSelector == 3)
                    cancelIsDone = false;
            }
        }

        private void ReplaceConflictingFile(ListView listBar, TextBlock addressBar, string sourseFileName)
        {
            string query = $"Destination folder already contains file(s) named \"{sourseFileName}\", would you like to replace it?";
            if (MessageBox.Show(query, "Replace the file?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                destinationPath = Path.Combine(addressBar.Text, sourseFileName);

                File.Delete(destinationPath);
            }
        }

        private void MenuItemDeleteLeft_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarLeft;

            Delete(listBar);
        }

        private void MenuItemDeleteRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;

            Delete(listBar);
        }

        private static void Delete(ListView listBar)
        {
            try
            {
                if (listBar.SelectedItem is DirectoryInfo)
                {
                    string query = "Are you sure you want to delete the folder? \n" + listBar.SelectedItem + " ?";
                    if (MessageBox.Show(query, "Delete the folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Directory.Delete(listBar.SelectedItem.ToString(), true);
                        listBar.Items.Remove(listBar.SelectedItem);
                    }
                }
                else if (listBar.SelectedItem is FileInfo)
                {
                    string query = "Are you sure you want to delete the file? \n" + listBar.SelectedItem + " ?";
                    if (MessageBox.Show(query, "Delete the file?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
            dirFileOperSelector = 5;

            InputBox inputBox = new();

            if (inputBox.ShowDialog() == true & inputBox.InputTextBox.Text != null)
            {
                string newName = inputBox.InputTextBox.Text;

                if (newName.Contains("\\") || newName.Contains("/") || newName.Contains(":") ||
                    newName.Contains("*") || newName.Contains("?") || newName.Contains("\"") ||
                    newName.Contains("<") || newName.Contains(">") || newName.Contains("|"))
                    MessageBox.Show("The name must not contain the following characters: \\/:*?\"<>|");
                else
                {
                    if (ListBarLeft.SelectedItem is DirectoryInfo)
                    {
                        try
                        {
                            Directory.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
                        }
                        catch (IOException)
                        {
                            MergeConflictingFolders(listBar, addressBar, newName);
                            if (cancelIsDone)
                                cancelIsDone = false;
                            else
                                Directory.Delete(listBar.SelectedItem.ToString(), true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else if (listBar.SelectedItem is FileInfo)
                    {
                        try
                        {
                            if (Path.GetExtension(newName) == string.Empty)
                            {
                                string query = "After changing the extension, the file may become unavailable. Want to change it?";
                                if (MessageBox.Show(query, "Delete the folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                    File.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
                            }

                            File.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
                        }
                        catch (IOException)
                        {
                            ReplaceConflictingFile(listBar, addressBar, newName);
                            File.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
                            listBar.Items.Remove(listBar.SelectedItem);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
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

        private static void ShowProperties(ListView listBar)
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
                        ReturnToParentFolder(addressBar, listBar);
                    else if (addressBar.Text[^1] == '\\')
                        DriveExplorer.ShowDrives(addressBar, listBar);
                }
                else if (numberOfSlash > 1)
                {
                    ReturnToParentFolder(addressBar, listBar);
                }
            }
        }

        private static void ReturnToParentFolder(TextBlock addressBar, ListView listBar)
        {
            addressBar.Text = Path.GetDirectoryName(addressBar.Text);

            FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
        }

        private void SyncButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;
            addressBar.Text = AddressBarRight.Text;

            Update(listBar, addressBar);
        }

        private void SyncButtonRight_Click(object sender, RoutedEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;
            addressBar.Text = AddressBarLeft.Text;

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

        private static void Update(ListView listBar, TextBlock addressBar)
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
                SearchButtonLeft.ToolTip = "Search Everywhere";
            else
                SearchButtonLeft.ToolTip = "Search in Current folder";

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

        private void LeftItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
            {
                Point currentPosition = e.GetPosition(this);
                double distanceX = Math.Abs(clickPoint.X - currentPosition.X);
                double distanceY = Math.Abs(clickPoint.Y - currentPosition.Y);
                if (distanceX > 10 || distanceY > 10)
                {
                    ListView listBar = ListBarLeft;
                    TextBlock addressBar = AddressBarLeft;

                    if (frameworkElement.DataContext is DriveInfo)
                    {

                    }
                    else if (frameworkElement.DataContext is DirectoryInfo)
                    {
                        GetFolderInformation(listBar, 2);
                    }
                    else if (frameworkElement.DataContext is FileInfo)
                    {
                        GetFileInformation(listBar, addressBar, 4);
                    }

                    listBar.UnselectAll();

                    DragDrop.DoDragDrop(frameworkElement, new DataObject(DataFormats.Serializable, frameworkElement.DataContext), DragDropEffects.Move);
                }
            }
        }

        private void RightItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
            {
                Point currentPosition = e.GetPosition(this);
                double distanceX = Math.Abs(clickPoint.X - currentPosition.X);
                double distanceY = Math.Abs(clickPoint.Y - currentPosition.Y);
                if (distanceX > 10 || distanceY > 10)
                {
                    ListView listBar = ListBarRight;
                    TextBlock addressBar = AddressBarRight;

                    if (frameworkElement.DataContext is DriveInfo) { }
                    else if (frameworkElement.DataContext is DirectoryInfo)
                        GetFolderInformation(listBar, 2);
                    else if (frameworkElement.DataContext is FileInfo)
                        GetFileInformation(listBar, addressBar, 4);

                    listBar.UnselectAll();

                    DragDrop.DoDragDrop(frameworkElement, new DataObject(DataFormats.Serializable, frameworkElement.DataContext), DragDropEffects.Copy);
                }
            }
        }

        private void ListBarLeft_Drop(object sender, DragEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            try
            {
                Insert(listBar, addressBar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ListBarRight_Drop(object sender, DragEventArgs e)
        {

            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            try
            {
                Insert(listBar, addressBar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}


