namespace led_strip_gui
{
    partial class Form1
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
            this.textBox_brightness = new System.Windows.Forms.TextBox();
            this.button_brightness = new System.Windows.Forms.Button();
            this.buttonOpenSerial = new System.Windows.Forms.Button();
            this.comboBox_mode = new System.Windows.Forms.ComboBox();
            this.button_mode = new System.Windows.Forms.Button();
            this.button_color = new System.Windows.Forms.Button();
            this.button_palette = new System.Windows.Forms.Button();
            this.comboBox_palette = new System.Windows.Forms.ComboBox();
            this.textBox_color_r = new System.Windows.Forms.TextBox();
            this.textBox_color_g = new System.Windows.Forms.TextBox();
            this.textBox_color_b = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox_brightness
            // 
            this.textBox_brightness.Location = new System.Drawing.Point(12, 110);
            this.textBox_brightness.Name = "textBox_brightness";
            this.textBox_brightness.Size = new System.Drawing.Size(90, 20);
            this.textBox_brightness.TabIndex = 0;
            this.textBox_brightness.Text = "0";
            this.textBox_brightness.TextChanged += new System.EventHandler(this.textbox_numberOnly_TextChanged);
            this.textBox_brightness.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_numberOnly_keyPress);
            // 
            // button_brightness
            // 
            this.button_brightness.Location = new System.Drawing.Point(255, 108);
            this.button_brightness.Name = "button_brightness";
            this.button_brightness.Size = new System.Drawing.Size(100, 23);
            this.button_brightness.TabIndex = 1;
            this.button_brightness.Text = "Set Brightness";
            this.button_brightness.UseVisualStyleBackColor = true;
            this.button_brightness.Click += new System.EventHandler(this.button_brightness_Click);
            // 
            // buttonOpenSerial
            // 
            this.buttonOpenSerial.Location = new System.Drawing.Point(255, 30);
            this.buttonOpenSerial.Name = "buttonOpenSerial";
            this.buttonOpenSerial.Size = new System.Drawing.Size(95, 23);
            this.buttonOpenSerial.TabIndex = 3;
            this.buttonOpenSerial.Text = "Open Serial";
            this.buttonOpenSerial.UseVisualStyleBackColor = true;
            this.buttonOpenSerial.Click += new System.EventHandler(this.buttonOpenSerial_Click);
            // 
            // comboBox_mode
            // 
            this.comboBox_mode.FormattingEnabled = true;
            this.comboBox_mode.Location = new System.Drawing.Point(12, 81);
            this.comboBox_mode.Name = "comboBox_mode";
            this.comboBox_mode.Size = new System.Drawing.Size(135, 21);
            this.comboBox_mode.TabIndex = 4;
            // 
            // button_mode
            // 
            this.button_mode.Location = new System.Drawing.Point(255, 79);
            this.button_mode.Name = "button_mode";
            this.button_mode.Size = new System.Drawing.Size(100, 23);
            this.button_mode.TabIndex = 5;
            this.button_mode.Text = "Set Mode";
            this.button_mode.UseVisualStyleBackColor = true;
            this.button_mode.Click += new System.EventHandler(this.button_mode_Click);
            // 
            // button_color
            // 
            this.button_color.Enabled = false;
            this.button_color.Location = new System.Drawing.Point(255, 137);
            this.button_color.Name = "button_color";
            this.button_color.Size = new System.Drawing.Size(100, 23);
            this.button_color.TabIndex = 6;
            this.button_color.Text = "Set Color";
            this.button_color.UseVisualStyleBackColor = true;
            this.button_color.Click += new System.EventHandler(this.button_color_Click);
            // 
            // button_palette
            // 
            this.button_palette.Location = new System.Drawing.Point(255, 166);
            this.button_palette.Name = "button_palette";
            this.button_palette.Size = new System.Drawing.Size(100, 23);
            this.button_palette.TabIndex = 7;
            this.button_palette.Text = "Set Palette";
            this.button_palette.UseVisualStyleBackColor = true;
            this.button_palette.Click += new System.EventHandler(this.button_palette_Click);
            // 
            // comboBox_palette
            // 
            this.comboBox_palette.FormattingEnabled = true;
            this.comboBox_palette.Location = new System.Drawing.Point(12, 168);
            this.comboBox_palette.Name = "comboBox_palette";
            this.comboBox_palette.Size = new System.Drawing.Size(237, 21);
            this.comboBox_palette.TabIndex = 8;
            // 
            // textBox_color_r
            // 
            this.textBox_color_r.Location = new System.Drawing.Point(33, 139);
            this.textBox_color_r.Name = "textBox_color_r";
            this.textBox_color_r.Size = new System.Drawing.Size(50, 20);
            this.textBox_color_r.TabIndex = 9;
            this.textBox_color_r.Text = "255";
            this.textBox_color_r.TextChanged += new System.EventHandler(this.textbox_numberOnly_TextChanged);
            this.textBox_color_r.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_numberOnly_keyPress);
            // 
            // textBox_color_g
            // 
            this.textBox_color_g.Location = new System.Drawing.Point(108, 139);
            this.textBox_color_g.Name = "textBox_color_g";
            this.textBox_color_g.Size = new System.Drawing.Size(50, 20);
            this.textBox_color_g.TabIndex = 10;
            this.textBox_color_g.Text = "255";
            this.textBox_color_g.TextChanged += new System.EventHandler(this.textbox_numberOnly_TextChanged);
            this.textBox_color_g.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_numberOnly_keyPress);
            // 
            // textBox_color_b
            // 
            this.textBox_color_b.Location = new System.Drawing.Point(184, 139);
            this.textBox_color_b.Name = "textBox_color_b";
            this.textBox_color_b.Size = new System.Drawing.Size(50, 20);
            this.textBox_color_b.TabIndex = 11;
            this.textBox_color_b.Text = "255";
            this.textBox_color_b.TextChanged += new System.EventHandler(this.textbox_numberOnly_TextChanged);
            this.textBox_color_b.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textbox_numberOnly_keyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "R";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 142);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "B";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "G";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 205);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_color_b);
            this.Controls.Add(this.textBox_color_g);
            this.Controls.Add(this.textBox_color_r);
            this.Controls.Add(this.comboBox_palette);
            this.Controls.Add(this.button_palette);
            this.Controls.Add(this.button_color);
            this.Controls.Add(this.button_mode);
            this.Controls.Add(this.comboBox_mode);
            this.Controls.Add(this.buttonOpenSerial);
            this.Controls.Add(this.button_brightness);
            this.Controls.Add(this.textBox_brightness);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_brightness;
        private System.Windows.Forms.Button button_brightness;
        private System.Windows.Forms.Button buttonOpenSerial;
        private System.Windows.Forms.ComboBox comboBox_mode;
        private System.Windows.Forms.Button button_mode;
        private System.Windows.Forms.Button button_color;
        private System.Windows.Forms.Button button_palette;
        private System.Windows.Forms.ComboBox comboBox_palette;
        private System.Windows.Forms.TextBox textBox_color_r;
        private System.Windows.Forms.TextBox textBox_color_g;
        private System.Windows.Forms.TextBox textBox_color_b;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

