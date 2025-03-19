using System;
using System.Drawing;
using System.Windows.Forms;

namespace LineDrawing
{
    public partial class Form1 : Form
    {
        private bool isEllipseDrawn = false; // Флаг для отслеживания, нарисован ли эллипс

        public Form1()
        {
            InitializeComponent();
        }

        // Обработчик клика по кнопке рисования эллипса
        private void btnDrawEllipse_Click(object sender, EventArgs e)
        {
            // Если эллипс уже нарисован, очищаем панель
            if (isEllipseDrawn)
            {
                ClearAll();
            }

            // Получаем диаметр эллипса
            int width = int.Parse(txtWidth.Text); // Горизонтальный диаметр
            int height = int.Parse(txtHeight.Text); // Вертикальный диаметр

            // Начальные координаты центра эллипса
            int centerX = panel1.Width / 2;
            int centerY = panel1.Height / 2;

            // Создаем объект Graphics для рисования
            using (Graphics g = panel1.CreateGraphics())
            {
                Pen pen = new Pen(Color.Red, 2);  // Используем красную ручку для рисования
                DrawEllipseBresenham(g, centerX, centerY, width, height, pen);  // Рисуем эллипс
            }

            isEllipseDrawn = true; // Устанавливаем флаг для нарисованного эллипса
        }

        // Метод рисования эллипса методом Брезенхема
        private void DrawEllipseBresenham(Graphics g, int centerX, int centerY, int width, int height, Pen pen)
        {
            int a = width / 2;  // Горизонтальный радиус
            int b = height / 2;  // Вертикальный радиус

            int x = 0;
            int y = b;
            int a2 = a * a;
            int b2 = b * b;
            int err = b2 - a2 * b + a2 / 4;

            // Рисуем 4 симметричные точки эллипса
            while (b2 * x <= a2 * y)
            {
                g.DrawRectangle(pen, centerX + x, centerY - y, 1, 1);  // Верхняя правая
                g.DrawRectangle(pen, centerX - x, centerY - y, 1, 1);  // Верхняя левая
                g.DrawRectangle(pen, centerX + x, centerY + y, 1, 1);  // Нижняя правая
                g.DrawRectangle(pen, centerX - x, centerY + y, 1, 1);  // Нижняя левая

                if (err <= 0)
                {
                    x++;
                    err += 2 * b2 * x + b2;
                }
                else
                {
                    y--;
                    err -= 2 * a2 * y + a2;
                }
            }

            // Рисуем вторую часть эллипса
            x = a;
            y = 0;
            err = a2 - b2 * a + b2 / 4;

            while (a2 * y <= b2 * x)
            {
                g.DrawRectangle(pen, centerX + x, centerY - y, 1, 1);  // Верхняя правая
                g.DrawRectangle(pen, centerX - x, centerY - y, 1, 1);  // Верхняя левая
                g.DrawRectangle(pen, centerX + x, centerY + y, 1, 1);  // Нижняя правая
                g.DrawRectangle(pen, centerX - x, centerY + y, 1, 1);  // Нижняя левая

                if (err <= 0)
                {
                    y++;
                    err += 2 * a2 * y + a2;
                }
                else
                {
                    x--;
                    err -= 2 * b2 * x + b2;
                }
            }
        }

        // Метод для очистки панели
        private void ClearAll()
        {
            panel1.Invalidate();  // Очищаем панель
            isEllipseDrawn = false;  // Сбрасываем флаг
        }

        // Обработчик для кнопки очистки
        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();  // Очищаем панель
        }
    }
}
