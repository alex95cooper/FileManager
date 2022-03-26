namespace FileManager.ViewModels
{
    internal class FolderViewModel : FileSystemViewModel
    {
        public FolderViewModel(string name, string path)
             : base(name, path, @"Images\folder_icon.png")  { }                                    
    }
}