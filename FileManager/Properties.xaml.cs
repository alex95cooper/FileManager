using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace FileManager
{
    public partial class Property : Window
    {
        public Property()
        {
            InitializeComponent();
        }

        public void CategorySelector(ListView listBar)
        {
            

            if (listBar.SelectedItem is DriveInfo)
            {
                propertyIcon.Source = (DrawingImage)Application.Current.TryFindResource("driveDrawingImage");
                NameBar.Text = listBar.SelectedItem.ToString();
            }
            else if (listBar.SelectedItem is DirectoryInfo)
            {
                propertyIcon.Source = (DrawingImage)Application.Current.TryFindResource("directoryDrawingImage");
                NameBar.Text = new DirectoryInfo(listBar.SelectedItem.ToString()).Name;
            }
            else if (listBar.SelectedItem is FileInfo)
            {
                propertyIcon.Source = (DrawingImage)Application.Current.TryFindResource("fileDrawingImage");
                NameBar.Text = Path.GetFileName(listBar.SelectedItem.ToString());
            }

        }
    }
}
