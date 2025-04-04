﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using QOI.Core;
using QOI.Wpf;

namespace QOI.Viewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static readonly string[] SupportedExtensions = ["qoi", "png", "jpg", "jpeg", "bmp"];

    private readonly QoiBitmapSourceDecoder _imageDecoder = new();
    private readonly SimpleCache<ImageSource> _imageCache = new(10);
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

    private void Prev_MouseDown(object sender, MouseButtonEventArgs e) => LoadPreviousImage();

    private void Next_MouseDown(object sender, MouseButtonEventArgs e) => LoadNextImage();

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

        try
        {
            Title = $"QOI Viewer: {_currentFile.Name}";
            var image = GetImage(_currentFile);
            Background = new ImageBrush(image) { Stretch = Stretch.Uniform };
            InitThumbnails();
        }
        catch (NotSupportedException)
        {
            MessageBox.Show($"Unsupported image type: {_currentFile.Name}",
                            "Cannot read image",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
        }
        catch (Exception e)
        {
            MessageBox.Show($"Error while reading file \"{_currentFile.Name}\":\n{e.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }
    }

    private void InitThumbnails()
    {
        ThumbnailList.ItemsSource = GetThumbnailFiles().Select(GetImage)
                                                       .ToArray();
    }

    private IEnumerable<FileInfo> GetThumbnailFiles()
    {
        if (_currentFile == null || _currentFile.Directory == null)
            yield break;

        var currentDirSupportedFiles = _currentFile.Directory.EnumerateFiles()
                                                             .Where(IsSupportedFile)
                                                             .Select(f => f.FullName)
                                                             .ToList();
        var currentIndex = currentDirSupportedFiles.IndexOf(_currentFile.FullName);
        var firstIndex = Math.Max(currentIndex - 3, 0);
        var lastIndex = Math.Min(currentIndex + 3, currentDirSupportedFiles.Count - 1);

        for (int i = firstIndex; i <= lastIndex; i++)
        {
            yield return new FileInfo(currentDirSupportedFiles[i]);
        }
    }

    private ImageSource GetImage(FileInfo file) => _imageCache.GetOrAdd(file.FullName, _ => LoadImage(file));

    private ImageSource LoadImage(FileInfo file)
    {
        using var fileContent = new MemoryStream();
        using var fileStream = file.OpenRead();
        fileStream.CopyTo(fileContent);

        fileContent.Position = 0;
        bool isQoiImage = QoiDecoder.IsQoiImage(fileContent);
        fileContent.Position = 0;

        return isQoiImage
            ? _imageDecoder.Read(fileContent)
            : LoadNonQoiImage(fileContent);
    }

    private static BitmapImage LoadNonQoiImage(Stream fileStream)
    {
        BitmapImage bmpSource = new();

        bmpSource.BeginInit();
        bmpSource.CacheOption = BitmapCacheOption.OnLoad;
        bmpSource.StreamSource = fileStream;
        bmpSource.EndInit();

        bmpSource.Freeze();

        return bmpSource;
    }
}
