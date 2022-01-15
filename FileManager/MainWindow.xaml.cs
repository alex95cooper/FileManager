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
        private string sourceFilePath, sourceFileName, sourceDirPath, sourceDirName, destinationPath;

        private int dirFileOperSelector;

        private bool cancelIsDone;

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
                if (Directory.Exists(listBar.SelectedItem.ToString()) | listBar.SelectedItem is DriveInfo)
                    OpenDriveOrFolder(listBar, addressBar);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                if (File.Exists(listBar.SelectedItem.ToString()))
                    OpenFile(listBar, addressBar);
            }
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
                if (Directory.Exists(listBar.SelectedItem.ToString()))
                    GetFolderInformation(listBar, 1);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                if (File.Exists(listBar.SelectedItem.ToString()))
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
                if (Directory.Exists(listBar.SelectedItem.ToString()))
                    GetFolderInformation(listBar, 3);
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                if (File.Exists(listBar.SelectedItem.ToString()))
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
                    GetDestinationPath(listBar, addressBar, sourceDirName);

                    if (sourceDirPath == destinationPath) { }
                    else
                        FolderExeptionCatch(listBar, addressBar);
                }
                else if (dirFileOperSelector == 2 | dirFileOperSelector == 4)
                {
                    GetDestinationPath(listBar, addressBar, sourceFileName);

                    if (sourceFilePath == destinationPath) { }
                    else
                        FileExeptionCatch(listBar, addressBar);
                }
            }
        }

        private void GetDestinationPath(ListView listBar, TextBlock addressBar, string sourceName)
        {
            if (listBar.SelectedItem == null | listBar.SelectedItem is FileInfo)
                destinationPath = Path.Combine(addressBar.Text, sourceName);
            else if (listBar.SelectedItem is DirectoryInfo)
            {
                if (sourceDirPath == listBar.SelectedItem.ToString())
                    destinationPath = sourceDirPath;
                else
                    destinationPath = Path.Combine(listBar.SelectedItem.ToString(), sourceName);
            }
        }

        private void FolderExeptionCatch(ListView listBar, TextBlock addressBar)
        {
            try
            {
                SelectFolderOperation(listBar, addressBar);
            }
            catch (IOException)
            {
                HandleFolderException(listBar, addressBar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectFolderOperation(ListView listBar, TextBlock addressBar)
        {
            if (dirFileOperSelector == 1)
                Directory.Move(sourceDirPath, destinationPath);
            else if (dirFileOperSelector == 3)
            {
                if (Directory.Exists(destinationPath))
                {
                    HandleFolderException(listBar, addressBar);
                }
            }
        }

        private void HandleFolderException(ListView listBar, TextBlock addressBar)
        {
            MergeConflictingFolders(listBar, addressBar, sourceDirName);
            if (!cancelIsDone)
            {
                if (dirFileOperSelector == 1)
                    Directory.Delete(sourceDirPath, true);
                else if (dirFileOperSelector == 3)
                    DirectoryCopy(sourceDirPath, destinationPath);
            }
            else cancelIsDone = false;
        }

        private void FileExeptionCatch(ListView listBar, TextBlock addressBar)
        {
            try
            {
                SelectFileOperation();
            }
            catch (IOException)
            {
                ReplaceConflictingFile(listBar, addressBar, sourceFileName);
                if (!cancelIsDone)
                    SelectFileOperation();
                else cancelIsDone = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectFileOperation()
        {
            if (dirFileOperSelector == 2)
                File.Move(sourceFilePath, destinationPath);
            else if (dirFileOperSelector == 4)
                File.Copy(sourceFilePath, destinationPath);
        }

        private void DirectoryCopy(string dirForCopyPath, string destinationDirPath)
        {
            if (cancelIsDone) return;

            DirectoryInfo dir = new(dirForCopyPath);

            foreach (DirectoryInfo crrDir in dir.GetDirectories())
            {
                if (cancelIsDone) return;

                if (Directory.Exists(Path.Combine(destinationDirPath, crrDir.Name)) != true)
                    Directory.CreateDirectory(Path.Combine(destinationDirPath, crrDir.Name));

                DirectoryCopy(crrDir.FullName, Path.Combine(destinationDirPath, crrDir.Name));
            }

            foreach (string file in Directory.GetFiles(dirForCopyPath))
            {
                string fileName = Path.GetFileName(file);

                try
                {
                    if (cancelIsDone) return;
                    File.Copy(file, Path.Combine(destinationDirPath, fileName));
                }
                catch (IOException)
                {
                    if (cancelIsDone) return;

                    ShowSubFileMoveExMessage(file, fileName, destinationDirPath);
                }
            }
        }

        private void ShowSubFileMoveExMessage(string file, string fileName, string destinationDirPath)
        {
            MessageBoxResult result = MessageBox.Show($"Destination folder already contains file (s) named \"{fileName}\", would you like to replace it?",
                "Replace file",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    File.Delete(Path.Combine(destinationDirPath, fileName));
                    File.Copy(file, Path.Combine(destinationDirPath, fileName));
                    break;
                case MessageBoxResult.No:
                    if (dirFileOperSelector == 3) { }
                    else
                        File.Delete(file);
                    break;
                case MessageBoxResult.Cancel:
                    cancelIsDone = true;
                    break;
            }
        }

        private void MergeConflictingFolders(ListView listBar, TextBlock addressBar, string sourseDirName)
        {
            string query = $"A folder named \"{sourseDirName}\" already exists, you want to merge conflicting folders?";
            if (MessageBox.Show(query, "Marge conflicting folders?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                destinationPath = Path.Combine(addressBar.Text, sourseDirName);

                if (dirFileOperSelector == 1 | dirFileOperSelector == 3)
                    DirectoryCopy(sourceDirPath, destinationPath);
                else
                    DirectoryCopy(listBar.SelectedItem.ToString(), destinationPath);
            }
            else cancelIsDone = true;
        }

        private void ReplaceConflictingFile(ListView listBar, TextBlock addressBar, string sourseFileName)
        {
            string query = $"Destination folder already contains file(s) named \"{sourseFileName}\", would you like to replace it?";
            if (MessageBox.Show(query, "Replace the file?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                destinationPath = Path.Combine(addressBar.Text, sourseFileName);
                File.Delete(destinationPath);
            }
            else
                cancelIsDone = true;
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
                    ShowMessageToDeleteFolder(listBar);
                }
                else if (listBar.SelectedItem is FileInfo)
                {
                    ShowMessageToDeleteFile(listBar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to delete file: " + ex.Message);
            }
        }

        private static void ShowMessageToDeleteFolder(ListView listBar)
        {
            string query = "Are you sure you want to delete the folder? \n" + listBar.SelectedItem + " ?";
            if (MessageBox.Show(query, "Delete the folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Directory.Delete(listBar.SelectedItem.ToString(), true);
                listBar.Items.Remove(listBar.SelectedItem);
            }
        }

        private static void ShowMessageToDeleteFile(ListView listBar)
        {
            string query = "Are you sure you want to delete the file? \n" + listBar.SelectedItem + " ?";
            if (MessageBox.Show(query, "Delete the file?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                File.Delete(listBar.SelectedItem.ToString());
                listBar.Items.Remove(listBar.SelectedItem);
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

            if (inputBox.ShowDialog() == true & inputBox.InputTextBox.Text != null)
            {
                string newName = inputBox.InputTextBox.Text;
                string[] unacceptableSymbols = new string[9] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };

                if (unacceptableSymbols.Any(newName.Contains))
                    MessageBox.Show("The name must not contain the following characters: \\/:*?\"<>|");
                else
                {
                    if (listBar.SelectedItem is DirectoryInfo)
                    {
                        RenameFolder(listBar, addressBar, newName);
                    }
                    else if (listBar.SelectedItem is FileInfo)
                    {
                        RenameFile(listBar, addressBar, newName);
                    }
                }
            }
            else { }
        }

        private void RenameFolder(ListView listBar, TextBlock addressBar, string newName)
        {
            try
            {
                Directory.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
            }
            catch (IOException)
            {
                MergeConflictingFolders(listBar, addressBar, newName);
                if (!cancelIsDone)
                    Directory.Delete(listBar.SelectedItem.ToString(), true);
                else
                    cancelIsDone = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RenameFile(ListView listBar, TextBlock addressBar, string newName)
        {
            try
            {
                ShowMessageOfFileWithoutExtension(listBar, addressBar, newName);
                if (!cancelIsDone)
                    File.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
            }
            catch (IOException)
            {
                ReplaceConflictingFile(listBar, addressBar, newName);
                if (!cancelIsDone)
                    File.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            cancelIsDone = false;
        }

        private void ShowMessageOfFileWithoutExtension(ListView listBar, TextBlock addressBar, string newName)
        {
            if (Path.GetExtension(newName) == string.Empty)
            {
                string query = "After changing the extension, the file may become unavailable. Want to change it?";
                if (MessageBox.Show(query, "Delete the folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    File.Move(listBar.SelectedItem.ToString(), Path.Combine(addressBar.Text, newName));
                else
                    cancelIsDone = true;
            }
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

        private void LeftItem_MouseMove(object sender, MouseEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;

            DragMouseMove(sender, e, listBar, addressBar);
        }

        private void RightItem_MouseMove(object sender, MouseEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;

            DragMouseMove(sender, e, listBar, addressBar);
        }

        private void DragMouseMove(object sender, MouseEventArgs e, ListView listBar, TextBlock addressBar)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
            {
                MouseMoveDetermine(e, out double distanceX, out double distanceY);
                if (distanceX > 10 || distanceY > 10)
                {
                    DragCutOrCopySelect(frameworkElement, listBar, addressBar);
                }
            }
        }

        private void MouseMoveDetermine(MouseEventArgs e, out double distanceX, out double distanceY)
        {
            Point currentPosition = e.GetPosition(this);
            distanceX = Math.Abs(clickPoint.X - currentPosition.X);
            distanceY = Math.Abs(clickPoint.Y - currentPosition.Y);
        }

        private void DragCutOrCopySelect(FrameworkElement frameworkElement, ListView listBar, TextBlock addressBar)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                DragCopyItemSelect(frameworkElement, listBar, addressBar);
                listBar.UnselectAll();
                DragDrop.DoDragDrop(frameworkElement, new DataObject(DataFormats.Serializable, frameworkElement.DataContext), DragDropEffects.Copy);
            }
            else
            {
                DragCutItemSelect(frameworkElement, listBar, addressBar);
                listBar.UnselectAll();
                DragDrop.DoDragDrop(frameworkElement, new DataObject(DataFormats.Serializable, frameworkElement.DataContext), DragDropEffects.Move);
            }
        }

        private void DragCopyItemSelect(FrameworkElement frameworkElement, ListView listBar, TextBlock addressBar)
        {
            if (frameworkElement.DataContext is DriveInfo) { }
            else if (frameworkElement.DataContext is DirectoryInfo)
                GetFolderInformation(listBar, 3);
            else if (frameworkElement.DataContext is FileInfo)
                GetFileInformation(listBar, addressBar, 4);
        }

        private void DragCutItemSelect(FrameworkElement frameworkElement, ListView listBar, TextBlock addressBar)
        {
            if (frameworkElement.DataContext is DriveInfo) { }
            else if (frameworkElement.DataContext is DirectoryInfo)
                GetFolderInformation(listBar, 1);
            else if (frameworkElement.DataContext is FileInfo)
                GetFileInformation(listBar, addressBar, 2);
        }

        private void ListBarLeft_DragOver(object sender, DragEventArgs e)
        {
            TextBlock addressBar = AddressBarLeft;
            ShowDragEffect(e, addressBar);
        }

        private void ListBarRight_DragOver(object sender, DragEventArgs e)
        {
            TextBlock addressBar = AddressBarRight;
            ShowDragEffect(e, addressBar);
        }

        private static void ShowDragEffect(DragEventArgs e, TextBlock addressBar)
        {
            if (addressBar.Text == string.Empty)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void ListViewItemLeft_DragOver(object sender, DragEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            item.IsSelected = true;
        }

        private void ListViewItemRight_DragOver(object sender, DragEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            item.IsSelected = true;
        }

        private void ListViewItemLeft_DragLeave(object sender, DragEventArgs e)
        {
            ListBarLeft.UnselectAll();
        }

        private void ListViewItemRight_DragLeave(object sender, DragEventArgs e)
        {
            ListBarRight.UnselectAll();
        }

        private void ListBarLeft_Drop(object sender, DragEventArgs e)
        {
            ListView listBar = ListBarLeft;
            TextBlock addressBar = AddressBarLeft;
            Insert(listBar, addressBar);
        }

        private void ListBarRight_Drop(object sender, DragEventArgs e)
        {
            ListView listBar = ListBarRight;
            TextBlock addressBar = AddressBarRight;
            Insert(listBar, addressBar);
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


