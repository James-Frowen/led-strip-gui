using System.Windows.Forms;

namespace led_strip_gui
{
    public partial class Form1 : Form
    {
        public StripInfo current;
        public StripInfo formValues;

        public Form1()
        {
            this.InitializeComponent();

            USB.Open();
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            USB.Close();
        }


        private void textBox_brightness_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            //// only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}

            int b;
            if (int.TryParse(this.textBox_brightness.Text, out b))
            {
                if (b < 0) { b = 0; }
                if (b > 255) { b = 255; }
                this.formValues.brightness = (byte)b;
            }
            else
            {
                MessageBox.Show("Could not Parse brightness value\n" + this.textBox_brightness.Text);
                this.textBox_brightness.Text = "0";
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            USB.Send("hello world");
        }
    }
}
