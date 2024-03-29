﻿using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;
using System.Text.RegularExpressions;
using FileManager.ViewModels;
using System.Threading;

namespace FileManager
{
    public partial class MainWindow : Window
    {
        private const string PatternName = @"^(AUX|CON|NUL|PRN|COM\d|LPT\d|)$";

        private const string ItemNotExcistMessage = "Current folder or file no longer exists!";

        private readonly string[] unacceptableSymbols = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };

        private string currentPathLeft;
        private string currentPathRight;
        private string sourceFilePath;
        private string sourceFileName;
        private string sourceDirPath;
        private string sourceDirName;
        private string destinationPath;

        private int dirFileOperSelector;

        private bool cancelIsDone;
        private bool contextMenuIsOpen;
        private bool searchIsDone;

        private Point clickPoint;

        public MainWindow()
        {
            InitializeComponent();
            SetUserPreferences();
            DriveExplorer.ShowDrives(AddressBarLeft, ListBarLeft);
            DriveExplorer.ShowDrives(AddressBarRight, ListBarRight);
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();

                if (sender == AddressBarLeft)
                    CheckFolderExistence(ListBarLeft, AddressBarLeft, ref currentPathLeft);
                else if (sender == AddressBarRight)
                    CheckFolderExistence(ListBarRight, AddressBarRight, ref currentPathRight);
            }
        }

        private void AddressBar_LostFocus(object sender, RoutedEventArgs e)
        {
            AddressBarLeft.Text = currentPathLeft;
            AddressBarRight.Text = currentPathRight;
        }

        private void ListBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender == ListBarLeft)
                OpenListBarItem(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == ListBarRight)
                OpenListBarItem(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void ListBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == ListBarLeft)
                GetMousePosition(ListBarLeft, e);
            else if (sender == ListBarRight)
                GetMousePosition(ListBarRight, e);
        }

        private void ListBar_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (sender == ListBarLeft)
                SelectContextMenu(ListBarLeft, AddressBarLeft, ContextMenuLeft, e);
            else if (sender == ListBarRight)
                SelectContextMenu(ListBarRight, AddressBarRight, ContextMenuRight, e);
        }

        private void MenuItemCut_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemCutLeft)
                Cut(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == MenuItemCutRight)
                Cut(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemCopyLeft)
                Copy(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == MenuItemCopyRight)
                Copy(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void MenuItemInsert_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemInsertLeft)
                Insert(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == MenuItemInsertRight)
                Insert(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemDeleteLeft)
                Delete(ListBarLeft);
            else if (sender == MenuItemDeleteRight)
                Delete(ListBarRight);
        }

        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemRenameLeft)
                ShowRenameDialog(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == MenuItemRenameRight)
                ShowRenameDialog(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void MenuItemProperties_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemPropertiesLeft)
                ShowPropertiesDialog(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == MenuItemPropertiesRight)
                ShowPropertiesDialog(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void ListBar_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            contextMenuIsOpen = false;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == ReturnButtonLeft)
                SelectPath(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == ReturnButtonRight)
                SelectPath(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == SyncButtonLeft)
            {
                AddressBarLeft.Text = AddressBarRight.Text;
                Update(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            }
            else if (sender == SyncButtonRight)
            {
                AddressBarRight.Text = AddressBarLeft.Text;
                Update(ListBarRight, AddressBarRight, ref currentPathRight);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == UpdateButtonLeft)
                Update(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == UpdateButtonRight)
                Update(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == SearchButtonLeft && SearchBarLeft.Text != string.Empty)
            {
                Searcher.Search(AddressBarLeft, ListBarLeft, SearchBarLeft);
            }
            else if (sender == SearchButtonRight && SearchBarRight.Text != string.Empty)
            {
                Searcher.Search(AddressBarRight, ListBarRight, SearchBarRight);
            }

            searchIsDone = true;
        }

        private void SearchButton_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (sender == SearchButtonLeft)
                SelectToolTip(AddressBarLeft);
            else if (sender == SearchButtonRight)
                SelectToolTip(AddressBarRight);
        }

        private void ListViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is ListViewItem listViewItem)
            {
                MouseMoveDetermine(e, out Vector distance);
                if (Math.Abs(distance.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(distance.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (ListBarLeft.Items.Contains(listViewItem.DataContext))
                        DragCutOrCopySelect(listViewItem, ListBarLeft);
                    else if (ListBarRight.Items.Contains(listViewItem.DataContext))
                        DragCutOrCopySelect(listViewItem, ListBarRight);
                }
            }
        }

        private void ListBar_DragOver(object sender, DragEventArgs e)
        {
            if (sender == ListBarLeft)
                ShowDragEffect(e, AddressBarLeft);
            else if (sender == ListBarRight)
                ShowDragEffect(e, AddressBarRight);
        }

        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            item.IsSelected = true;
        }

        private void ListViewItem_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is ListViewItem listViewItem)
            {
                if (ListBarLeft.Items.Contains(listViewItem.DataContext))
                    ListBarLeft.UnselectAll();
                else if (ListBarRight.Items.Contains(listViewItem.DataContext))
                    ListBarRight.UnselectAll();
            }

        }

        private void ListBar_Drop(object sender, DragEventArgs e)
        {
            if (sender == ListBarLeft)
                Insert(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == ListBarRight)
                Insert(ListBarRight, AddressBarRight, ref currentPathRight);
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

        private void SetUserPreferences()
        {
            if (Properties.Settings.Default.Top == 0 && Properties.Settings.Default.Left == 0 &&
                Properties.Settings.Default.Height == 600 && Properties.Settings.Default.Width == 800)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                this.Top = Properties.Settings.Default.Top;
                this.Left = Properties.Settings.Default.Left;
            }

            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void CheckFolderExistence(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (Directory.Exists(addressBar.Text))
                Update(listBar, addressBar, ref currentPath);
            else
                MessageBox.Show("The folder with this address does not exist.");
        }

        private void OpenListBarItem(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (listBar.SelectedItem is DriveViewModel driveViewModel)
            {
                addressBar.Text = driveViewModel.Path;
                Update(listBar, addressBar, ref currentPath);
            }
            else if (listBar.SelectedItem is FolderViewModel folderViewModel && Directory.Exists(folderViewModel.Path))
            {
                addressBar.Text = folderViewModel.Path;
                Update(listBar, addressBar, ref currentPath);
            }
            else if (listBar.SelectedItem is FileViewModel fileViewModel && File.Exists(fileViewModel.Path))
            {
                OpenFile(fileViewModel);
            }
        }

        public void Update(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            currentPath = addressBar.Text;
            searchIsDone = false;

            if (addressBar.Text == string.Empty)
                DriveExplorer.ShowDrives(addressBar, listBar);
            else
                FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
        }

        private static void OpenFile(FileViewModel fileViewModel)
        {
            var openAnyFile = new Process
            {
                StartInfo = new ProcessStartInfo(fileViewModel.Path)
                {
                    UseShellExecute = true
                }
            };

            openAnyFile.Start();
        }

        private void GetMousePosition(ListView listBar, MouseButtonEventArgs e)
        {
            clickPoint = e.GetPosition(this);

            HitTestResult result = VisualTreeHelper.HitTest(this, clickPoint);
            if (result.VisualHit.GetType() != typeof(ListBoxItem))
                listBar.UnselectAll();
        }

        private void SelectContextMenu(ListView listBar, TextBox addressBar, ContextMenu contextMenu, ContextMenuEventArgs e)
        {
            contextMenuIsOpen = true;
            e.Handled = false;

            foreach (Control item in contextMenu.Items)
                item.Visibility = Visibility.Collapsed;

            if (listBar.SelectedItem is DriveViewModel)
            {
                ((MenuItem)contextMenu.Items[(int)ContextMenuItemIndex.Properties]).Visibility = Visibility.Visible;
            }
            else if (listBar.SelectedItem is FolderViewModel)
            {
                foreach (Control item in contextMenu.Items)
                    item.Visibility = Visibility.Visible;

                if (dirFileOperSelector == (int)ContextMenuOperation.None || dirFileOperSelector == (int)ContextMenuOperation.RenameElement)                
                    ((MenuItem)contextMenu.Items[(int)ContextMenuItemIndex.Insert]).IsEnabled = false;                
                else               
                    ((MenuItem)contextMenu.Items[(int)ContextMenuItemIndex.Insert]).IsEnabled = true;                
            }
            else if (listBar.SelectedItem is FileViewModel)
            {
                foreach (Control item in contextMenu.Items)
                    item.Visibility = Visibility.Visible;

                ((MenuItem)contextMenu.Items[(int)ContextMenuItemIndex.Insert]).Visibility = Visibility.Collapsed;
            }
            else if (listBar.SelectedItem is null && addressBar.Text != string.Empty)
            {
                ((MenuItem)contextMenu.Items[(int)ContextMenuItemIndex.Properties]).Visibility = Visibility.Visible;

                if (dirFileOperSelector != (int)ContextMenuOperation.None && dirFileOperSelector != (int)ContextMenuOperation.RenameElement)
                {
                    ((MenuItem)contextMenu.Items[(int)ContextMenuItemIndex.Insert]).Visibility = Visibility.Visible;
                    ((MenuItem)contextMenu.Items[(int)ContextMenuItemIndex.Insert]).IsEnabled = true;
                    ((Separator)contextMenu.Items[(int)ContextMenuItemIndex.Separator]).Visibility = Visibility.Visible;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Cut(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (listBar.SelectedItem is FolderViewModel folderViewModel && Directory.Exists(folderViewModel.Path))
                GetFolderInformation(folderViewModel, (int)ContextMenuOperation.CutFolder);
            else if (listBar.SelectedItem is FileViewModel fileViewModel && File.Exists(fileViewModel.Path))
                GetFileInformation(fileViewModel, (int)ContextMenuOperation.CutFile);
            else
            {
                MessageBox.Show(ItemNotExcistMessage);
                Update(listBar, addressBar, ref currentPath);
            }
        }

        private void GetFolderInformation(FolderViewModel folderViewModel, int count)
        {
            sourceDirName = folderViewModel.Name;

            sourceDirPath = folderViewModel.Path;

            dirFileOperSelector = count;
        }

        private void GetFileInformation(FileViewModel fileViewModel, int count)
        {
            sourceFileName = fileViewModel.Name;

            sourceFilePath = fileViewModel.Path;

            dirFileOperSelector = count;
        }

        private void Copy(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (listBar.SelectedItem is FolderViewModel folderViewModel && Directory.Exists(folderViewModel.Path))
                GetFolderInformation(folderViewModel, (int)ContextMenuOperation.CopyFolder);
            else if (listBar.SelectedItem is FileViewModel fileViewModel && File.Exists(fileViewModel.Path))
                GetFileInformation(fileViewModel, (int)ContextMenuOperation.CopyFile);
            else
            {
                MessageBox.Show(ItemNotExcistMessage);
                Update(listBar, addressBar, ref currentPath);
            }
        }

        private void Insert(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (addressBar.Text != string.Empty)
            {
                if (dirFileOperSelector == (int)ContextMenuOperation.CutFolder || dirFileOperSelector == (int)ContextMenuOperation.CopyFolder)
                {
                    GetDestinationPath(listBar, addressBar, sourceDirName);

                    if (destinationPath.Contains(sourceDirPath) && sourceDirPath != destinationPath)
                        MessageBox.Show("You cannot copy or move this folder to its child folder!");
                    else if (sourceDirPath != destinationPath)
                        SelectFolderOperation(listBar, addressBar);
                }
                else if (dirFileOperSelector == (int)ContextMenuOperation.CutFile || dirFileOperSelector == (int)ContextMenuOperation.CopyFile)
                {
                    GetDestinationPath(listBar, addressBar, sourceFileName);

                    if (sourceFilePath != destinationPath)
                        SelectFileOperation(listBar, addressBar);
                }

                Update(ListBarLeft, AddressBarLeft, ref currentPath);
                Update(ListBarRight, AddressBarRight, ref currentPath);
            }
        }

        private void GetDestinationPath(ListView listBar, TextBox addressBar, string sourceName)
        {
            if (listBar.SelectedItem == null || listBar.SelectedItem is FileViewModel)
                destinationPath = Path.Combine(addressBar.Text, sourceName);
            else if (listBar.SelectedItem is FolderViewModel folderViewModel)
            {
                if (sourceDirPath == folderViewModel.Path)
                    destinationPath = sourceDirPath;
                else
                    destinationPath = Path.Combine(folderViewModel.Path, sourceName);
            }
        }

        private void SelectFolderOperation(ListView listBar, TextBox addressBar)
        {
            try
            {
                if (Directory.Exists(destinationPath) && Directory.Exists(sourceDirPath))
                {
                    MergeConflictingFolders(listBar, addressBar, sourceDirName);
                    HandleConflictingFolders(listBar);
                }
                else if (Directory.Exists(sourceDirPath))
                {
                    if (dirFileOperSelector == (int)ContextMenuOperation.CutFolder)
                    {
                        Directory.Move(sourceDirPath, destinationPath);
                        dirFileOperSelector = (int)ContextMenuOperation.None;
                    }
                    else if (dirFileOperSelector == (int)ContextMenuOperation.CopyFolder)
                        DirectoryCopy(sourceDirPath, destinationPath);
                }
                else if (!Directory.Exists(sourceDirPath))
                    MessageBox.Show(ItemNotExcistMessage);
            }
            catch (Exception ex)
            {
                HandleAllIOExceptions(ex);
            }
        }

        private static void HandleAllIOExceptions(Exception ex)
        {
            if (ex is FileNotFoundException)
            {
                MessageBox.Show("This file no longer exists!");
            }
            else if (ex is ArgumentException)
            {
                MessageBox.Show("Invalid source or destination path!");
            }
            else if (ex is ArgumentNullException)
            {
                MessageBox.Show("Invalid source or destination path!");
            }
            else if (ex is UnauthorizedAccessException)
            {
                MessageBox.Show("No access to the current folder or file!");
            }
            else if (ex is PathTooLongException)
            {
                MessageBox.Show("Source or destination path is too long!");
            }
            else if (ex is DirectoryNotFoundException)
            {
                MessageBox.Show("Current folder or foler with current file no longer exists!");
            }
            else if (ex is NotSupportedException)
            {
                MessageBox.Show("This operation cannot be performed!");
            }
            else
            {
                throw ex;
            }
        }

        private void HandleConflictingFolders(ListView listBar)
        {
            if (!cancelIsDone)
            {
                if (dirFileOperSelector == (int)ContextMenuOperation.CutFolder)
                    Directory.Delete(sourceDirPath, true);
                else if (dirFileOperSelector == (int)ContextMenuOperation.RenameElement)
                {
                    FolderViewModel folderViewModel = listBar.SelectedItem as FolderViewModel;
                    Directory.Delete(folderViewModel.Path, true);
                }
            }
            else
                cancelIsDone = false;
        }

        private void SelectFileOperation(ListView listBar, TextBox addressBar)
        {
            try
            {
                if (File.Exists(destinationPath) && File.Exists(sourceFilePath))
                {
                    ReplaceConflictingFile(addressBar, sourceFileName);
                    if (!cancelIsDone)
                        SelectFileOperation(listBar, addressBar);
                    else cancelIsDone = false;
                }
                else if (File.Exists(sourceFilePath))
                {
                    if (dirFileOperSelector == (int)ContextMenuOperation.CutFile)
                    {
                        File.Move(sourceFilePath, destinationPath);
                        dirFileOperSelector = (int)ContextMenuOperation.None;
                    }
                    else if (dirFileOperSelector == (int)ContextMenuOperation.CopyFile)
                        File.Copy(sourceFilePath, destinationPath);
                }
                else if (!File.Exists(sourceFilePath))
                    MessageBox.Show(ItemNotExcistMessage);
            }
            catch (Exception ex)
            {
                HandleAllIOExceptions(ex);
            }
        }

        private void DirectoryCopy(string dirForCopyPath, string destinationDirPath)
        {
            if (cancelIsDone) return;

            DirectoryInfo dir = new(dirForCopyPath);

            if (!Directory.Exists(destinationDirPath))
                Directory.CreateDirectory(destinationDirPath);

            foreach (DirectoryInfo crrDir in dir.GetDirectories())
            {
                if (cancelIsDone) return;

                if (!Directory.Exists(Path.Combine(destinationDirPath, crrDir.Name)))
                    Directory.CreateDirectory(Path.Combine(destinationDirPath, crrDir.Name));

                DirectoryCopy(crrDir.FullName, Path.Combine(destinationDirPath, crrDir.Name));
            }

            foreach (string file in Directory.GetFiles(dirForCopyPath))
            {
                string fileName = Path.GetFileName(file);

                try
                {
                    if (cancelIsDone) return;

                    if (File.Exists(Path.Combine(destinationDirPath, fileName)))
                        ShowSubFileMoveExMessage(file, fileName, destinationDirPath);
                    else
                        File.Copy(file, Path.Combine(destinationDirPath, fileName));
                }
                catch (Exception ex)
                {
                    HandleAllIOExceptions(ex);
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
                    if (dirFileOperSelector != 3)
                        File.Delete(file);
                    break;
                case MessageBoxResult.Cancel:
                    cancelIsDone = true;
                    break;
            }
        }

        private void MergeConflictingFolders(ListView listBar, TextBox addressBar, string sourseDirName)
        {
            string query = $"A folder named \"{sourseDirName}\" already exists, you want to merge conflicting folders?";
            if (MessageBox.Show(query, "Merge conflicting folders?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                GetDestinationPath(listBar, addressBar, sourseDirName);

                if (dirFileOperSelector == (int)ContextMenuOperation.CutFolder || dirFileOperSelector == (int)ContextMenuOperation.CopyFolder)
                    DirectoryCopy(sourceDirPath, destinationPath);
                else
                {
                    FolderViewModel folderViewModel = listBar.SelectedItem as FolderViewModel;
                    DirectoryCopy(folderViewModel.Path, destinationPath);
                }
            }
            else
                cancelIsDone = true;
        }

        private void ReplaceConflictingFile(TextBox addressBar, string sourseFileName)
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

        private static void Delete(ListView listBar)
        {
            try
            {
                if (listBar.SelectedItem is FolderViewModel folderViewModel)
                {
                    FileSystemViewModel fileSystemViewModel = folderViewModel;
                    ShowMessageToDelete(listBar, fileSystemViewModel, "folder");
                }
                else if (listBar.SelectedItem is FileViewModel fileViewModel)
                {
                    FileSystemViewModel fileSystemViewModel = fileViewModel;
                    ShowMessageToDelete(listBar, fileSystemViewModel, "file");
                }
            }
            catch (Exception ex)
            {
                HandleAllIOExceptions(ex);
            }
        }

        private static void ShowMessageToDelete(ListView listBar, FileSystemViewModel fileSystemViewModel, string name)
        {
            string query = $"Are you sure you want to delete the {name}: \n" + fileSystemViewModel.Name + " ?";

            if (MessageBox.Show(query, $"Delete the {name}?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (listBar.SelectedItem is FolderViewModel folderViewModel && Directory.Exists(folderViewModel.Path))
                    Directory.Delete(folderViewModel.Path, true);
                else if (listBar.SelectedItem is FileViewModel fileViewModel && File.Exists(fileViewModel.Path))
                    File.Delete(fileViewModel.Path);
                else
                    MessageBox.Show(ItemNotExcistMessage);

                listBar.Items.Remove(listBar.SelectedItem);
            }
        }

        private void ShowRenameDialog(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            dirFileOperSelector = (int)ContextMenuOperation.RenameElement;
            InputBox inputBox = new();
            inputBox.Owner = this;
            inputBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            FileSystemViewModel fileSystemViewModel = listBar.SelectedItem as FileSystemViewModel;
            sourceDirName = fileSystemViewModel.Name;
            inputBox.InputTextBox.Text = sourceDirName;

            Rename(listBar, addressBar, inputBox, ref currentPath);
        }

        private void Rename(ListView listBar, TextBox addressBar, InputBox inputBox, ref string currentPath)
        {
            if (inputBox.ShowDialog() == true & inputBox.InputTextBox.Text != string.Empty)
            {
                string newName = inputBox.InputTextBox.Text;
                Regex unacceptableNames = new(PatternName, RegexOptions.IgnoreCase);

                if (unacceptableNames.IsMatch(Path.GetFileNameWithoutExtension(newName)))
                    MessageBox.Show($"Name \"{newName}\" reserved by Windows");
                else if (unacceptableSymbols.Any(newName.Contains))
                    MessageBox.Show("The name must not contain the following characters: \\/:*?\"<>|");
                else if (inputBox.InputTextBox.Text != sourceDirName)
                {
                    if (listBar.SelectedItem is FolderViewModel folderViewModel && Directory.Exists(folderViewModel.Name))
                        RenameFolder(listBar, addressBar, newName);
                    else if (listBar.SelectedItem is FileViewModel fileViewModel && File.Exists(fileViewModel.Name))
                        RenameFile(listBar, addressBar, newName);
                    else
                        MessageBox.Show(ItemNotExcistMessage);
                }

                Update(listBar, addressBar, ref currentPath);
            }
        }

        private void RenameFolder(ListView listBar, TextBox addressBar, string newName)
        {
            try
            {
                if (Directory.Exists(Path.Combine(addressBar.Text, newName)))
                {
                    MergeConflictingFolders(listBar, addressBar, newName);
                    HandleConflictingFolders(listBar);
                }
                else
                {
                    FolderViewModel folderViewModel = listBar.SelectedItem as FolderViewModel;
                    Directory.Move(folderViewModel.Path, Path.Combine(addressBar.Text, newName));
                }
            }
            catch (Exception ex)
            {
                HandleAllIOExceptions(ex);
            }
        }

        private void RenameFile(ListView listBar, TextBox addressBar, string newName)
        {
            try
            {
                FileViewModel fileViewModel = listBar.SelectedItem as FileViewModel;

                if (File.Exists(Path.Combine(addressBar.Text, newName)))
                {
                    ReplaceConflictingFile(addressBar, newName);
                    if (!cancelIsDone)
                        File.Move(fileViewModel.Path, Path.Combine(addressBar.Text, newName));
                }
                else
                {
                    ShowMessageOfFileWithoutExtension(addressBar, fileViewModel, newName);
                    if (!cancelIsDone)
                        File.Move(fileViewModel.Path, Path.Combine(addressBar.Text, newName));
                }
            }
            catch (Exception ex)
            {
                HandleAllIOExceptions(ex);
            }
            finally
            {
                cancelIsDone = false;
            }
        }

        private void ShowMessageOfFileWithoutExtension(TextBox addressBar, FileViewModel fileViewModel, string newName)
        {
            if (Path.GetExtension(newName) == string.Empty)
            {
                string query = "After changing the extension, the file may become unavailable. Want to change it?";
                if (MessageBox.Show(query, "Delete the folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    File.Move(fileViewModel.Path, Path.Combine(addressBar.Text, newName));
                else
                    cancelIsDone = true;
            }
        }

        private void ShowPropertiesDialog(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            Property properties = new();
            properties.Owner = this;
            properties.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            properties.CategorySelector(listBar, addressBar);
            properties.ShowDialog();

            Update(listBar, addressBar, ref currentPath);
        }

        private void SelectPath(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (searchIsDone)
            {
                Update(listBar, addressBar, ref currentPath);
            }
            else
            {
                if (addressBar.Text != string.Empty)
                {
                    DirectoryInfo children = new(addressBar.Text);
                    if (children.Parent == null)
                    {
                        DriveExplorer.ShowDrives(addressBar, listBar);
                    }
                    else
                    {
                        addressBar.Text = Path.GetDirectoryName(addressBar.Text);
                        FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
                    }

                    currentPath = addressBar.Text;
                }
            }
        }

        private void SelectToolTip(TextBox addressBar)
        {
            if (addressBar.Text == string.Empty)
            {
                SearchButtonLeft.ToolTip = "Search Everywhere";
            }
            else
            {
                SearchButtonLeft.ToolTip = "Search in Current folder";
            }
        }

        private void MouseMoveDetermine(MouseEventArgs e, out Vector distance)
        {
            Point currentPosition = e.GetPosition(this);
            distance = clickPoint - currentPosition;
        }

        private void DragCutOrCopySelect(ListViewItem listViewItem, ListView listBar)
        {
            if (listBar.SelectedItem != null && !contextMenuIsOpen)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    DragCopyItemSelect(listViewItem);
                    listBar.UnselectAll();
                    DragDrop.DoDragDrop(listViewItem, new DataObject(DataFormats.Serializable, listViewItem.DataContext), DragDropEffects.Copy);
                }
                else
                {
                    DragCutItemSelect(listViewItem);
                    listBar.UnselectAll();
                    DragDrop.DoDragDrop(listViewItem, new DataObject(DataFormats.Serializable, listViewItem.DataContext), DragDropEffects.Move);
                }
            }
        }

        private void DragCutItemSelect(ListViewItem listViewItem)
        {
            if (listViewItem.DataContext is FolderViewModel folderViewModel)
                GetFolderInformation(folderViewModel, (int)ContextMenuOperation.CutFolder);
            else if (listViewItem.DataContext is FileViewModel fileViewModel)
                GetFileInformation(fileViewModel, (int)ContextMenuOperation.CutFile);
        }

        private void DragCopyItemSelect(ListViewItem listViewItem)
        {
            if (listViewItem.DataContext is FolderViewModel folderViewModel)
                GetFolderInformation(folderViewModel, (int)ContextMenuOperation.CopyFolder);
            else if (listViewItem.DataContext is FileViewModel fileViewModel)
                GetFileInformation(fileViewModel, (int)ContextMenuOperation.CopyFile);
        }

        private static void ShowDragEffect(DragEventArgs e, TextBox addressBar)
        {
            if (addressBar.Text == string.Empty)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }
    }
}


