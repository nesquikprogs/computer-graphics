using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClippingApp
{
    public partial class MainWindow : Window
    {
        private Point rectangleStartPoint;
        private Point rectangleEndPoint;
        private bool isDrawingRectangle = false;
        private Rect clippingRectangle;
        private bool rectangleDrawn = false;
        private Point lineStartPoint;
        private bool isDrawingLine = false;
        private WriteableBitmap writeableBitmap;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            writeableBitmap = new WriteableBitmap((int)DrawingAreaCanvas.ActualWidth, (int)DrawingAreaCanvas.ActualHeight, 96, 96, PixelFormats.Bgra32, null);
            DrawingAreaImage.Source = writeableBitmap;
            ClearBitmap();

        }


        private void DrawingAreaCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
            {
                Point currentPosition = e.GetPosition(DrawingAreaCanvas);


                if (!rectangleDrawn)
                {
                    isDrawingRectangle = true;
                    rectangleStartPoint = currentPosition;
                }

                else
                {
                    isDrawingLine = true;
                    lineStartPoint = currentPosition;
                }
            }
        }


        private void DrawingAreaCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.GetPosition(DrawingAreaCanvas);

            //Если рисуется прямоугольник и левая кнопка мыши нажата, обновляем конечную точку и перерисовываем прямоугольник
            if (isDrawingRectangle && e.LeftButton == MouseButtonState.Pressed)
            {
                rectangleEndPoint = currentPosition;
                DrawRectangle();
            }
        }


        private void DrawingAreaCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (isDrawingRectangle)
            {
                isDrawingRectangle = false;
                rectangleDrawn = true;
                rectangleEndPoint = e.GetPosition(DrawingAreaCanvas);


                DrawRectangle();
            }

            else if (isDrawingLine)
            {
                isDrawingLine = false;
                Point lineEndPoint = e.GetPosition(DrawingAreaCanvas);
                DrawClippedLine(lineStartPoint, lineEndPoint);
            }
        }


        private void DrawRectangle()
        {
            if (writeableBitmap == null) return;
            ClearBitmap();


            int x1 = (int)Math.Min(rectangleStartPoint.X, rectangleEndPoint.X);
            int y1 = (int)Math.Min(rectangleStartPoint.Y, rectangleEndPoint.Y);
            int x2 = (int)Math.Max(rectangleStartPoint.X, rectangleEndPoint.X);
            int y2 = (int)Math.Max(rectangleStartPoint.Y, rectangleEndPoint.Y);

            clippingRectangle = new Rect(rectangleStartPoint, rectangleEndPoint);


            for (int x = x1; x <= x2; x++)
            {
                DrawPixel(x, y1, Colors.Black);
                DrawPixel(x, y2, Colors.Black);
            }
            for (int y = y1; y <= y2; y++)
            {
                DrawPixel(x1, y, Colors.Black);
                DrawPixel(x2, y, Colors.Black);
            }
        }


        private void DrawClippedLine(Point p1, Point p2)
        {
            if (writeableBitmap == null) return;
            if (!rectangleDrawn) return;

            int x1 = (int)p1.X;
            int y1 = (int)p1.Y;
            int x2 = (int)p2.X;
            int y2 = (int)p2.Y;

            int code1 = ComputeOutCode(x1, y1);
            int code2 = ComputeOutCode(x2, y2);

            bool accept = false;


            while (true)
            {
                //Если обе точки внутри прямоугольника, линия принята
                if ((code1 | code2) == 0)
                {
                    accept = true;
                    break;
                }
                //Если обе точки с одной и той же стороны от прямоугольника, линия отклонена
                else if ((code1 & code2) != 0)
                {
                    break;
                }
                //Иначе разделяем линию и повторяем
                else
                {
                    int outcodeOut = (code1 != 0) ? code1 : code2;

                    int x = 0, y = 0;

                    //Находим точку пересечения с границей прямоугольника
                    if ((outcodeOut & 8) != 0)
                    {
                        //Точка выше прямоугольника
                        x = x1 + (x2 - x1) * ((int)clippingRectangle.Top - y1) / (y2 - y1); //Формула для вычисления X координаты пересечения
                        y = (int)clippingRectangle.Top;
                    }
                    else if ((outcodeOut & 4) != 0)
                    {
                        //Точка ниже прямоугольника
                        x = x1 + (x2 - x1) * ((int)clippingRectangle.Bottom - y1) / (y2 - y1); //Формула для вычисления X координаты пересечения
                        y = (int)clippingRectangle.Bottom;
                    }
                    else if ((outcodeOut & 2) != 0)
                    {
                        //Точка справа от прямоугольника
                        y = y1 + (y2 - y1) * ((int)clippingRectangle.Right - x1) / (x2 - x1); //Формула для вычисления Y координаты пересечения
                        x = (int)clippingRectangle.Right;
                    }
                    else if ((outcodeOut & 1) != 0)
                    {
                        //Точка слева от прямоугольника
                        y = y1 + (y2 - y1) * ((int)clippingRectangle.Left - x1) / (x2 - x1); //Формула для вычисления Y координаты пересечения
                        x = (int)clippingRectangle.Left;
                    }

                    //Обновляем координаты точки и ее код области
                    if (outcodeOut == code1)
                    {
                        x1 = x;
                        y1 = y;
                        code1 = ComputeOutCode(x1, y1);
                    }
                    else
                    {
                        x2 = x;
                        y2 = y;
                        code2 = ComputeOutCode(x2, y2);
                    }
                }
            }

            //Если линия была принята, рисуем ее
            if (accept)
            {
                DrawLineOnBitmap(x1, y1, x2, y2, Colors.Black);
            }
        }

        //Вычисление кода области для точки относительно прямоугольника отсечения
        private int ComputeOutCode(int x, int y)
        {
            int code = 0;

            //Проверяем положение точки относительно каждой стороны прямоугольника и устанавливаем соответствующие биты в коде
            if (y < clippingRectangle.Top)
            {
                code |= 8; //Выше прямоугольника
            }
            else if (y > clippingRectangle.Bottom)
            {
                code |= 4; //Ниже прямоугольника
            }
            if (x > clippingRectangle.Right)
            {
                code |= 2; //Справа от прямоугольника
            }
            else if (x < clippingRectangle.Left)
            {
                code |= 1; //Слева от прямоугольника
            }

            return code;
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

        //Рисование линии алгоритмом Брезенхэма
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
            rectangleDrawn = false;
        }
    }
}