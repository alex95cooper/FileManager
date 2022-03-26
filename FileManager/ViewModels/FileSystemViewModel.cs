namespace FileManager.ViewModels
{
    internal abstract class FileSystemViewModel
    {
        public FileSystemViewModel(string name, string path, string iconPath)
        {
            Name = name;
            Path = path;
            IconPath = iconPath;
        }

        public string Name { get; }
        public string Path { get; }
        public string IconPath { get; }
    }
}
