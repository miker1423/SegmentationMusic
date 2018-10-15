﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Microsoft.Win32;

namespace PracticaCV6
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Bitmap loadedImage;
        Grayscale grayscale = new Grayscale(0.9, 0.9, 0.9);
        GaussianBlur filter = new GaussianBlur();
        Subtract subtract = new Subtract();
        private Bitmap NewImage(Bitmap selectedImage)
        {
            var grayed = grayscale.Apply(selectedImage);
            var partialResult = filter.Apply(grayed);
            subtract.OverlayImage = grayed;
            return subtract.Apply(partialResult);
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if(loadedImage != null)
            {
                var resultImage = NewImage(loadedImage);
                var rotated = Rotate(resultImage);
                var horizontalStatistics = new HorizontalIntensityStatistics(rotated);
                var gray = horizontalStatistics.Gray.Values;
                result.Dispatcher.Invoke(() => result.Source = GetImage(resultImage));
            }
        }

        RotateBilinear rotator = new RotateBilinear(90, true);
        private Bitmap Rotate(Bitmap bitmap) => rotator.Apply(bitmap);

        private BitmapImage GetImage(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                ms.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            var result = ofd?.ShowDialog() ?? false;
            if (result)
            {
                loadedImage = new Bitmap(ofd.FileName);
            }
        }
    }
}
