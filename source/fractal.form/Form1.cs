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
    System.Windows.Forms.PictureBox box;
    public Form1()
    {
      InitializeComponent();

      box = new System.Windows.Forms.PictureBox();
      box.Width = Width;
      box.Height = Height;
      box.Dock = DockStyle.Fill;
      box.Paint += OnPaint;
      Controls.Add(box);
    }

    private void OnPaint(object sender, PaintEventArgs e)
    {
      var plane = new fractal.lib.Plane(box.Width, box.Height);
      IEnumerable<fractal.lib.Segment> line = fractal.lib.Fractal.make_line(0F, 0F, 100F, 0F, 1);
      foreach (var segment in line)
      {
        var a = plane.ToAPI<float>(segment.A);
        var b = plane.ToAPI<float>(segment.B);
        e.Graphics.DrawLine(Pens.Blue, a.X, a.Y, b.X, b.Y);
      }
    }
  }
}