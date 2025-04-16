using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Collections.Concurrent;

namespace SeedFillApp
{
    public partial class MainWindow : Window
    {
        private Point startPoint;
        private bool isDrawing = false;
        private List<Point> polygonPoints = new List<Point>();
        private Color fillColor = Colors.Red;
        private Color borderColor = Colors.Black;

        private WriteableBitmap writeableBitmap;
        private bool isShapeDrawn = false;
        private enum FillMode { FourWay, EightWay }
        private FillMode fillMode = FillMode.FourWay;

        private DispatcherTimer timer;
        private ConcurrentQueue<Point> pixelsToFill;
        private HashSet<string> visited;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1); // Более плавный эффект
            timer.Tick += Timer_Tick;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            writeableBitmap = new WriteableBitmap((int)DrawingAreaCanvas.ActualWidth, (int)DrawingAreaCanvas.ActualHeight, 96, 96, PixelFormats.Bgra32, null);
            DrawingAreaImage.Source = writeableBitmap;
            ClearBitmap();


        }

        private void DrawingArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (DrawingAreaCanvas.ActualWidth <= 0 || DrawingAreaCanvas.ActualHeight <= 0)
                {
                    return;
                }
                isDrawing = true;
                isShapeDrawn = false;
                startPoint = e.GetPosition(DrawingAreaCanvas);
                polygonPoints.Clear();
                polygonPoints.Add(startPoint);
            }
        }

        private void DrawingArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(DrawingAreaCanvas);
                DrawLineOnBitmap((int)polygonPoints[polygonPoints.Count - 1].X, (int)polygonPoints[polygonPoints.Count - 1].Y, (int)currentPoint.X, (int)currentPoint.Y, borderColor);
                polygonPoints.Add(currentPoint);
            }
        }

        private void DrawingArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                if (polygonPoints.Count > 2)
                {
                    DrawLineOnBitmap((int)polygonPoints[polygonPoints.Count - 1].X, (int)polygonPoints[polygonPoints.Count - 1].Y, (int)polygonPoints[0].X, (int)polygonPoints[0].Y, borderColor);
                    isShapeDrawn = true;
                }
                else
                {
                    isShapeDrawn = false;
                }
            }
        }

        private void DrawingArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing && isShapeDrawn)
            {
                Point seedPoint = e.GetPosition(DrawingAreaCanvas);
                StartSeedFill((int)seedPoint.X, (int)seedPoint.Y, fillMode);
            }
        }

        private void StartSeedFill(int x, int y, FillMode fillMode)
        {
            if (x < 0 || x >= DrawingAreaCanvas.ActualWidth || y < 0 || y >= DrawingAreaCanvas.ActualHeight)
            {
                return;
            }

            pixelsToFill = new ConcurrentQueue<Point>();
            visited = new HashSet<string>();

            Point startPoint = new Point(x, y);
            pixelsToFill.Enqueue(startPoint);
            visited.Add(GetKey(startPoint));

            timer.Start();
        }

        private List<Point> GetNeighbors(int x, int y, FillMode fillMode)
        {
            List<Point> neighbors = new List<Point>();

            if (fillMode == FillMode.FourWay)
            {
                neighbors.Add(new Point(x + 1, y));
                neighbors.Add(new Point(x - 1, y));
                neighbors.Add(new Point(x, y + 1));
                neighbors.Add(new Point(x, y - 1));
            }
            else if (fillMode == FillMode.EightWay)
            {
                neighbors.Add(new Point(x + 1, y));     // E
                neighbors.Add(new Point(x - 1, y));     // W
                neighbors.Add(new Point(x, y + 1));     // S
                neighbors.Add(new Point(x, y - 1));     // N
                neighbors.Add(new Point(x + 1, y + 1)); // SE
                neighbors.Add(new Point(x - 1, y - 1)); // NW
                neighbors.Add(new Point(x - 1, y + 1)); // SW
                neighbors.Add(new Point(x + 1, y - 1)); // NE
            }

            return neighbors;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!pixelsToFill.IsEmpty)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (pixelsToFill.TryDequeue(out Point current))
                    {
                        int cx = (int)current.X;
                        int cy = (int)current.Y;

                        if (cx < 0 || cx >= DrawingAreaCanvas.ActualWidth || cy < 0 || cy >= DrawingAreaCanvas.ActualHeight)
                        {
                            continue;
                        }

                        Color pixelColor = GetPixelColor(cx, cy);

                        if (IsSameColor(pixelColor, borderColor) || IsSameColor(pixelColor, fillColor))
                        {
                            continue;
                        }

                        DrawPixel(cx, cy, fillColor);

                        List<Point> neighbors = GetNeighbors(cx, cy, fillMode);

                        foreach (Point neighbor in neighbors)
                        {
                            int nx = (int)neighbor.X;
                            int ny = (int)neighbor.Y;

                            if (nx < 0 || nx >= DrawingAreaCanvas.ActualWidth || ny < 0 || ny >= DrawingAreaCanvas.ActualHeight)
                            {
                                continue;
                            }

                            //  Дополнительная проверка для диагональных соседей в 8-связном методе
                            if (fillMode == FillMode.EightWay)
                            {
                                // Проверяем, что для диагонального пикселя оба его "прямых" соседа
                                // либо уже закрашены, либо не являются границей.
                                if ((nx == cx + 1 && ny == cy + 1) || (nx == cx - 1 && ny == cy - 1) ||
                                    (nx == cx - 1 && ny == cy + 1) || (nx == cx + 1 && ny == cy - 1))
                                {
                                    // SE, NW, SW, NE neighbors

                                    bool okToFill = true;
                                    if (nx == cx + 1 && ny == cy + 1) //SE
                                    {
                                        okToFill = !(IsSameColor(GetPixelColor(cx + 1, cy), borderColor) || IsSameColor(GetPixelColor(cx, cy + 1), borderColor));
                                    }
                                    else if (nx == cx - 1 && ny == cy - 1) //NW
                                    {
                                        okToFill = !(IsSameColor(GetPixelColor(cx - 1, cy), borderColor) || IsSameColor(GetPixelColor(cx, cy - 1), borderColor));
                                    }
                                    else if (nx == cx - 1 && ny == cy + 1) //SW
                                    {
                                        okToFill = !(IsSameColor(GetPixelColor(cx - 1, cy), borderColor) || IsSameColor(GetPixelColor(cx, cy + 1), borderColor));
                                    }
                                    else if (nx == cx + 1 && ny == cy - 1) //NE
                                    {
                                        okToFill = !(IsSameColor(GetPixelColor(cx + 1, cy), borderColor) || IsSameColor(GetPixelColor(cx, cy - 1), borderColor));
                                    }

                                    if (!okToFill) continue; // Не добавляем диагонального соседа, если хотя бы один "прямой" сосед - граница.
                                }
                            }


                            Color neighborColor = GetPixelColor(nx, ny);
                            if (!IsSameColor(neighborColor, borderColor) && !IsSameColor(neighborColor, fillColor))
                            {
                                string key = GetKey(neighbor);
                                if (!visited.Contains(key))
                                {
                                    pixelsToFill.Enqueue(neighbor);
                                    visited.Add(key);
                                }
                            }

                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                timer.Stop();
            }
        }

        private string GetKey(Point p)
        {
            return $"{p.X}-{p.Y}";
        }

        private Color GetPixelColor(int x, int y)
        {
            if (writeableBitmap == null) return Colors.Transparent;

            byte[] pixels = new byte[4];
            Int32Rect rect = new Int32Rect(x, y, 1, 1);
            writeableBitmap.CopyPixels(rect, pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);

        }

        private void DrawPixel(int x, int y, Color color)
        {
            if (writeableBitmap != null)
            {
                if (x >= 0 && x < writeableBitmap.PixelWidth && y >= 0 && y < writeableBitmap.PixelHeight)
                {
                    writeableBitmap.Lock();
                    unsafe
                    {
                        IntPtr pBackBuffer = writeableBitmap.BackBuffer;
                        int stride = writeableBitmap.BackBufferStride;

                        pBackBuffer += y * stride;
                        pBackBuffer += x * 4;

                        int colorData = color.B;
                        colorData |= color.G << 8;
                        colorData |= color.R << 16;
                        colorData |= color.A << 24;

                        *((int*)pBackBuffer) = colorData;
                    }
                    writeableBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
                    writeableBitmap.Unlock();
                }
            }
        }

        private bool IsSameColor(Color color1, Color color2)
        {
            return Math.Abs(color1.R - color2.R) <= 40 &&
                   Math.Abs(color1.G - color2.G) <= 40 &&
                   Math.Abs(color1.B - color2.B) <= 40;
        }

        private void DrawLineOnBitmap(int x1, int y1, int x2, int y2, Color color)
        {
            if (writeableBitmap == null) return;
            if (x1 < 0 || x1 >= writeableBitmap.PixelWidth || y1 < 0 || y1 >= writeableBitmap.PixelHeight ||
                x2 < 0 || x2 >= writeableBitmap.PixelWidth || y2 >= writeableBitmap.PixelHeight)
            {

                return;
            }

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                DrawPixel(x1, y1, color);

                if (x1 == x2 && y1 == y2) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x1 += sx; }
                if (e2 < dx) { err += dx; y1 += sy; }
            }
        }

        private void ClearBitmap()
        {
            if (writeableBitmap != null)
            {

                writeableBitmap.Lock();

                unsafe
                {
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer;
                    int stride = writeableBitmap.BackBufferStride;

                    for (int y = 0; y < writeableBitmap.PixelHeight; y++)
                    {
                        byte* pRow = (byte*)pBackBuffer + y * stride;
                        for (int x = 0; x < writeableBitmap.PixelWidth; x++)
                        {
                            int index = x * 4;
                            pRow[index] = 255;
                            pRow[index + 1] = 255;
                            pRow[index + 2] = 255;
                            pRow[index + 3] = 255;
                        }
                    }
                }
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight));
                writeableBitmap.Unlock();
            }
        }

        private void Clearutton_Click(object sender, RoutedEventArgs e)
        {
            ClearBitmap();
        }

        private void FourWayButton_Click(object sender, RoutedEventArgs e)
        {
            fillMode = FillMode.FourWay;
        }

        private void EightWayButton_Click(object sender, RoutedEventArgs e)
        {
            fillMode = FillMode.EightWay;
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                fillColor = e.NewValue.Value;
            }
        }

        private void FillTriangleButton_Click(object sender, RoutedEventArgs e)
        {
            Point p1 = new Point(100, 50);
            Point p2 = new Point(50, 150);
            Point p3 = new Point(150, 150);

            if (p1.X < 0 || p1.X >= DrawingAreaCanvas.ActualWidth || p1.Y < 0 || p1.Y >= DrawingAreaCanvas.ActualHeight ||
                p2.X < 0 || p2.X >= DrawingAreaCanvas.ActualWidth || p2.Y < 0 || p2.Y >= DrawingAreaCanvas.ActualHeight ||
                p3.X < 0 || p3.X >= DrawingAreaCanvas.ActualWidth || p3.Y >= DrawingAreaCanvas.ActualHeight)
            {

                return;
            }

            FillTriangle(p1, p2, p3, fillColor);
        }

        private void FillTriangle(Point p1, Point p2, Point p3, Color fillColor)
        {
            List<Point> points = new List<Point> { p1, p2, p3 };
            points.Sort((a, b) => a.Y.CompareTo(b.Y));

            Point top = points[0];
            Point middle = points[1];
            Point bottom = points[2];

            FillTriangleHalf(top, middle, bottom, fillColor);
            FillTriangleHalf(bottom, middle, top, fillColor);
        }

        private void FillTriangleHalf(Point top, Point middle, Point bottom, Color fillColor)
        {
            double dY = bottom.Y - top.Y;
            if (dY == 0) return;

            double dX1 = middle.X - top.X;
            double dX2 = bottom.X - top.X;

            double slope1 = dX1 / dY;
            double slope2 = dX2 / dY;

            double x1 = top.X;
            double x2 = top.X;

            for (int y = (int)top.Y; y <= (int)bottom.Y; y++)
            {
                for (int x = (int)x1; x <= (int)x2; x++)
                {
                    DrawPixel(x, y, fillColor);
                }

                x1 += slope1;
                x2 += slope2;
            }
        }
    }
}