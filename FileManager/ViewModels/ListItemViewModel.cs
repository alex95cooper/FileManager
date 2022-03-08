namespace FileManager.ViewModels
{
    internal abstract class ListItemViewModel
    {
        public string Name { get; }
        public string Path { get; }
        public string IconPath { get; }

        public ListItemViewModel(string name, string path, string iconPath)
        {
            Name = name;
            Path = path;
            IconPath = iconPath;
        }
    }
}
