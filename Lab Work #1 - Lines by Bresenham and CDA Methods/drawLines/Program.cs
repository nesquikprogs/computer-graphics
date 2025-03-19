using System;
using System.Windows.Forms;

namespace LineDrawing
{
    static class Program
    {
        [STAThread]  // Указывает, что метод Main выполняется в однопоточном режиме (STA)
        static void Main()
        {
            Application.EnableVisualStyles();  // Включение визуальных стилей Windows
            Application.SetCompatibleTextRenderingDefault(false);  // Настройка рендеринга текста
            Application.Run(new Form1());  // Запуск главного окна приложения
        }
    }
}
