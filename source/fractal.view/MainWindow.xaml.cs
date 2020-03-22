using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fractal.view
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void addLine(Grid grid, double x1, double y1, double x2, double y2)
    {
      var line = new Line();
      line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
      line.X1 = x1;
      line.Y1 = y1;
      line.X2 = x2;
      line.Y2 = y2;
      line.HorizontalAlignment = HorizontalAlignment.Left;
      line.VerticalAlignment = VerticalAlignment.Center;
      line.StrokeThickness = 2;
      grid.Children.Add(line);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      Title = $"{Width} {Height}";

      var plane = new fractal.lib.Plane(Width, Height);
      IEnumerable<fractal.lib.Segment> line = fractal.lib.Fractal.make_line(0F, 0F, 100F, 0F, 1);
      foreach (var segment in line)
      {
        var a = plane.ToAPI(segment.A);
        var b = plane.ToAPI(segment.B);
        addLine(grid1, a.X, a.Y, b.X, b.Y);
      }
    }
  }
}