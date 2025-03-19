using System;
using System.Windows.Forms;

namespace LineDrawing
{
    static class Program
    {
        // Главная точка входа приложения
        [STAThread]
        static void Main()
        {
            // Настройка и запуск формы
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Запуск основной формы
        }
    }
}
