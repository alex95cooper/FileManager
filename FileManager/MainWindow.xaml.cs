using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;
using System.Text.RegularExpressions;


namespace FileManager
{
    public partial class MainWindow : Window
    {
        private string currentPathLeft, currentPathRight, sourceFilePath, sourceFileName, sourceDirPath, sourceDirName, destinationPath;

        private enum Operations
        {
            cutFolder = 1,
            cutFile,
            copyFolder,
            copyFile
        }

        private int dirFileOperSelector;

        private bool cancelIsDone, contextMenuIsOpen, searchIsDone;

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
            if (Properties.Settings.Default.Top == 0 && Properties.Settings.Default.Left == 0 &&
                Properties.Settings.Default.Height == 600 && Properties.Settings.Default.Width == 800)
            {
                this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);
                this.Left = (SystemParameters.PrimaryScreenWidth / 2) - (this.Width / 2);
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

        private void CheckFolderExistence(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (Directory.Exists(addressBar.Text))
                Update(listBar, addressBar, ref currentPath);
            else
                MessageBox.Show("The folder with this address does not exist.");
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

        private void OpenListBarItem(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (listBar.SelectedItem is DriveInfo | listBar.SelectedItem is DirectoryInfo)
            {
                if (Directory.Exists(listBar.SelectedItem.ToString()) | listBar.SelectedItem is DriveInfo)
                {
                    addressBar.Text = listBar.SelectedItem.ToString();
                    Update(listBar, addressBar, ref currentPath);
                }                    
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                if (File.Exists(listBar.SelectedItem.ToString()))
                    OpenFile(listBar, addressBar);
            }
        }

        private static void OpenFile(ListView listBar, TextBox addressBar)
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

        private void ListBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == ListBarLeft)
                GetMousePosition(ListBarLeft, e);
            else if (sender == ListBarRight)
                GetMousePosition(ListBarRight, e);
        }

        private void GetMousePosition(ListView listBar, MouseButtonEventArgs e)
        {
            clickPoint = e.GetPosition(this);

            HitTestResult result = VisualTreeHelper.HitTest(this, clickPoint);
            if (result.VisualHit.GetType() != typeof(ListBoxItem))
                listBar.UnselectAll();
        }

