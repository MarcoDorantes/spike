using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fractal.form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var g = new System.Windows.Forms.PictureBox();
            g.Dock = DockStyle.Fill;
            g.Paint += OnPaint;
            Controls.Add(g);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Blue, 10, 20, 110, 70);
        }
    }
}