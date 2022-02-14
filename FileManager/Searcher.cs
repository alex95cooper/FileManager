using System;
using System.IO;
using System.Windows.Controls;

namespace FileManager
{
    public class Searcher
    {
        private static readonly DirectoryInfo dir = null;

        public static void Search(TextBox addressBar, ListView listBar, TextBox searchBar)
        {
            listBar.Items.Clear();

            SearchSelector(addressBar, listBar, searchBar);
        }

        private static void SearchSelector(TextBox addressBar, ListView listBar, TextBox searchBar)
        {
            if (addressBar.Text == "")
            {
                SearchEverywhere(listBar, searchBar);
            }
            else
            {
                string pathRoot = addressBar.Text;

                SearchInCurrentFolder(pathRoot, dir, listBar, searchBar);
            }
        }

        private static void SearchEverywhere(ListView listBar, TextBox searchBar)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed)
                {
                    string pathRoot = drive.Name;

                    SearchInCurrentFolder(pathRoot, dir, listBar, searchBar);
                }
            }
        }

        private static void SearchInCurrentFolder(string pathRoot, DirectoryInfo dir, ListView listBar, TextBox searchBar)
        {
            DirectoryInfo searchFolder = new(pathRoot);
            DirectoryInfo[] subsSearchedDirectories = searchFolder.GetDirectories($"*{searchBar.Text}*", SearchOption.TopDirectoryOnly);
            FileInfo[] SearchedFiles = searchFolder.GetFiles($"*{searchBar.Text}*", SearchOption.TopDirectoryOnly);
            DirectoryInfo[] subDirectoriesForSearch = searchFolder.GetDirectories();
            try
            {
                ShowFoundFolders(subsSearchedDirectories, listBar);

                ShowFoundFiles(SearchedFiles, listBar);

                foreach (DirectoryInfo subDir in subDirectoriesForSearch)
                {
                    try
                    {
                        string path = (Path.Combine(pathRoot, subDir.Name));

                        SearchInCurrentFolder(path, subDir, listBar, searchBar);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }

            return;
        }

        private static void ShowFoundFolders(DirectoryInfo[] subsSearchedDirectories, ListView listBar)
        {
            foreach (DirectoryInfo subDir in subsSearchedDirectories)
            {
                listBar.Items.Add(subDir);
            }
        }

        private static void ShowFoundFiles(FileInfo[] SearchedFiles, ListView listBar)
        {
            foreach (FileInfo file in SearchedFiles)
            {
                listBar.Items.Add(file);
            }
        }
    }
}
