namespace FileManager.ViewModels
{
    internal class DriveViewModel : FileSystemViewModel
    {
        public DriveViewModel(string name, string path)
             : base(name, path, @"Images\drive_icon.png") { }      
    }
}