        private void ListBar_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (sender == ListBarLeft)
                SelectContextMenu(ListBarLeft, AddressBarLeft, ContextMenuLeft, e);
            else if (sender == ListBarRight)
                SelectContextMenu(ListBarRight, AddressBarRight, ContextMenuRight, e);
        }

        private void SelectContextMenu(ListView listBar, TextBox addressBar, ContextMenu contextMenu, ContextMenuEventArgs e)
        {
            contextMenuIsOpen = true;
            e.Handled = false;

            foreach (Control item in contextMenu.Items)
                item.Visibility = Visibility.Collapsed;
            

            if (listBar.SelectedItem is DriveInfo)
            {
                MenuItemPropertiesLeft.Visibility = Visibility.Visible;
                MenuItemPropertiesRight.Visibility = Visibility.Visible;
            }
            else if (listBar.SelectedItem is DirectoryInfo)
            {
                foreach (Control item in contextMenu.Items)
                    item.Visibility = Visibility.Visible;

                if (dirFileOperSelector == 0)
                {
                    MenuItemInsertLeft.IsEnabled = false;
                    MenuItemInsertRight.IsEnabled = false;
                }
                else
                {
                    MenuItemInsertLeft.IsEnabled = true;
                    MenuItemInsertRight.IsEnabled = true;
                }
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                foreach (Control item in contextMenu.Items)
                    item.Visibility = Visibility.Visible;

                MenuItemInsertLeft.Visibility = Visibility.Collapsed;
                MenuItemInsertRight.Visibility = Visibility.Collapsed;
            }
            else if (listBar.SelectedItem is null && addressBar.Text != string.Empty)
            {                
                MenuItemPropertiesLeft.Visibility = Visibility.Visible;
                MenuItemPropertiesRight.Visibility = Visibility.Visible;

                if (dirFileOperSelector != 0)
                {
                    MenuItemInsertLeft.Visibility = Visibility.Visible;
                    MenuItemInsertLeft.IsEnabled = true;
                    MenuSeparatorLeft.Visibility = Visibility.Visible;
                    MenuItemInsertRight.Visibility = Visibility.Visible;
                    MenuItemInsertRight.IsEnabled = true;
                    MenuSeparatorRight.Visibility = Visibility.Visible;
                }
            }
            else
            {
                e.Handled = true;
            }
                
        }

        private void MenuItemCut_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemCutLeft)
                Cut(ListBarLeft, AddressBarLeft);
            else if (sender == MenuItemCutRight)
                Cut(ListBarRight, AddressBarRight);
        }

        private void Cut(ListView listBar, TextBox addressBar)
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

        private void GetFileInformation(ListView listBar, TextBox addressBar, int count)
        {
            sourceFileName = Path.GetFileName(listBar.SelectedItem.ToString());

            sourceFilePath = Path.Combine(addressBar.Text, sourceFileName);

            dirFileOperSelector = count;
        }

        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemCopyLeft)
                Copy(ListBarLeft, AddressBarLeft);
            else if (sender == MenuItemCopyRight)
                Copy(ListBarRight, AddressBarRight);
        }

        private void Copy(ListView listBar, TextBox addressBar)
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

        private void MenuItemInsert_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemInsertLeft)
                Insert(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == MenuItemInsertRight)
                Insert(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void Insert(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (addressBar.Text != string.Empty)
            {
                if (dirFileOperSelector == 1 | dirFileOperSelector == 3)
                {
                    GetDestinationPath(listBar, addressBar, sourceDirName);

                    if (destinationPath.Contains(sourceDirPath) & sourceDirPath != destinationPath)
                        MessageBox.Show("You cannot copy or move this folder to its child folder!");
                    else if (sourceDirPath != destinationPath)
                        FolderExeptionCatch(listBar, addressBar);
                }
                else if (dirFileOperSelector == 2 | dirFileOperSelector == 4)
                {
                    GetDestinationPath(listBar, addressBar, sourceFileName);

                    if (sourceFilePath != destinationPath)
                        FileExeptionCatch(listBar, addressBar);
                }

                Update(ListBarLeft, AddressBarLeft, ref currentPath);
                Update(ListBarRight, AddressBarRight, ref currentPath);
            }
        }

        private void GetDestinationPath(ListView listBar, TextBox addressBar, string sourceName)
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

        private void FolderExeptionCatch(ListView listBar, TextBox addressBar)
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

        private void SelectFolderOperation(ListView listBar, TextBox addressBar)
        {
            if (Directory.Exists(sourceDirPath))
            {
                if (dirFileOperSelector == 1)
                {
                    Directory.Move(sourceDirPath, destinationPath);
                    dirFileOperSelector = 0;
                }
                else if (dirFileOperSelector == 3)
                {
                    if (Directory.Exists(destinationPath))
                        HandleFolderException(listBar, addressBar);
                    else
                        DirectoryCopy(sourceDirPath, destinationPath);
                }
            }
            else
                MessageBox.Show("This folder no longer exists");
        }

        private void HandleFolderException(ListView listBar, TextBox addressBar)
        {
            MergeConflictingFolders(listBar, addressBar, sourceDirName);
            if (!cancelIsDone)
            {
                if (dirFileOperSelector == 1)
                    Directory.Delete(sourceDirPath, true);
            }
            else
                cancelIsDone = false;
        }

        private void FileExeptionCatch(ListView listBar, TextBox addressBar)
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
            if (File.Exists(sourceFilePath))
            {
                if (dirFileOperSelector == 2)
                {
                    File.Move(sourceFilePath, destinationPath);
                    dirFileOperSelector = 0;
                }
                else if (dirFileOperSelector == 4)
                    File.Copy(sourceFilePath, destinationPath);
            }
            else
                MessageBox.Show("This file no longer exists");
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
                    File.Copy(file, Path.Combine(destinationDirPath, fileName));
                }
                catch (IOException)
                {
                    if (cancelIsDone) return;
                    ShowSubFileMoveExMessage(file, fileName, destinationDirPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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

                if (dirFileOperSelector == 1 | dirFileOperSelector == 3)
                    DirectoryCopy(sourceDirPath, destinationPath);
                else
                    DirectoryCopy(listBar.SelectedItem.ToString(), destinationPath);
            }
            else
                cancelIsDone = true;
        }

        private void ReplaceConflictingFile(ListView listBar, TextBox addressBar, string sourseFileName)
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

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemDeleteLeft)
                Delete(ListBarLeft);
            else if (sender == MenuItemDeleteRight)
                Delete(ListBarRight);
        }

        private static void Delete(ListView listBar)
        {
            try
            {
                if (listBar.SelectedItem is DirectoryInfo)
                    ShowMessageToDelete(listBar, "folder");
                else if (listBar.SelectedItem is FileInfo)
                    ShowMessageToDelete(listBar, "file");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to delete file: " + ex.Message);
            }
        }

        private static void ShowMessageToDelete(ListView listBar, string name)
        {
            string query = $"Are you sure you want to delete the {name}? \n" + listBar.SelectedItem + " ?";
            if (MessageBox.Show(query, $"Delete the {name}?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (listBar.SelectedItem is DirectoryInfo && Directory.Exists(listBar.SelectedItem.ToString()))
                    Directory.Delete(listBar.SelectedItem.ToString(), true);
                else if (listBar.SelectedItem is FileInfo && File.Exists(listBar.SelectedItem.ToString()))
                    File.Delete(listBar.SelectedItem.ToString());

                listBar.Items.Remove(listBar.SelectedItem);
            }
        }

        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemRenameLeft)
                Rename(ListBarLeft, AddressBarLeft, ref currentPathLeft);
            else if (sender == MenuItemRenameRight)
                Rename(ListBarRight, AddressBarRight, ref currentPathRight);
        }

        private void Rename(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            InputBox inputBox = new();
            inputBox.Owner = this;
            inputBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            sourceDirName = new DirectoryInfo(listBar.SelectedItem.ToString()).Name;
            inputBox.InputTextBox.Text = sourceDirName;

            if (inputBox.ShowDialog() == true & inputBox.InputTextBox.Text != string.Empty)
            {
                string newName = inputBox.InputTextBox.Text;
                string[] unacceptableSymbols = new string[9] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
                Regex unacceptableNames = new(@"^(AUX|CON|NUL|PRN|COM\d|LPT\d|)$", RegexOptions.IgnoreCase);

                if (unacceptableNames.IsMatch(Path.GetFileNameWithoutExtension(newName)))
                    MessageBox.Show($"Name \"{newName}\" reserved by Windows");
                else if (unacceptableSymbols.Any(newName.Contains))
                    MessageBox.Show("The name must not contain the following characters: \\/:*?\"<>|");
                else if (inputBox.InputTextBox.Text != sourceDirName)
                {
                    if (listBar.SelectedItem is DirectoryInfo && Directory.Exists(listBar.SelectedItem.ToString()))
                        RenameFolder(listBar, addressBar, newName);
                    else if (listBar.SelectedItem is FileInfo && Directory.Exists(listBar.SelectedItem.ToString()))
                        RenameFile(listBar, addressBar, newName);
                }

                Update(listBar, addressBar, ref currentPath);
            }
        }

        private void RenameFolder(ListView listBar, TextBox addressBar, string newName)
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

        private void RenameFile(ListView listBar, TextBox addressBar, string newName)
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

        private void ShowMessageOfFileWithoutExtension(ListView listBar, TextBox addressBar, string newName)
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

        private void MenuItemProperties_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItemPropertiesLeft)
                ShowProperties(ListBarLeft, AddressBarLeft);
            else if (sender == MenuItemPropertiesRight)
                ShowProperties(ListBarRight, AddressBarRight);
        }

        private void ShowProperties(ListView listBar, TextBox addressBar)
        {
            Property properties = new();
            properties.Owner = this;
            properties.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            properties.CategorySelector(listBar, addressBar);
            properties.ShowDialog();
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

        private void SelectPath(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            if (searchIsDone)            
                Update(listBar, addressBar, ref currentPath);                           
            else
            {
                if (addressBar.Text.Contains('\\'))
                {
                    int numberOfSlash = addressBar.Text.Count('\\'.Equals);

                    if (numberOfSlash == 1)
                    {
                        if (addressBar.Text[^1] == '\\')
                        {
                            DriveExplorer.ShowDrives(addressBar, listBar);
                            currentPath = addressBar.Text;
                        }
                        else if (addressBar.Text[^1] != '\\')
                            ReturnToParentFolder(addressBar, listBar, ref currentPath);
                    }
                    else if (numberOfSlash > 1)
                        ReturnToParentFolder(addressBar, listBar, ref currentPath);
                }
            }
        }

        private static void ReturnToParentFolder(TextBox addressBar, ListView listBar, ref string currentPath)
        {
            addressBar.Text = Path.GetDirectoryName(addressBar.Text);
            FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
            currentPath = addressBar.Text;
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

        private void Update(ListView listBar, TextBox addressBar, ref string currentPath)
        {
            currentPath = addressBar.Text;
            searchIsDone = false;

            if (!addressBar.Text.Contains('\\'))
                DriveExplorer.ShowDrives(addressBar, listBar);
            else if (addressBar.Text.Contains('\\'))
                FolderAndFileExplorer.ShowContentFolder(addressBar, listBar);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {           
            if (sender == SearchButtonLeft & SearchBarLeft.Text != string.Empty)
                Searcher.Search(AddressBarLeft, ListBarLeft, SearchBarLeft);
            else if (sender == SearchButtonRight & SearchBarRight.Text != string.Empty)
                Searcher.Search(AddressBarRight, ListBarRight, SearchBarRight);

            searchIsDone = true;
        }

        private void SearchButton_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (sender == SearchButtonLeft)
                SelectToolTip(AddressBarLeft);
            else if (sender == SearchButtonRight)
                SelectToolTip(AddressBarRight);           
        }

        private void SelectToolTip(TextBox addressBar)
        {
            if (addressBar.Text == string.Empty)
                SearchButtonLeft.ToolTip = "Search Everywhere";
            else
                SearchButtonLeft.ToolTip = "Search in Current folder";

        }

        private void ListViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is ListViewItem lisviewItem)
            {
                MouseMoveDetermine(e, out Vector distance);
                if (Math.Abs(distance.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(distance.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (ListBarLeft.Items.Contains(lisviewItem.DataContext))
                        DragCutOrCopySelect(lisviewItem, ListBarLeft, AddressBarLeft);
                    else if (ListBarRight.Items.Contains(lisviewItem.DataContext))
                        DragCutOrCopySelect(lisviewItem, ListBarRight, AddressBarRight);
                }
            }
        }

        private void MouseMoveDetermine(MouseEventArgs e, out Vector distance)
        {
            Point currentPosition = e.GetPosition(this);
            distance = clickPoint - currentPosition;
        }

        private void DragCutOrCopySelect(ListViewItem lisviewItem, ListView listBar, TextBox addressBar)
        {
            if (listBar.SelectedItem != null & !contextMenuIsOpen)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    DragCopyItemSelect(lisviewItem, listBar, addressBar);
                    listBar.UnselectAll();
                    DragDrop.DoDragDrop(lisviewItem, new DataObject(DataFormats.Serializable, lisviewItem.DataContext), DragDropEffects.Copy);
                }
                else
                {
                    DragCutItemSelect(lisviewItem, listBar, addressBar);
                    listBar.UnselectAll();
                    DragDrop.DoDragDrop(lisviewItem, new DataObject(DataFormats.Serializable, lisviewItem.DataContext), DragDropEffects.Move);
                }
            }
        }

        private void DragCutItemSelect(ListViewItem lisviewItem, ListView listBar, TextBox addressBar)
        {
            if (lisviewItem.DataContext is DirectoryInfo)
                GetFolderInformation(listBar, 1);
            else if (lisviewItem.DataContext is FileInfo)
                GetFileInformation(listBar, addressBar, 2);
        }

        private void DragCopyItemSelect(ListViewItem lisviewItem, ListView listBar, TextBox addressBar)
        {
            if (lisviewItem.DataContext is DriveInfo) { }
            else if (lisviewItem.DataContext is DirectoryInfo)
                GetFolderInformation(listBar, 3);
            else if (lisviewItem.DataContext is FileInfo)
                GetFileInformation(listBar, addressBar, 4);
        }

        private void ListBar_DragOver(object sender, DragEventArgs e)
        {
            if (sender == ListBarLeft)
                ShowDragEffect(e, AddressBarLeft);
            else if (sender == ListBarRight)
                ShowDragEffect(e, AddressBarRight);
        }

        private static void ShowDragEffect(DragEventArgs e, TextBox addressBar)
        {
            if (addressBar.Text == string.Empty)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            item.IsSelected = true;
        }

        private void ListViewItem_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is ListViewItem lisviewItem)
            {
                if (ListBarLeft.Items.Contains(lisviewItem.DataContext))
                    ListBarLeft.UnselectAll();
                else if (ListBarRight.Items.Contains(lisviewItem.DataContext))
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
    }
}


