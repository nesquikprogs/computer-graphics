using System.Windows.Forms;

namespace LineDrawing
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnDrawEllipse;
        private Button btnClear;
        private Panel panel1;
        private TextBox txtWidth; // Поле для ввода горизонтального диаметра
        private TextBox txtHeight; // Поле для ввода вертикального диаметра
        private Label lblWidth; // Метка для горизонтального диаметра
        private Label lblHeight; // Метка для вертикального диаметра

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDrawEllipse = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Location = new System.Drawing.Point(11, 51);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(237, 231);
            this.panel1.TabIndex = 0;
            // 
            // btnDrawEllipse
            // 
            this.btnDrawEllipse.Location = new System.Drawing.Point(112, 23);
            this.btnDrawEllipse.Name = "btnDrawEllipse";
            this.btnDrawEllipse.Size = new System.Drawing.Size(66, 22);
            this.btnDrawEllipse.TabIndex = 1;
            this.btnDrawEllipse.Text = "Рисовать эллипс";
            this.btnDrawEllipse.UseVisualStyleBackColor = true;
            this.btnDrawEllipse.Click += new System.EventHandler(this.btnDrawEllipse_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(182, 23);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(66, 22);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Очистить";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(10, 25);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(45, 20);
            this.txtWidth.TabIndex = 3;
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(61, 25);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(45, 20);
            this.txtHeight.TabIndex = 4;
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(9, 9);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(46, 13);
            this.lblWidth.TabIndex = 5;
            this.lblWidth.Text = "Ширина";
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(61, 9);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(45, 13);
            this.lblHeight.TabIndex = 6;
            this.lblHeight.Text = "Высота";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(260, 296);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.btnDrawEllipse);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Рисование эллипса";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
