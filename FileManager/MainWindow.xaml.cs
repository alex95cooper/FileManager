using System.Windows;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HelloButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = "Hello";
        }
    }
}
