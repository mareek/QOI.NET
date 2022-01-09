using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using QOI.Core;

namespace QOI.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]))
            {
                LoadImage(args[1]);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            if (openFileDialog.ShowDialog() ?? false)
            {
                LoadImage(openFileDialog.FileName);
            }
        }

        private void LoadImage(string filePath)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            try
            {
                if (!TryLoadQoiImage(filePath))
                {
                    LoadPngImage(filePath);
                }
            }
            catch (NotSupportedException)
            {
                MessageBox.Show($"Unsupported image type: {fileName}",
                                "Cannot read image",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error while reading file \"{fileName}\":\n{e.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void LoadPngImage(string filePath)
        {
            BitmapImage bmpSource = new(new(filePath));
            ImageViewer.Source = bmpSource;
        }

        private bool TryLoadQoiImage(string filePath)
        {
            try
            {
                using var fileStream = File.OpenRead(filePath);
                QoiImage qoiImage = new QoiDecoder().Read(fileStream);
                ImageViewer.Source = qoiImage.ToImageSource();
                return true;

            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
    }
}
