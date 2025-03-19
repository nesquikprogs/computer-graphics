using System;
using System.Drawing;
using System.Windows.Forms;

namespace LineDrawing
{
    public partial class Form1 : Form
    {
        private bool isBresenhamDrawn = false; // Флаг для первой линии (метод Брезенхема)
        private bool isCDADrawn = false; // Флаг для второй линии (метод ЦДА)
        private Point[] firstLinePoints = new Point[2]; // Массив для координат первой линии
        private Point[] secondLinePoints = new Point[2]; // Массив для координат второй линии

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBresenham_Click(object sender, EventArgs e)
        {
            if (isBresenhamDrawn && isCDADrawn)
            {
                // При рисовании третьей линии, убираем первую и сдвигаем вторую на место первой
                ClearAll();
            }

            // Получаем координаты для линии Брезенхема
            int x1 = int.Parse(txtX1.Text);
            int y1 = int.Parse(txtY1.Text);
            int x2 = int.Parse(txtX2.Text);
            int y2 = int.Parse(txtY2.Text);

            firstLinePoints[0] = new Point(x1, y1);
            firstLinePoints[1] = new Point(x2, y2);

            using (Graphics g = panel1.CreateGraphics())
            {
                Pen pen = new Pen(Color.Red, 2);
                DrawBresenhamLine(g, x1, y1, x2, y2, pen);
            }

            isBresenhamDrawn = true;
        }

        private void btnCDA_Click(object sender, EventArgs e)
        {
            if (isBresenhamDrawn && isCDADrawn)
            {
                // При рисовании третьей линии, убираем первую и сдвигаем вторую на место первой
                ClearAll();
            }

            // Получаем координаты для линии ЦДА
            int x1 = int.Parse(txtX1.Text);
            int y1 = int.Parse(txtY1.Text);
            int x2 = int.Parse(txtX2.Text);
            int y2 = int.Parse(txtY2.Text);

            secondLinePoints[0] = new Point(x1, y1);
            secondLinePoints[1] = new Point(x2, y2);

            using (Graphics g = panel1.CreateGraphics())
            {
                Pen pen = new Pen(Color.Blue, 2);
                DrawCDALine(g, x1, y1, x2, y2, pen);
            }

            isCDADrawn = true;
        }

        // Метод для рисования линии методом Брезенхема
        private void DrawBresenhamLine(Graphics g, int x1, int y1, int x2, int y2, Pen pen)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                g.DrawRectangle(pen, x1, y1, 1, 1);
                if (x1 == x2 && y1 == y2) break;
                int e2 = err * 2;
                if (e2 > -dy) { err -= dy; x1 += sx; }
                if (e2 < dx) { err += dx; y1 += sy; }
            }
        }

        // Метод для рисования линии методом ЦДА
        private void DrawCDALine(Graphics g, int x1, int y1, int x2, int y2, Pen pen)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            float steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float xIncrement = dx / steps;
            float yIncrement = dy / steps;

            float x = x1;
            float y = y1;

            for (int i = 0; i <= steps; i++)
            {
                g.DrawRectangle(pen, (int)x, (int)y, 1, 1);
                x += xIncrement;
                y += yIncrement;
            }
        }

        // Очистка всех линий
        private void ClearAll()
        {
            // Очищаем панель, чтобы удалить все нарисованные линии
            panel1.Invalidate();
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

        // Очистка экрана, чтобы сбросить рисование, если необходимо
        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
