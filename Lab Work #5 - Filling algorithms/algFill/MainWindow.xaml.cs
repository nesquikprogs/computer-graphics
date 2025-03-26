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
using System.Linq;

namespace ScanlineFillApp
{
    public partial class MainWindow : Window
    {
        private Point startPoint;  // Начальная точка для рисования
        private bool isDrawing = false;  // Флаг, указывающий на процесс рисования
        private List<Point> polygonPoints = new List<Point>();  // Список точек многоугольника
        private Color fillColor = Colors.Red;  // Цвет заливки
        private Color borderColor = Colors.Black;  // Цвет границы
        private WriteableBitmap writeableBitmap;  // Рабочий битмап для отрисовки
        private bool isShapeDrawn = false;  // Флаг, указывающий, что форма нарисована

        private DispatcherTimer timer;  // Таймер для пошагового рисования заливки
        private int currentScanlineY;  // Текущая Y-координата сканируемой строки
        private int minY, maxY;  // Минимальная и максимальная Y-координаты многоугольника
        private List<List<(int x, int y, Color color)>> scanlinesToDraw;  // Список строк для заливки

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;  // Обработчик события загрузки окна

            // Настройка таймера для пошагового рисования
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1); // Интервал 0.1 секунды
            timer.Tick += Timer_Tick;  // Обработчик события для тикера таймера
        }

        // Обработчик события загрузки окна
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Проверка размера области рисования и инициализация битмапа
            if (DrawingAreaCanvas.ActualWidth > 0 && DrawingAreaCanvas.ActualHeight > 0)
            {
                writeableBitmap = new WriteableBitmap((int)DrawingAreaCanvas.ActualWidth, (int)DrawingAreaCanvas.ActualHeight, 96, 96, PixelFormats.Bgra32, null);
                DrawingAreaImage.Source = writeableBitmap;
                ClearBitmap();  // Очищаем область рисования
            }
        }

        // Обработчик события нажатия кнопки мыши
        private void DrawingArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)  // Если нажата левая кнопка мыши
            {
                if (DrawingAreaCanvas.ActualWidth <= 0 || DrawingAreaCanvas.ActualHeight <= 0)
                {
                    return;
                }
                isDrawing = true;  // Начинаем рисование
                isShapeDrawn = false;  // Форма еще не нарисована
                startPoint = e.GetPosition(DrawingAreaCanvas);  // Сохраняем начальную точку
                polygonPoints.Clear();  // Очищаем список точек
                polygonPoints.Add(startPoint);  // Добавляем начальную точку
            }
        }

        // Обработчик события движения мыши
        private void DrawingArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(DrawingAreaCanvas);  // Получаем текущую позицию мыши
                // Рисуем линию от последней точки до текущей
                DrawLineOnBitmap((int)polygonPoints[polygonPoints.Count - 1].X, (int)polygonPoints[polygonPoints.Count - 1].Y, (int)currentPoint.X, (int)currentPoint.Y, borderColor);
                polygonPoints.Add(currentPoint);  // Добавляем точку в список
            }
        }

        // Обработчик события отпускания кнопки мыши
        private void DrawingArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;  // Завершаем рисование
                // Если рисуется многоугольник с более чем двумя точками
                if (polygonPoints.Count > 2)
                {
                    // Замкнем многоугольник, нарисовав последнюю линию
                    DrawLineOnBitmap((int)polygonPoints[polygonPoints.Count - 1].X, (int)polygonPoints[polygonPoints.Count - 1].Y, (int)polygonPoints[0].X, (int)polygonPoints[0].Y, borderColor);
                    isShapeDrawn = true;  // Устанавливаем флаг, что форма нарисована
                }
                else
                {
                    isShapeDrawn = false;  // Если форма недостаточно сложна, сбрасываем флаг
                }
            }
        }

        // Обработчик события нажатия кнопки мыши на уже нарисованной форме
        private void DrawingArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing && isShapeDrawn)  // Если форма уже нарисована
            {
                StartScanlineFill();  // Запускаем процесс заливки
            }
        }

        // Построчная заливка(вычисление пересечний для каждой строки от Ymin до Ymax)
        // DispatcherTimer (таймера) поочередно отрисовываются пиксели для каждой строки в методе Timer_Tick
        // Метод для начала заливки с использованием алгоритма сканирующих строк
        private void StartScanlineFill()
        {
            if (polygonPoints == null || polygonPoints.Count < 3) return;  // Проверка на наличие формы

            // Вычисляем минимальное и максимальное значение Y для многоугольника
            minY = (int)polygonPoints.Min(p => p.Y);
            maxY = (int)polygonPoints.Max(p => p.Y);
            currentScanlineY = minY;  // Начинаем с минимальной Y-координаты
            scanlinesToDraw = new List<List<(int x, int y, Color color)>>();  // Список для хранения линий заливки

            // Предварительно вычисляем все строки для заливки
            for (int y = minY; y <= maxY; y++)
            {
                List<int> intersections = new List<int>();  // Список пересечений с горизонтальными линиями

                for (int i = 0; i < polygonPoints.Count; i++)
                {
                    Point p1 = polygonPoints[i];
                    Point p2 = polygonPoints[(i + 1) % polygonPoints.Count];

                    // Проверка на пересечение с текущей горизонтальной строкой
                    if ((p1.Y <= y && p2.Y > y) || (p2.Y <= y && p1.Y > y))
                    {
                        int x = (int)(p1.X + (double)(y - p1.Y) / (p2.Y - p1.Y) * (p2.X - p1.X));  // Вычисление точки пересечения
                        intersections.Add(x);
                    }
                }

                intersections.Sort();  // Сортировка точек пересечения

                List<(int x, int y, Color color)> pixelsToDraw = new List<(int, int, Color)>();

                // Для каждой пары пересечений рисуем пиксели
                for (int i = 0; i < intersections.Count - 1; i += 2)
                {
                    int x1 = intersections[i];
                    int x2 = intersections[i + 1];

                    // Добавляем пиксели для заливки между x1 и x2
                    for (int x = x1; x <= x2; x++)
                    {
                        if (x >= 0 && x < writeableBitmap.PixelWidth && y >= 0 && y < writeableBitmap.PixelHeight)
                        {
                            pixelsToDraw.Add((x, y, fillColor));
                        }
                    }
                }

                scanlinesToDraw.Add(pixelsToDraw);  // Добавляем строку для отрисовки
            }

            timer.Start();  // Запускаем таймер для пошаговой заливки
        }

        // Обработчик события тикера таймера для рисования каждой строки
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentScanlineY <= maxY)
            {
                DrawScanline(scanlinesToDraw[currentScanlineY - minY]);  // Рисуем текущую строку
                currentScanlineY++;
            }
            else
            {
                timer.Stop();  // Останавливаем таймер после завершения
            }
        }

        // Метод для рисования сканируемой строки
        private void DrawScanline(List<(int x, int y, Color color)> pixels)
        {
            if (writeableBitmap == null || pixels == null || pixels.Count == 0) return;

            writeableBitmap.Lock();
            try
            {
                IntPtr pBackBuffer = writeableBitmap.BackBuffer;
                int stride = writeableBitmap.BackBufferStride;

                // Рисуем каждый пиксель в строке
                foreach (var pixel in pixels)
                {
                    int x = pixel.x;
                    int y = pixel.y;
                    Color color = pixel.color;

                    unsafe
                    {
                        byte* pPixel = (byte*)pBackBuffer + y * stride + x * 4;

                        pPixel[0] = color.B;
                        pPixel[1] = color.G;
                        pPixel[2] = color.R;
                        pPixel[3] = color.A;
                    }
                }

                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight));
            }
            finally
            {
                writeableBitmap.Unlock();
            }
        }

        // Метод для получения цвета пикселя по координатам
        private Color GetPixelColor(int x, int y)
        {
            if (writeableBitmap == null) return Colors.Transparent;

            byte[] pixels = new byte[4];
            Int32Rect rect = new Int32Rect(x, y, 1, 1);
            writeableBitmap.CopyPixels(rect, pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        // Метод для рисования линии на битмапе
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

            // Алгоритм Брезенхема для рисования линии
            while (true)
            {
                DrawPixel(x1, y1, color);  // Рисуем пиксель

                if (x1 == x2 && y1 == y2) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x1 += sx; }
                if (e2 < dx) { err += dx; y1 += sy; }
            }
        }

        // Метод для рисования одного пикселя
        private void DrawPixel(int x, int y, Color color)
        {
            if (writeableBitmap != null)
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
                writeableBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));  // Отметка области как грязной
                writeableBitmap.Unlock();
            }
        }

        // Метод для очистки битмапа
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
                            pRow[index] = 255;  // Прозрачность (A)
                            pRow[index + 1] = 255;  // Зеленый
                            pRow[index + 2] = 255;  // Красный
                            pRow[index + 3] = 255;  // Синий
                        }
                    }
                }

                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight));
                writeableBitmap.Unlock();
            }
        }

        // Обработчик клика по кнопке очистки
        private void Clearutton_Click(object sender, RoutedEventArgs e)
        {
            ClearBitmap();  // Очищаем битмап
        }

        // Обработчик изменения цвета в селекторе цвета
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                fillColor = e.NewValue.Value;  // Обновляем цвет заливки
            }
        }
    }
}
