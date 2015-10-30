using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
  class UserControl1ViewModel : INotifyPropertyChanged
  {
    public UserControl1ViewModel(string id, int a,int z)
    {
      ID = $"UserControl Data ID: {id}";
      System.Diagnostics.Trace.WriteLine($"[{DateTime.Now.ToString("s")}] UserControl1ViewModel {ID}");
      Names = new ObservableCollection<string>();
      Enumerable.Range(a, z).ToList().ForEach(n => Names.Add($"Item{n}"));
    }
    public string ID { get; private set; }
    public ObservableCollection<string> Names { get; private set; }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    private void Notify(string property)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
    #endregion
  }
}