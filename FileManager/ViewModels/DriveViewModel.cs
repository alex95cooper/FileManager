namespace FileManager.ViewModels
{
    internal class DriveViewModel : ListItemViewModel
    {
        public DriveViewModel(string name, string path)
             : base(name, path, @"Images\drive_icon.png") { }      
    }
}
