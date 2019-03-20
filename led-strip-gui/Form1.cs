﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace LedStripGui
{
    public partial class Form1 : Form
    {
        public Settings settings;
        public LEDController ledController;

        public Form1()
        {
            this.InitializeComponent();

            var modes = Enum.GetNames(typeof(ArduinoCodes.Mode));
            this.comboBox_mode.Items.AddRange(modes);

            var palettes = Enum.GetNames(typeof(ArduinoCodes.Palette));
            this.comboBox_palette.Items.AddRange(palettes);

            var opened = Serial.Open();
            this.buttonOpenSerial.Enabled = !opened;

            this.settings = new Settings(ArduinoCodes.LED_COUNT);

            this.textBox_brightness.Text = this.settings.Brightness.ToString();
            this.textBox_color_r.Text = this.settings.color.R.ToString();
            this.textBox_color_g.Text = this.settings.color.G.ToString();
            this.textBox_color_b.Text = this.settings.color.B.ToString();
            this.textBox_updates.Text = this.settings.UpdatesPerSecond.ToString();
            this.textBox_paletteChangeDivider.Text = this.settings.PaletteChangeDivider.ToString();

            this.comboBox_mode.SelectedIndex = (int)this.settings.mode;
            this.comboBox_palette.SelectedIndex = (int)this.settings.palette;

            this.button_ScreenColorStart.Enabled = false;
            this.button_ScreenColorStop.Enabled = false;
        }

        private void onChangeModes(ArduinoCodes.Mode mode)
        {
            this.button_ScreenColorStart.Enabled = (mode == ArduinoCodes.Mode.Controlled);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Serial.Close();
        }

        private void textbox_numberOnly_keyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
             (e.KeyChar == '.') && (e.KeyChar == '-')) // ignore . and -
            {
                e.Handled = true;
                return;
            }
        }

        private void textbox_numberOnly_LeaveFocus(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out int number))
            {
                if (number > 255)
                {
                    number = 255;
                }
                if (number < 0)
                {
                    number = 0;
                }
                textBox.Text = number.ToString();
            }
            else
            {
                MessageBox.Show("Could not Parse Number value in " + textBox.Name + "\n" + textBox.Text);
                textBox.Text = "0";
            }
        }
        private void textbox_numberOnly_LeaveFocus_Divider(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out int number))
            {
                if (number > Settings.MAX_PALETTE_CHANGE_DIVIDER)
                {
                    number = Settings.MAX_PALETTE_CHANGE_DIVIDER;
                }
                if (number < Settings.MIN_PALETTE_CHANGE_DIVIDER)
                {
                    number = Settings.MIN_PALETTE_CHANGE_DIVIDER;
                }
                textBox.Text = number.ToString();
            }
            else
            {
                MessageBox.Show("Could not Parse Number value in " + textBox.Name + "\n" + textBox.Text);
                textBox.Text = "0";
            }
        }

        private void buttonOpenSerial_Click(object sender, System.EventArgs e)
        {
            var opened = Serial.Open();
            this.buttonOpenSerial.Enabled = !opened;
        }

        private void button_brightness_Click(object sender, System.EventArgs e)
        {
            this.settings.Brightness = int.Parse(this.textBox_brightness.Text);
            Serial.SendValue((byte)this.settings.Brightness, ArduinoCodes.MessageType.BRIGHTNESS);
        }

        private void button_mode_Click(object sender, EventArgs e)
        {
            this.settings.mode = (ArduinoCodes.Mode)this.comboBox_mode.SelectedIndex;

            Serial.SendValue((byte)this.settings.mode, ArduinoCodes.MessageType.MODE);

            this.onChangeModes(this.settings.mode);
        }

        private void button_color_Click(object sender, EventArgs e)
        {
            var r = int.Parse(this.textBox_color_r.Text);
            var g = int.Parse(this.textBox_color_g.Text);
            var b = int.Parse(this.textBox_color_b.Text);
            this.settings.color = Color.FromArgb(r, g, b);
            Serial.SendColor(this.settings.color);
        }

        private void button_palette_Click(object sender, EventArgs e)
        {
            this.settings.palette = (ArduinoCodes.Palette)this.comboBox_palette.SelectedIndex;

            Serial.SendValue((byte)this.settings.palette, ArduinoCodes.MessageType.PALETTE);
        }

        private void button_updates_Click(object sender, EventArgs e)
        {
            this.settings.UpdatesPerSecond = int.Parse(this.textBox_updates.Text);

            Serial.SendValue((byte)this.settings.UpdatesPerSecond, ArduinoCodes.MessageType.UPDATES_PER_SECOND);
        }

        private void button_paletteChangeDivider_Click(object sender, EventArgs e)
        {
            this.settings.PaletteChangeDivider = int.Parse(this.textBox_paletteChangeDivider.Text);

            Serial.SendValue((short)this.settings.PaletteChangeDivider, ArduinoCodes.MessageType.PALETTE_CHANGE_DIVIDER);
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            this.noFocus.Focus();
        }

        private void button_ScreenColorStart_Click(object sender, EventArgs e)
        {
            this.ledController = new HueCyleLEDController(this.settings);
            //this.ledController = new ScreenLEDController(this.settings);
            this.ledController.Start();

            this.button_mode.Enabled = false;
            this.button_ScreenColorStart.Enabled = false;
            this.button_ScreenColorStop.Enabled = true;
        }

        private void button_ScreenColorStop_Click(object sender, EventArgs e)
        {
            this.ledController.Stop();
            this.ledController = null;

            this.button_mode.Enabled = true;
            this.button_ScreenColorStart.Enabled = true;
            this.button_ScreenColorStop.Enabled = false;
        }
    }
}
