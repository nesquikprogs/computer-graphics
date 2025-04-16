using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace HistogramApp
{
    public partial class MainWindow : Window
    {
        private BitmapSource imageSource;
        private int[] redHistogramData;
        private int[] greenHistogramData;
        private int[] blueHistogramData;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    imageSource = new BitmapImage(new Uri(openFileDialog.FileName));
                    ImageViewer.Source = imageSource;


                    await Task.Run(() => CalculateHistogram(imageSource));

                    UpdateHistogramDisplay();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }


        private void CalculateHistogram(BitmapSource bitmapSource)
        {
            if (bitmapSource == null) return;

            int width = 0;
            int height = 0;
            int stride = 0;
            byte[] pixels = null;
            PixelFormat format = PixelFormats.Default;

            Application.Current.Dispatcher.Invoke(() =>
            {
                width = bitmapSource.PixelWidth;
                height = bitmapSource.PixelHeight;
                format = bitmapSource.Format;
                stride = width * ((format.BitsPerPixel + 7) / 8);
                pixels = new byte[height * stride];
                bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            });



            redHistogramData = new int[256];
            greenHistogramData = new int[256];
            blueHistogramData = new int[256];

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte blue = pixels[i];
                byte green = pixels[i + 1];
                byte red = pixels[i + 2];

                byte threshold = 2;

                if (red < threshold) red = 0;
                if (blue < threshold) blue = 0;



                redHistogramData[red]++;
                greenHistogramData[green]++;
                blueHistogramData[blue]++;
            }
        }


        private void UpdateHistogramDisplay()
        {
            if (redHistogramData == null || greenHistogramData == null || blueHistogramData == null) return;

            RedHistogramCanvas.Children.Clear();
            GreenHistogramCanvas.Children.Clear();
            BlueHistogramCanvas.Children.Clear();

            DrawHistogram(RedHistogramCanvas, redHistogramData, Brushes.Red);
            DrawHistogram(GreenHistogramCanvas, greenHistogramData, Brushes.Green);
            DrawHistogram(BlueHistogramCanvas, blueHistogramData, Brushes.Blue);
        }


        private void DrawHistogram(Canvas canvas, int[] histogramData, Brush color)
        {
            if (histogramData == null || histogramData.Length != 256) return;

            double canvasWidth = canvas.ActualWidth;
            double canvasHeight = canvas.ActualHeight;

            if (canvasWidth <= 0 || canvasHeight <= 0) return;
            int maxFrequency = histogramData.Max();

            for (int i = 0; i < 256; i++)
            {
                double x = (double)i / 255 * canvasWidth;
                double barHeight = (double)histogramData[i] / maxFrequency * canvasHeight;
                double y = canvasHeight - barHeight;

                System.Windows.Shapes.Rectangle bar = new System.Windows.Shapes.Rectangle
                {
                    Width = canvasWidth / 256,
                    Height = barHeight,
                    Fill = color
                };

                Canvas.SetLeft(bar, x);
                Canvas.SetBottom(bar, 0);
                canvas.Children.Add(bar);
            }
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateHistogramDisplay();
        }
    }


}