using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> InfoList = new Dictionary<string, string>();
            foreach (ComboBox cb in this.Controls.OfType<ComboBox>())
            {
                if (cb.SelectedItem != null)
                {
                    InfoList.Add(cb.Name, cb.SelectedItem.ToString());
                }
            }

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                if (!String.IsNullOrEmpty(tb.Text))
                {
                    InfoList.Add(tb.Name, tb.Text);
                }
            }

            Form2 form = new Form2(InfoList);
            form.ShowDialog();
            ButtonOff();
        }

        private void ButtonOff()
        {
            StringBuilder DeviceName = new StringBuilder(Delcom.MAXDEVICENAMELEN);
            if (Delcom.DelcomGetNthDevice(0, 0, DeviceName) == 0)
            {
                return;
            }
            uint deviceHandle = Delcom.DelcomOpenDevice(DeviceName, 0);
            if (deviceHandle == 0)
            {
                return;
            }
            const int ledColor = Delcom.GREENLED;
            Delcom.DelcomLEDControl(deviceHandle, ledColor, Delcom.LEDON);
            Delcom.DelcomLEDPower(deviceHandle, ledColor, 0);
            Delcom.DelcomCloseDevice(deviceHandle);
        }

        private void Validate(object sender, EventArgs e)
        {
            foreach (ComboBox cb in this.Controls.OfType<ComboBox>())
            {
                if (cb.SelectedItem == null)
                {
                    btnNext.Enabled = false;
                    return;
                }
            }

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                if (String.IsNullOrEmpty(tb.Text))
                {
                    btnNext.Enabled = false;
                    return;
                }
            }
            btnNext.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ButtonOff();
        }
    }
}
