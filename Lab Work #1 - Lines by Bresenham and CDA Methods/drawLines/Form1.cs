using System;
using System.Drawing;
using System.Windows.Forms;

namespace LineDrawing
{
    public partial class Form1 : Form
    {
        // Флаги для отслеживания нарисованных линий
        private bool isBresenhamDrawn = false; // Флаг для первой линии (метод Брезенхема)
        private bool isCDADrawn = false; // Флаг для второй линии (метод ЦДА)

        // Массивы для хранения координат точек линий
        private Point[] firstLinePoints = new Point[2]; // Массив для координат первой линии
        private Point[] secondLinePoints = new Point[2]; // Массив для координат второй линии

        public Form1()
        {
            InitializeComponent();
        }

        // Обработчик клика по кнопке Брезенхема
        private void btnBresenham_Click(object sender, EventArgs e)
        {
            // Если обе линии нарисованы, очищаем панель перед рисованием новой
            if (isBresenhamDrawn && isCDADrawn)
            {
                ClearAll();
            }

            // Получаем координаты первой линии
            int x1 = int.Parse(txtX1.Text);
            int y1 = int.Parse(txtY1.Text);
            int x2 = int.Parse(txtX2.Text);
            int y2 = int.Parse(txtY2.Text);

            // Сохраняем координаты первой линии
            firstLinePoints[0] = new Point(x1, y1);
            firstLinePoints[1] = new Point(x2, y2);

            // Создаем объект Graphics для рисования
            using (Graphics g = panel1.CreateGraphics())
            {
                Pen pen = new Pen(Color.Red, 2);  // Используем красную ручку для рисования
                DrawBresenhamLine(g, x1, y1, x2, y2, pen);  // Рисуем линию методом Брезенхема
            }

            isBresenhamDrawn = true; // Устанавливаем флаг для первой линии
        }

        // Обработчик клика по кнопке ЦДА
        private void btnCDA_Click(object sender, EventArgs e)
        {
            // Если обе линии нарисованы, очищаем панель перед рисованием новой
            if (isBresenhamDrawn && isCDADrawn)
            {
                ClearAll();
            }

            // Получаем координаты второй линии
            int x1 = int.Parse(txtX1.Text);
            int y1 = int.Parse(txtY1.Text);
            int x2 = int.Parse(txtX2.Text);
            int y2 = int.Parse(txtY2.Text);

            // Сохраняем координаты второй линии
            secondLinePoints[0] = new Point(x1, y1);
            secondLinePoints[1] = new Point(x2, y2);

            // Создаем объект Graphics для рисования
            using (Graphics g = panel1.CreateGraphics())
            {
                Pen pen = new Pen(Color.Blue, 2);  // Используем синюю ручку для рисования
                DrawCDALine(g, x1, y1, x2, y2, pen);  // Рисуем линию методом ЦДА
            }

            isCDADrawn = true; // Устанавливаем флаг для второй линии
        }

        // Метод рисования линии методом Брезенхема
        private void DrawBresenhamLine(Graphics g, int x1, int y1, int x2, int y2, Pen pen)
        {
            int dx = Math.Abs(x2 - x1);  // Вычисляем изменения по оси X
            int dy = Math.Abs(y2 - y1);  // Вычисляем изменения по оси Y
            int sx = (x1 < x2) ? 1 : -1; // Направление движения по оси X
            int sy = (y1 < y2) ? 1 : -1; // Направление движения по оси Y
            int err = dx - dy;  // Начальная ошибка

            while (true)
            {
                g.DrawRectangle(pen, x1, y1, 1, 1);  // Рисуем точку
                if (x1 == x2 && y1 == y2) break;  // Если достигли конечной точки, выходим из цикла
                int e2 = err * 2;
                if (e2 > -dy) { err -= dy; x1 += sx; }  // Корректировка ошибки по X
                if (e2 < dx) { err += dx; y1 += sy; }  // Корректировка ошибки по Y
            }
        }

        // Метод рисования линии методом ЦДА
        private void DrawCDALine(Graphics g, int x1, int y1, int x2, int y2, Pen pen)
        {
            float dx = x2 - x1;  // Изменение по оси X
            float dy = y2 - y1;  // Изменение по оси Y
            float steps = Math.Max(Math.Abs(dx), Math.Abs(dy));  // Количество шагов для рисования

            float xIncrement = dx / steps;  // Приращение по X на каждом шаге
            float yIncrement = dy / steps;  // Приращение по Y на каждом шаге

            float x = x1;
            float y = y1;

            for (int i = 0; i <= steps; i++)
            {
                g.DrawRectangle(pen, (int)x, (int)y, 1, 1);  // Рисуем точку на каждом шаге
                x += xIncrement;  // Обновляем X
                y += yIncrement;  // Обновляем Y
            }
        }

        // Очистка всех линий
        private void ClearAll()
        {
            panel1.Invalidate();  // Очищаем панель для удаления нарисованных линий
            isBresenhamDrawn = false;
            isCDADrawn = false;

            // Переносим вторую линию на место первой
            if (isCDADrawn)
            {
                firstLinePoints[0] = secondLinePoints[0];
                firstLinePoints[1] = secondLinePoints[1];
                isCDADrawn = false;
            }
        }

        // Обработчик для кнопки очистки
        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();  // Очищаем все линии
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Инициализация формы (можно добавить код для загрузки настроек или других операций)
        }
    }
}
