namespace ZxDisAsm
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_enter = new System.Windows.Forms.Button();
            this.timer_flash = new System.Windows.Forms.Timer(this.components);
            this.btn_L = new System.Windows.Forms.Button();
            this.btn_K = new System.Windows.Forms.Button();
            this.btn_J = new System.Windows.Forms.Button();
            this.btn_U = new System.Windows.Forms.Button();
            this.btn_I = new System.Windows.Forms.Button();
            this.btn_O = new System.Windows.Forms.Button();
            this.btn_P = new System.Windows.Forms.Button();
            this.btn_H = new System.Windows.Forms.Button();
            this.btn_Y = new System.Windows.Forms.Button();
            this.btn_6 = new System.Windows.Forms.Button();
            this.btn_7 = new System.Windows.Forms.Button();
            this.btn_8 = new System.Windows.Forms.Button();
            this.btn_9 = new System.Windows.Forms.Button();
            this.btn_0 = new System.Windows.Forms.Button();
            this.btn_B = new System.Windows.Forms.Button();
            this.btn_N = new System.Windows.Forms.Button();
            this.btn_M = new System.Windows.Forms.Button();
            this.btn_SymShift = new System.Windows.Forms.Button();
            this.btn_break = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(234, 468);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_enter
            // 
            this.btn_enter.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_enter.Location = new System.Drawing.Point(716, 370);
            this.btn_enter.Name = "btn_enter";
            this.btn_enter.Size = new System.Drawing.Size(75, 23);
            this.btn_enter.TabIndex = 1;
            this.btn_enter.Text = "Enter";
            this.btn_enter.UseVisualStyleBackColor = true;
            this.btn_enter.Click += new System.EventHandler(this.btn_enter_Click);
            // 
            // timer_flash
            // 
            this.timer_flash.Interval = 200;
            this.timer_flash.Tick += new System.EventHandler(this.timerflash_Tick);
            // 
            // btn_L
            // 
            this.btn_L.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_L.Location = new System.Drawing.Point(635, 370);
            this.btn_L.Name = "btn_L";
            this.btn_L.Size = new System.Drawing.Size(75, 23);
            this.btn_L.TabIndex = 2;
            this.btn_L.Text = "L / LET";
            this.btn_L.UseVisualStyleBackColor = true;
            this.btn_L.Click += new System.EventHandler(this.btn_L_Click);
            // 
            // btn_K
            // 
            this.btn_K.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_K.Location = new System.Drawing.Point(554, 370);
            this.btn_K.Name = "btn_K";
            this.btn_K.Size = new System.Drawing.Size(75, 23);
            this.btn_K.TabIndex = 3;
            this.btn_K.Text = "K / LIST";
            this.btn_K.UseVisualStyleBackColor = true;
            this.btn_K.Click += new System.EventHandler(this.btn_K_Click);
            // 
            // btn_J
            // 
            this.btn_J.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_J.Location = new System.Drawing.Point(473, 370);
            this.btn_J.Name = "btn_J";
            this.btn_J.Size = new System.Drawing.Size(75, 23);
            this.btn_J.TabIndex = 4;
            this.btn_J.Text = "J / LOAD";
            this.btn_J.UseVisualStyleBackColor = true;
            this.btn_J.Click += new System.EventHandler(this.btn_J_Click);
            // 
            // btn_U
            // 
            this.btn_U.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_U.Location = new System.Drawing.Point(473, 341);
            this.btn_U.Name = "btn_U";
            this.btn_U.Size = new System.Drawing.Size(75, 23);
            this.btn_U.TabIndex = 8;
            this.btn_U.Text = "U / IF";
            this.btn_U.UseVisualStyleBackColor = true;
            this.btn_U.Click += new System.EventHandler(this.btn_U_Click);
            // 
            // btn_I
            // 
            this.btn_I.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_I.Location = new System.Drawing.Point(554, 341);
            this.btn_I.Name = "btn_I";
            this.btn_I.Size = new System.Drawing.Size(75, 23);
            this.btn_I.TabIndex = 7;
            this.btn_I.Text = "I / INPUT";
            this.btn_I.UseVisualStyleBackColor = true;
            this.btn_I.Click += new System.EventHandler(this.btn_I_Click);
            // 
            // btn_O
            // 
            this.btn_O.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_O.Location = new System.Drawing.Point(635, 341);
            this.btn_O.Name = "btn_O";
            this.btn_O.Size = new System.Drawing.Size(75, 23);
            this.btn_O.TabIndex = 6;
            this.btn_O.Text = "O / POKE";
            this.btn_O.UseVisualStyleBackColor = true;
            this.btn_O.Click += new System.EventHandler(this.btn_O_Click);
            // 
            // btn_P
            // 
            this.btn_P.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_P.Location = new System.Drawing.Point(716, 341);
            this.btn_P.Name = "btn_P";
            this.btn_P.Size = new System.Drawing.Size(75, 23);
            this.btn_P.TabIndex = 5;
            this.btn_P.Text = "P / PRINT";
            this.btn_P.UseVisualStyleBackColor = true;
            this.btn_P.Click += new System.EventHandler(this.btn_P_Click);
            // 
            // btn_H
            // 
            this.btn_H.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_H.Location = new System.Drawing.Point(392, 370);
            this.btn_H.Name = "btn_H";
            this.btn_H.Size = new System.Drawing.Size(75, 23);
            this.btn_H.TabIndex = 9;
            this.btn_H.Text = "H / GOSUB";
            this.btn_H.UseVisualStyleBackColor = true;
            this.btn_H.Click += new System.EventHandler(this.btn_H_Click);
            // 
            // btn_Y
            // 
            this.btn_Y.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Y.Location = new System.Drawing.Point(392, 341);
            this.btn_Y.Name = "btn_Y";
            this.btn_Y.Size = new System.Drawing.Size(75, 23);
            this.btn_Y.TabIndex = 10;
            this.btn_Y.Text = "Y / RETURN";
            this.btn_Y.UseVisualStyleBackColor = true;
            this.btn_Y.Click += new System.EventHandler(this.btn_Y_Click);
            // 
            // btn_6
            // 
            this.btn_6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_6.Location = new System.Drawing.Point(392, 312);
            this.btn_6.Name = "btn_6";
            this.btn_6.Size = new System.Drawing.Size(75, 23);
            this.btn_6.TabIndex = 15;
            this.btn_6.Text = "6";
            this.btn_6.UseVisualStyleBackColor = true;
            this.btn_6.Click += new System.EventHandler(this.btn_6_Click);
            // 
            // btn_7
            // 
            this.btn_7.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_7.Location = new System.Drawing.Point(473, 312);
            this.btn_7.Name = "btn_7";
            this.btn_7.Size = new System.Drawing.Size(75, 23);
            this.btn_7.TabIndex = 14;
            this.btn_7.Text = "7";
            this.btn_7.UseVisualStyleBackColor = true;
            this.btn_7.Click += new System.EventHandler(this.btn_7_Click);
            // 
            // btn_8
            // 
            this.btn_8.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_8.Location = new System.Drawing.Point(554, 312);
            this.btn_8.Name = "btn_8";
            this.btn_8.Size = new System.Drawing.Size(75, 23);
            this.btn_8.TabIndex = 13;
            this.btn_8.Text = "8";
            this.btn_8.UseVisualStyleBackColor = true;
            this.btn_8.Click += new System.EventHandler(this.btn_8_Click);
            // 
            // btn_9
            // 
            this.btn_9.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_9.Location = new System.Drawing.Point(635, 312);
            this.btn_9.Name = "btn_9";
            this.btn_9.Size = new System.Drawing.Size(75, 23);
            this.btn_9.TabIndex = 12;
            this.btn_9.Text = "9";
            this.btn_9.UseVisualStyleBackColor = true;
            this.btn_9.Click += new System.EventHandler(this.btn_9_Click);
            // 
            // btn_0
            // 
            this.btn_0.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_0.Location = new System.Drawing.Point(716, 312);
            this.btn_0.Name = "btn_0";
            this.btn_0.Size = new System.Drawing.Size(75, 23);
            this.btn_0.TabIndex = 11;
            this.btn_0.Text = "0";
            this.btn_0.UseVisualStyleBackColor = true;
            this.btn_0.Click += new System.EventHandler(this.btn_0_Click);
            // 
            // btn_B
            // 
            this.btn_B.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_B.Location = new System.Drawing.Point(392, 399);
            this.btn_B.Name = "btn_B";
            this.btn_B.Size = new System.Drawing.Size(75, 23);
            this.btn_B.TabIndex = 20;
            this.btn_B.Text = "B/BORDER";
            this.btn_B.UseVisualStyleBackColor = true;
            this.btn_B.Click += new System.EventHandler(this.btn_B_Click);
            // 
            // btn_N
            // 
            this.btn_N.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_N.Location = new System.Drawing.Point(473, 399);
            this.btn_N.Name = "btn_N";
            this.btn_N.Size = new System.Drawing.Size(75, 23);
            this.btn_N.TabIndex = 19;
            this.btn_N.Text = "N / NEXT";
            this.btn_N.UseVisualStyleBackColor = true;
            this.btn_N.Click += new System.EventHandler(this.btn_N_Click);
            // 
            // btn_M
            // 
            this.btn_M.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_M.Location = new System.Drawing.Point(554, 399);
            this.btn_M.Name = "btn_M";
            this.btn_M.Size = new System.Drawing.Size(75, 23);
            this.btn_M.TabIndex = 18;
            this.btn_M.Text = "M / PAUSE";
            this.btn_M.UseVisualStyleBackColor = true;
            this.btn_M.Click += new System.EventHandler(this.btn_M_Click);
            // 
            // btn_SymShift
            // 
            this.btn_SymShift.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_SymShift.Location = new System.Drawing.Point(635, 399);
            this.btn_SymShift.Name = "btn_SymShift";
            this.btn_SymShift.Size = new System.Drawing.Size(75, 23);
            this.btn_SymShift.TabIndex = 17;
            this.btn_SymShift.Text = "SYMBOL";
            this.btn_SymShift.UseVisualStyleBackColor = true;
            this.btn_SymShift.Click += new System.EventHandler(this.btn_SymShift_Click);
            // 
            // btn_break
            // 
            this.btn_break.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_break.Location = new System.Drawing.Point(716, 399);
            this.btn_break.Name = "btn_break";
            this.btn_break.Size = new System.Drawing.Size(75, 23);
            this.btn_break.TabIndex = 16;
            this.btn_break.Text = "Break Space";
            this.btn_break.UseVisualStyleBackColor = true;
            this.btn_break.Click += new System.EventHandler(this.btn_break_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.btn_enter;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 522);
            this.Controls.Add(this.btn_B);
            this.Controls.Add(this.btn_N);
            this.Controls.Add(this.btn_M);
            this.Controls.Add(this.btn_SymShift);
            this.Controls.Add(this.btn_break);
            this.Controls.Add(this.btn_6);
            this.Controls.Add(this.btn_7);
            this.Controls.Add(this.btn_8);
            this.Controls.Add(this.btn_9);
            this.Controls.Add(this.btn_0);
            this.Controls.Add(this.btn_Y);
            this.Controls.Add(this.btn_H);
            this.Controls.Add(this.btn_U);
            this.Controls.Add(this.btn_I);
            this.Controls.Add(this.btn_O);
            this.Controls.Add(this.btn_P);
            this.Controls.Add(this.btn_J);
            this.Controls.Add(this.btn_K);
            this.Controls.Add(this.btn_L);
            this.Controls.Add(this.btn_enter);
            this.Controls.Add(this.button1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_enter;
        private System.Windows.Forms.Timer timer_flash;
        private System.Windows.Forms.Button btn_L;
        private System.Windows.Forms.Button btn_K;
        private System.Windows.Forms.Button btn_J;
        private System.Windows.Forms.Button btn_U;
        private System.Windows.Forms.Button btn_I;
        private System.Windows.Forms.Button btn_O;
        private System.Windows.Forms.Button btn_P;
        private System.Windows.Forms.Button btn_H;
        private System.Windows.Forms.Button btn_Y;
        private System.Windows.Forms.Button btn_6;
        private System.Windows.Forms.Button btn_7;
        private System.Windows.Forms.Button btn_8;
        private System.Windows.Forms.Button btn_9;
        private System.Windows.Forms.Button btn_0;
        private System.Windows.Forms.Button btn_B;
        private System.Windows.Forms.Button btn_N;
        private System.Windows.Forms.Button btn_M;
        private System.Windows.Forms.Button btn_SymShift;
        private System.Windows.Forms.Button btn_break;
    }
}

