using System;
using System.Windows.Forms;

namespace led_strip_gui
{
    public partial class Form1 : Form
    {
        public Settings settings;

        public Form1()
        {
            this.InitializeComponent();

            var modes = Enum.GetNames(typeof(ArduinoCodes.Mode));
            this.comboBox_mode.Items.AddRange(modes);
            this.comboBox_mode.SelectedIndex = 0;

            var palettes = Enum.GetNames(typeof(ArduinoCodes.Palette));
            this.comboBox_palette.Items.AddRange(palettes);
            this.comboBox_palette.SelectedIndex = 0;

            var opened = Serial.Open();
            this.buttonOpenSerial.Enabled = !opened;

            this.settings = new Settings();
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

        private void textbox_numberOnly_TextChanged(object sender, EventArgs e)
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

        private void buttonOpenSerial_Click(object sender, System.EventArgs e)
        {
            var opened = Serial.Open();
            this.buttonOpenSerial.Enabled = !opened;
        }

        private void button_brightness_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(this.textBox_brightness.Text, out int b))
            {
                this.settings.Brightness = b;

                var msg = ArduinoCodes.SetBrightnessText(this.settings.Brightness);
                Serial.Send(msg);
            }
            else
            {
                MessageBox.Show("Could not Parse brightness value\n" + this.textBox_brightness.Text);
                this.textBox_brightness.Text = "0";
            }
        }

        private void button_mode_Click(object sender, EventArgs e)
        {
            this.settings.mode = (ArduinoCodes.Mode)this.comboBox_mode.SelectedIndex;
            var msg = ArduinoCodes.SetModeText(this.settings.mode);
            Serial.Send(msg);
        }

        private void button_color_Click(object sender, EventArgs e)
        {
            // get color
            var msg = ArduinoCodes.SetColorText(this.settings.color);
            Serial.Send(msg);
            throw new NotImplementedException();
        }

        private void button_palette_Click(object sender, EventArgs e)
        {
            this.settings.palette = (ArduinoCodes.Palette)this.comboBox_palette.SelectedIndex;
            var msg = ArduinoCodes.SetPaletteText(this.settings.palette);
            Serial.Send(msg);
        }
    }
}
