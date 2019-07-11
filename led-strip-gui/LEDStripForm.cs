using led_strip_simulation;
using LedStrip;
using LedStrip.Controllers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LedStripGui
{
    public partial class LEDStripForm : Form
    {
        private Settings settings;
        private LEDController ledController;
        private ILedMessageSender messageSender;

        public LEDStripForm()
        {
            this.InitializeComponent();

            var modes = Enum.GetNames(typeof(Codes.Mode));
            this.comboBox_mode.Items.AddRange(modes);

            var palettes = Enum.GetNames(typeof(Codes.Palette));
            this.comboBox_palette.Items.AddRange(palettes);

            //this.messageSender = Serial.Instance;
            this.messageSender = new SimulationMessageSender();
            var opened = this.messageSender.Open();
            this.buttonOpenSerial.Enabled = !opened;

            this.settings = new Settings(Codes.LED_COUNT);

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

        private void onChangeModes(Codes.Mode mode)
        {
            this.button_ScreenColorStart.Enabled = (mode == Codes.Mode.Controlled);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.messageSender.Close();
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
                if (number > Codes.MAX_PALETTE_CHANGE_DIVIDER)
                {
                    number = Codes.MAX_PALETTE_CHANGE_DIVIDER;
                }
                if (number < Codes.MIN_PALETTE_CHANGE_DIVIDER)
                {
                    number = Codes.MIN_PALETTE_CHANGE_DIVIDER;
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
            var opened = this.messageSender.Open();
            this.buttonOpenSerial.Enabled = !opened;
        }

        private void button_brightness_Click(object sender, System.EventArgs e)
        {
            this.settings.Brightness = int.Parse(this.textBox_brightness.Text);
            this.messageSender.SendValue((byte)this.settings.Brightness, Codes.MessageType.BRIGHTNESS);
        }

        private void button_mode_Click(object sender, EventArgs e)
        {
            this.settings.mode = (Codes.Mode)this.comboBox_mode.SelectedIndex;

            this.messageSender.SendValue((byte)this.settings.mode, Codes.MessageType.MODE);

            this.onChangeModes(this.settings.mode);
        }

        private void button_color_Click(object sender, EventArgs e)
        {
            var r = int.Parse(this.textBox_color_r.Text);
            var g = int.Parse(this.textBox_color_g.Text);
            var b = int.Parse(this.textBox_color_b.Text);
            this.settings.color = Color.FromArgb(r, g, b);
            this.messageSender.SendColor(this.settings.color);
        }

        private void button_palette_Click(object sender, EventArgs e)
        {
            this.settings.palette = (Codes.Palette)this.comboBox_palette.SelectedIndex;

            this.messageSender.SendValue((byte)this.settings.palette, Codes.MessageType.PALETTE);
        }

        private void button_updates_Click(object sender, EventArgs e)
        {
            this.settings.UpdatesPerSecond = int.Parse(this.textBox_updates.Text);

            this.messageSender.SendValue((byte)this.settings.UpdatesPerSecond, Codes.MessageType.UPDATES_PER_SECOND);
        }

        private void button_paletteChangeDivider_Click(object sender, EventArgs e)
        {
            this.settings.PaletteChangeDivider = int.Parse(this.textBox_paletteChangeDivider.Text);

            this.messageSender.SendValue((short)this.settings.PaletteChangeDivider, Codes.MessageType.PALETTE_CHANGE_DIVIDER);
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            this.noFocus.Focus();
        }

        private void button_ScreenColorStart_Click(object sender, EventArgs e)
        {
            this.ledController = new SmoothRandomLEDController(
                this.messageSender,
                this.settings.UpdatesPerSecond,
                SmoothRandomLEDController.DEFAULT_SECONDS_BETWEEN_CHANGE,
                SmoothRandomLEDController.DEFAULT_TRANSITION_TIME);
            //this.ledController = new SmoothRandomLEDController(this.settings, this.messageSender);
            //this.ledController = new HueCycleLEDController(this.settings, this.messageSender);
            //this.ledController = new ScreenLEDController(this.settings, this.messageSender);
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
