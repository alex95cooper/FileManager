namespace FileManager.ViewModels
{
    internal class FileViewModel : ListItemViewModel
    {
        public FileViewModel(string name, string path)
             : base(name, path, @"Images\file_icon.png") { }      
    }
}
