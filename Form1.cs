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
    }
}
