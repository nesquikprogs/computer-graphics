using System.Windows.Forms;

namespace LineDrawing
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtX1;
        private TextBox txtY1;
        private TextBox txtX2;
        private TextBox txtY2;
        private Button btnBresenham;
        private Button btnCDA;
        private Panel panel1;

        private void InitializeComponent()
        {
            this.txtX1 = new System.Windows.Forms.TextBox();
            this.txtY1 = new System.Windows.Forms.TextBox();
            this.txtX2 = new System.Windows.Forms.TextBox();
            this.txtY2 = new System.Windows.Forms.TextBox();
            this.btnBresenham = new System.Windows.Forms.Button();
            this.btnCDA = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtX1
            // 
            this.txtX1.Location = new System.Drawing.Point(92, 16);
            this.txtX1.Name = "txtX1";
            this.txtX1.Size = new System.Drawing.Size(40, 20);
            this.txtX1.TabIndex = 0;
            // 
            // txtY1
            // 
            this.txtY1.Location = new System.Drawing.Point(92, 39);
            this.txtY1.Name = "txtY1";
            this.txtY1.Size = new System.Drawing.Size(40, 20);
            this.txtY1.TabIndex = 1;
            // 
            // txtX2
            // 
            this.txtX2.Location = new System.Drawing.Point(174, 16);
            this.txtX2.Name = "txtX2";
            this.txtX2.Size = new System.Drawing.Size(40, 20);
            this.txtX2.TabIndex = 2;
            // 
            // txtY2
            // 
            this.txtY2.Location = new System.Drawing.Point(174, 39);
            this.txtY2.Name = "txtY2";
            this.txtY2.Size = new System.Drawing.Size(40, 20);
            this.txtY2.TabIndex = 3;
            // 
            // btnBresenham
            // 
            this.btnBresenham.Location = new System.Drawing.Point(174, 321);
            this.btnBresenham.Name = "btnBresenham";
            this.btnBresenham.Size = new System.Drawing.Size(71, 23);
            this.btnBresenham.TabIndex = 4;
            this.btnBresenham.Text = "Брезенхам";
            this.btnBresenham.UseVisualStyleBackColor = true;
            this.btnBresenham.Click += new System.EventHandler(this.btnBresenham_Click);
            // 
            // btnCDA
            // 
            this.btnCDA.Location = new System.Drawing.Point(40, 321);
            this.btnCDA.Name = "btnCDA";
            this.btnCDA.Size = new System.Drawing.Size(71, 23);
            this.btnCDA.TabIndex = 5;
            this.btnCDA.Text = "ЦДА";
            this.btnCDA.UseVisualStyleBackColor = true;
            this.btnCDA.Click += new System.EventHandler(this.btnCDA_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(16, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(250, 250);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(68, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "x1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(68, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "y1";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(150, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "x2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "y2";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 361);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCDA);
            this.Controls.Add(this.btnBresenham);
            this.Controls.Add(this.txtY2);
            this.Controls.Add(this.txtX2);
            this.Controls.Add(this.txtY1);
            this.Controls.Add(this.txtX1);
            this.Name = "Form1";
            this.Text = "Линии";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label label1;
        private Label label5;
        private Label label7;
        private Label label2;
    }
}
