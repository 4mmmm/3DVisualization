namespace _3DVisualization
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("华文新魏", 13F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label1.Location = new System.Drawing.Point(253, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("华文新魏", 13F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label2.Location = new System.Drawing.Point(253, 209);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "password:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(386, 164);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(150, 25);
            this.textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(386, 206);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(150, 25);
            this.textBox2.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 15F);
            this.button1.Location = new System.Drawing.Point(340, 259);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 41);
            this.button1.TabIndex = 4;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("华文行楷", 20F);
            this.label3.Location = new System.Drawing.Point(440, 391);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(316, 36);
            this.label3.TabIndex = 6;
            this.label3.Text = "——made by lai jian qiao";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("华文行楷", 20F);
            this.label4.Location = new System.Drawing.Point(229, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(321, 36);
            this.label4.TabIndex = 7;
            this.label4.Text = "三维可视化软件开发";
            // 
            // skinEngine1
            // 
            this.skinEngine1.SerialNumber = "";
            this.skinEngine1.SkinFile = "E:\\程序设计实践\\皮肤控件\\皮肤\\Diamond\\DiamondBlue.ssk";
            this.skinEngine1.SkinStreamMain = ((System.IO.Stream)(resources.GetObject("skinEngine1.SkinStreamMain")));
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("华文行楷", 16F);
            this.button2.Location = new System.Drawing.Point(349, 108);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(76, 35);
            this.button2.TabIndex = 8;
            this.button2.Text = "换肤";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Sunisoft.IrisSkin.SkinEngine skinEngine1;
        private System.Windows.Forms.Button button2;
    }
}