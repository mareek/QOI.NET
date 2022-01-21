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

namespace QOI.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string[] SupportedExtensions = { "qoi", "png", "jpg", "jpeg", "bmp" };

        private FileInfo? _currentFile;

        public MainWindow()
        {
            InitializeComponent();
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]))
            {
                InitFile(args[1]);
            }
            else
            {
                ChooseFile();
            }
        }

        private void ChooseFile()
        {
            OpenFileDialog openFileDialog = new()
            {
                CheckFileExists = true,
                Multiselect = false,
                Filter = $"Images|{string.Join(";", SupportedExtensions.Select(e => $"*.{e}"))}"
            };
            if (openFileDialog.ShowDialog() ?? false)
            {
                InitFile(openFileDialog.FileName);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) 
                LoadPreviousImage();
            else if (e.Key == Key.Right)
                LoadNextImage();
        }

        private void LoadNextImage() => LoadAnotherImage(1);
        private void LoadPreviousImage() => LoadAnotherImage(-1);

        private void LoadAnotherImage(int direction)
        {
            if (_currentFile == null || _currentFile.Directory == null)
                return;

            var currentDirSupportedFiles = _currentFile.Directory.EnumerateFiles()
                                                                 .Where(IsSupportedFile)
                                                                 .Select(f => f.FullName)
                                                                 .ToList();

            var currentIndex = currentDirSupportedFiles.IndexOf(_currentFile.FullName);
            var futureIndex = currentIndex + direction;
            if (0 <= futureIndex && futureIndex < currentDirSupportedFiles.Count)
            {
                InitFile(currentDirSupportedFiles[futureIndex]);
            }
        }

        private static bool IsSupportedFile(FileInfo file)
            => SupportedExtensions.Any(e => file.Extension.Equals($".{e}", StringComparison.OrdinalIgnoreCase));

        private void InitFile(string filePath)
        {
            _currentFile = new FileInfo(filePath);
            LoadImage(_currentFile);
        }

        private void LoadImage(FileInfo file)
        {
            Title = $"QOI Viewer: {file.Name}";

            try
            {
                if (!TryLoadQoiImage(file))
                {
                    LoadPngImage(file);
                }
            }
            catch (NotSupportedException)
            {
                MessageBox.Show($"Unsupported image type: {file.Name}",
                                "Cannot read image",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error while reading file \"{file.Name}\":\n{e.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void LoadPngImage(FileInfo file)
        {
            BitmapImage bmpSource = new(new(file.FullName));
            ImageViewer.Source = bmpSource;
        }

        private bool TryLoadQoiImage(FileInfo file)
        {
            try
            {
                var imageWriter = new BitmapSourceImageWriter();
                using var fileStream = File.OpenRead(file.FullName);
                new QoiDecoder().Read(fileStream, imageWriter);
                ImageViewer.Source = imageWriter.GetImage();
                return true;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
    }
}
