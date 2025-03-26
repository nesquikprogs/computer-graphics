namespace EllipseDrawing
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnDrawEllipse;
        private System.Windows.Forms.Button btnDrawCircle;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.TextBox txtRadius;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblRadius;

        private void InitializeComponent()
        {
            this.btnDrawEllipse = new System.Windows.Forms.Button();
            this.btnDrawCircle = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.txtRadius = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblRadius = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnDrawEllipse
            // 
            this.btnDrawEllipse.Location = new System.Drawing.Point(10, 62);
            this.btnDrawEllipse.Name = "btnDrawEllipse";
            this.btnDrawEllipse.Size = new System.Drawing.Size(150, 23);
            this.btnDrawEllipse.TabIndex = 0;
            this.btnDrawEllipse.Text = "Рисовать эллипс";
            this.btnDrawEllipse.Click += new System.EventHandler(this.btnDrawEllipse_Click);
            // 
            // btnDrawCircle
            // 
            this.btnDrawCircle.Location = new System.Drawing.Point(173, 62);
            this.btnDrawCircle.Name = "btnDrawCircle";
            this.btnDrawCircle.Size = new System.Drawing.Size(150, 23);
            this.btnDrawCircle.TabIndex = 1;
            this.btnDrawCircle.Text = "Рисовать окружность";
            this.btnDrawCircle.Click += new System.EventHandler(this.btnDrawCircle_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(173, 414);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(150, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Очистить";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(60, 10);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(100, 20);
            this.txtWidth.TabIndex = 3;
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(60, 33);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(100, 20);
            this.txtHeight.TabIndex = 4;
            // 
            // txtRadius
            // 
            this.txtRadius.Location = new System.Drawing.Point(223, 10);
            this.txtRadius.Name = "txtRadius";
            this.txtRadius.Size = new System.Drawing.Size(100, 20);
            this.txtRadius.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(10, 100);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(312, 308);
            this.panel1.TabIndex = 6;
            // 
            // lblWidth
            // 
            this.lblWidth.Location = new System.Drawing.Point(12, 13);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(100, 23);
            this.lblWidth.TabIndex = 7;
            this.lblWidth.Text = "Ширина:";
            // 
            // lblHeight
            // 
            this.lblHeight.Location = new System.Drawing.Point(12, 36);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(100, 23);
            this.lblHeight.TabIndex = 8;
            this.lblHeight.Text = "Высота:";
            // 
            // lblRadius
            // 
            this.lblRadius.Location = new System.Drawing.Point(170, 13);
            this.lblRadius.Name = "lblRadius";
            this.lblRadius.Size = new System.Drawing.Size(100, 23);
            this.lblRadius.TabIndex = 9;
            this.lblRadius.Text = "Радиус:";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(331, 447);
            this.Controls.Add(this.txtRadius);
            this.Controls.Add(this.btnDrawEllipse);
            this.Controls.Add(this.btnDrawCircle);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.lblRadius);
            this.Name = "Form1";
            this.Text = "Элипс и Окружность методом Брезенхема";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
