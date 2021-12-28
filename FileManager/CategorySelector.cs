using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FileManager
{
    internal class FileEntityToImageConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var drawingImage = new DrawingImage();

            if (value is DriveInfo)
            {
                var recource = Application.Current.TryFindResource("driveDrawingImage");

                if (recource is ImageSource driveImageSource)
                    return driveImageSource;
            }
            else if (value is DirectoryInfo)
            {
                var recource = Application.Current.TryFindResource("directoryDrawingImage");

                if (recource is ImageSource directoryImageSource)
                    return directoryImageSource;
            }
            else if (value is FileInfo)
            {
                var recource = Application.Current.TryFindResource("fileDrawingImage");

                if (recource is ImageSource fileImageSource)
                    return fileImageSource;
            }

            return drawingImage;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

