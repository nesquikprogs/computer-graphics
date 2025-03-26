using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RotateShape
{
    public partial class MainWindow : Window
    {
        //Список точек, составляющих нарисованную пользователем фигуру
        private List<Point> points = new List<Point>();
        //Флаг, указывающий, находится ли пользователь в процессе рисования фигуры
        private bool isDrawing = false;
        //Общий угол поворота фигуры в градусах (увеличиваетс с каждым кадром анимации)
        private double totalRotationAngle = 0;
        //Таймер, управляющий анимацией поворота
        private DispatcherTimer timer;
        //Скорость поворота в градусах за один тик таймера 
        private double rotationSpeed = 0;
        //Целевой угол поворота
        private double targetRotationAngle = 0;
        //Объект Polyline для отображения рисунка пользователя
        private Polyline polyline;

        public MainWindow()
        {
            InitializeComponent();

            //Создаем объект Polyline для отображения рисунка пользователя
            polyline = new Polyline();
            //Устанавливаем цвет линии
            polyline.Stroke = Brushes.Black;
            //Устанавливаем толщину линии
            polyline.StrokeThickness = 2;
            //Добавляем Polyline на Canvas
            MyCanvas.Children.Add(polyline);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += Timer_Tick;
        }

        //Обработчик события MouseDown на Canvas (нажатие кнопки мыши)
        private void MyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Проверяем, нажата ли левая кнопка мыши
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //Проверяем, не находится ли пользователь в процессе рисования
                if (!isDrawing)
                {
                    //Очищаем список точек и Polyline
                    points.Clear();
                    polyline.Points.Clear();
                    //Делаем Polyline видимой
                    polyline.Visibility = Visibility.Visible;
                    //Добавляем первую точку в список
                    points.Add(e.GetPosition(MyCanvas));
                    //Отображаем первую точку на Polyline
                    polyline.Points.Add(points[0]);
                    //Устанавливаем флаг рисования в true
                    isDrawing = true;
                }
            }
        }

        //Обработчик события MouseMove на Canvas (движение мыши)
        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //Проверяем, нажата ли левая кнопка мыши и находится ли пользователь в процессе рисования
            if (e.LeftButton == MouseButtonState.Pressed && isDrawing)
            {
                //Получаем текущую позицию мыши на Canvas
                Point currentPoint = e.GetPosition(MyCanvas);
                //Добавляем текущую точку в список
                points.Add(currentPoint);
                //Отображаем текущую точку на Polyline
                polyline.Points.Add(currentPoint);
            }
        }

        //Обработчик события MouseUp на Canvas (отпускание кнопки мыши)
        private void MyCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Проверяем, отпущена ли левая кнопка мыши
            if (e.LeftButton == MouseButtonState.Released)
            {
                //Устанавливаем флаг рисования в false
                isDrawing = false;
            }
        }

        //Обработчик события Click на кнопке StartRotationButton (запуск вращения)
        private void StartRotationButton_Click(object sender, RoutedEventArgs e)
        {
            //Проверяем, нарисована ли фигура
            if (points.Count == 0)
            {
                //Если фигура не нарисована, показываем сообщение об ошибке
                MessageBox.Show("Сначала нарисуйте фигуру!");
                return;
            }

            //Проверяем, введены ли корректные значения скорости и угла поворота
            if (double.TryParse(SpeedTextBox.Text, out double durationSeconds) &&
                double.TryParse(AngleTextBox.Text, out double targetAngle) &&
                durationSeconds > 0)
            {
                //Сохраняем целевой угол поворота
                this.targetRotationAngle = targetAngle;
                //Сбрасываем общий угол поворота
                totalRotationAngle = 0;
                //Вычисляем скорость поворота в градусах за один тик таймера
                rotationSpeed = targetRotationAngle / (durationSeconds * 60);

                //Обновляем координаты точек Polyline перед запуском таймера (важно для начального отображения)
                UpdatePolylinePoints();

                //Запускаем таймер (начинаем анимацию)
                timer.Start();
            }
            else
            {
                //Если введены некорректные значения, показываем сообщение об ошибке
                MessageBox.Show("Некорректная скорость или угол поворота! Задайте значения больше 0.");
                return;
            }
        }

        //Метод для обновления координат точек Polyline (выполняет аффинные преобразования)
        private void UpdatePolylinePoints()
        {
            //Вычисляем центр фигуры (среднее арифметическое координат всех точек)
            double centerX = 0;
            double centerY = 0;
            foreach (Point point in points)
            {
                centerX += point.X;
                centerY += point.Y;
            }
            centerX /= points.Count;
            centerY /= points.Count;

            //Создаем новую коллекцию точек для хранения повернутых координат
            PointCollection rotatedPoints = new PointCollection();

            //Перебираем все точки фигуры
            foreach (Point originalPoint in points)
            {
                //Преобразуем угол поворота в радианы
                double angleInRadians = totalRotationAngle * Math.PI / 180;

                //Аффинные преобразования
                //Переносим точку в начало координат 
                double translatedX = originalPoint.X - centerX;
                double translatedY = originalPoint.Y - centerY;

                //Выполняем поворот
                double rotatedX = translatedX * Math.Cos(angleInRadians) - translatedY * Math.Sin(angleInRadians);
                double rotatedY = translatedX * Math.Sin(angleInRadians) + translatedY * Math.Cos(angleInRadians);

                //Возвращаем точку на прежнее место
                double finalX = rotatedX + centerX;
                double finalY = rotatedY + centerY;

                //Добавляем повернутую точку в коллекцию
                rotatedPoints.Add(new Point(finalX, finalY));
            }

            //Обновляем коллекцию точек Polyline 
            polyline.Points = rotatedPoints;
        }

        //Обработчик события Tick таймера 
        private void Timer_Tick(object sender, EventArgs e)
        {
            //Проверяем, нарисована ли фигура
            if (points.Count > 0)
            {
                //Увеличиваем общий угол поворота на величину скорости поворота
                totalRotationAngle += rotationSpeed;

                //Проверяем, достигнут ли целевой угол поворота
                if (totalRotationAngle > targetRotationAngle)
                {
                    //Если достигнут, фиксируем угол поворота на целевом значении
                    totalRotationAngle = targetRotationAngle;
                    //Останавливаем вращение
                    rotationSpeed = 0;
                    //Останавливаем таймер
                    timer.Stop();
                }

                //Обновляем координаты точек Polyline (выполняем аффинные преобразования)
                UpdatePolylinePoints();
            }
        }
    }
}