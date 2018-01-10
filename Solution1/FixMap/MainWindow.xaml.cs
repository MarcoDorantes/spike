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

namespace FixMap
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      //string edition = 
      string version = $"{this.GetType().Assembly.GetName().Version}";
      Title = $"[PID: {System.Diagnostics.Process.GetCurrentProcess().Id}] {Title}{version}";
      DataContext = new MainWindowViewModel();
    }
  }
}