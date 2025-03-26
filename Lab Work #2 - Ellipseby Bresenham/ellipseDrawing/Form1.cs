using System;
using System.Drawing;
using System.Windows.Forms;

namespace EllipseDrawing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Обработчик клика по кнопке рисования эллипса
        private void btnDrawEllipse_Click(object sender, EventArgs e)
        {
            // Получаем диаметр эллипса
            int width = int.Parse(txtWidth.Text);  // Горизонтальный диаметр
            int height = int.Parse(txtHeight.Text);  // Вертикальный диаметр

            // Начальные координаты центра эллипса
            int centerX = panel1.Width / 2;
            int centerY = panel1.Height / 2;

            // Создаем объект Graphics для рисования
            using (Graphics g = panel1.CreateGraphics())
            {
                Pen pen = new Pen(Color.Red, 2);  // Используем красную ручку для рисования
                DrawEllipseBresenham(g, centerX, centerY, width, height, pen);  // Рисуем эллипс
            }
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

        // Обработчик клика по кнопке рисования окружности
        private void btnDrawCircle_Click(object sender, EventArgs e)
        {
            // Получаем радиус окружности
            int radius = int.Parse(txtRadius.Text);

            // Начальные координаты центра окружности
            int centerX = panel1.Width / 2;
            int centerY = panel1.Height / 2;

            // Создаем объект Graphics для рисования
            using (Graphics g = panel1.CreateGraphics())
            {
                Pen pen = new Pen(Color.Blue, 2);  // Используем синюю ручку для рисования
                DrawCircleBresenham(g, centerX, centerY, radius, pen);  // Рисуем окружность
            }
        }

        // Метод рисования окружности методом Брезенхема
        private void DrawCircleBresenham(Graphics g, int centerX, int centerY, int radius, Pen pen)
        {
            int x = 0;
            int y = radius;
            int p = 3 - 2 * radius;

            // Рисуем 4 симметричные точки окружности
            while (x <= y)
            {
                g.DrawRectangle(pen, centerX + x, centerY - y, 1, 1);  // Верхняя правая
                g.DrawRectangle(pen, centerX - x, centerY - y, 1, 1);  // Верхняя левая
                g.DrawRectangle(pen, centerX + x, centerY + y, 1, 1);  // Нижняя правая
                g.DrawRectangle(pen, centerX - x, centerY + y, 1, 1);  // Нижняя левая
                g.DrawRectangle(pen, centerX + y, centerY - x, 1, 1);  // Верхняя правая (с симметрией)
                g.DrawRectangle(pen, centerX - y, centerY - x, 1, 1);  // Верхняя левая (с симметрией)
                g.DrawRectangle(pen, centerX + y, centerY + x, 1, 1);  // Нижняя правая (с симметрией)
                g.DrawRectangle(pen, centerX - y, centerY + x, 1, 1);  // Нижняя левая (с симметрией)

                if (p <= 0)
                {
                    p += 4 * x + 6;
                }
                else
                {
                    p += 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }
        }

        // Метод для очистки панели
        private void ClearAll()
        {
            panel1.Invalidate();  // Очищаем панель
        }

        // Обработчик клика по кнопке очистки
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();  // Очищаем панель
        }
    }
}
