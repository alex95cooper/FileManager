namespace FileManager.ViewModels
{
    internal class FolderViewModel : ListItemViewModel
    {
        public FolderViewModel(string name, string path)
             : base(name, path, @"Images\folder_icon.png")  { }                                    
    }

}
