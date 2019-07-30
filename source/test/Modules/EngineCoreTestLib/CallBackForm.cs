using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineCoreTestLib
{
    public partial class CallBackForm : Form
    {
        public CallBackForm()
        {
            InitializeComponent();
        }

        public CallBackForm(int x, int y)
        {
            InitializeComponent();
            label1.Text = x.ToString();
            label2.Text = y.ToString();
        }
    }
}
