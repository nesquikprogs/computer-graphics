using System.Windows.Forms;

namespace LineDrawing
{
    partial class Form1  // Частичный класс, содержащий объявление компонентов формы
    {
        // Компоненты формы (кнопки, текстовые поля, панель для рисования)
        private System.ComponentModel.IContainer components = null;
        private Button btnBresenham;
        private Button btnCDA;
        private Button btnClear;
        private Panel panel1;
        private TextBox txtX1, txtY1, txtX2, txtY2;
        private Label label1, label2, label3, label4;

        // Метод освобождения ресурсов формы
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Инициализация компонентов формы
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnBresenham = new System.Windows.Forms.Button();
            this.btnCDA = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtX1 = new System.Windows.Forms.TextBox();
            this.txtY1 = new System.Windows.Forms.TextBox();
            this.txtX2 = new System.Windows.Forms.TextBox();
            this.txtY2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // Панель для рисования
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Location = new System.Drawing.Point(11, 51);
            this.panel1.Size = new System.Drawing.Size(237, 231);

            // Кнопка "Брезенхем"
            this.btnBresenham.Location = new System.Drawing.Point(11, 288);
            this.btnBresenham.Size = new System.Drawing.Size(75, 23);
            this.btnBresenham.Text = "Брезенхем";
            this.btnBresenham.UseVisualStyleBackColor = true;
            this.btnBresenham.Click += new System.EventHandler(this.btnBresenham_Click);

            // Кнопка "ЦДА"
            this.btnCDA.Location = new System.Drawing.Point(92, 288);
            this.btnCDA.Size = new System.Drawing.Size(75, 23);
            this.btnCDA.Text = "ЦДА";
            this.btnCDA.UseVisualStyleBackColor = true;
            this.btnCDA.Click += new System.EventHandler(this.btnCDA_Click);

            // Кнопка "Очистить"
            this.btnClear.Location = new System.Drawing.Point(173, 288);
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.Text = "Очистить";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.ClearButton_Click);

            // Поля для ввода координат X1, Y1, X2, Y2
            this.txtX1.Location = new System.Drawing.Point(35, 25);
            this.txtX1.Size = new System.Drawing.Size(45, 20);
            this.txtY1.Location = new System.Drawing.Point(86, 25);
            this.txtY1.Size = new System.Drawing.Size(45, 20);
            this.txtX2.Location = new System.Drawing.Point(137, 25);
            this.txtX2.Size = new System.Drawing.Size(45, 20);
            this.txtY2.Location = new System.Drawing.Point(188, 25);
            this.txtY2.Size = new System.Drawing.Size(45, 20);

            // Метки для координат
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 9);
            this.label1.Text = "X1";
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 9);
            this.label2.Text = "Y1";
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(145, 9);
            this.label3.Text = "X2";
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(196, 9);
            this.label4.Text = "Y2";

            // Настройки формы
            this.ClientSize = new System.Drawing.Size(260, 320);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtY2);
            this.Controls.Add(this.txtX2);
            this.Controls.Add(this.txtY1);
            this.Controls.Add(this.txtX1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCDA);
            this.Controls.Add(this.btnBresenham);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Рисовалка";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
