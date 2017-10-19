using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace timeline
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
        }
        public SplashForm(GetSet g)
        {
            InitializeComponent();
            LDgs = g;
        }
       
        int x = 0;
        private GetSet LDgs; // delegate for load settings method in parent
        
        private void SplashForm_Load(object sender, EventArgs e)
        {
            label3.Visible = true;
            progressBar1.Visible = true;
            progressBar1.Value = 50;
            LDgs();
            progressBar1.Value = 100;
            timer1.Interval = 1000;
            timer1.Start();
            hcwgenericclasses.DLLInfo di = new hcwgenericclasses.DLLInfo();
            label4.Text = "DLL Version = "+di.GetVersion();
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            x++;
            if (x == 2)
            {
                this.Close();
            }
        }

        
    }
}
