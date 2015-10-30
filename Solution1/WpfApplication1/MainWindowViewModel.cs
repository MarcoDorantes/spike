using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApplication1
{
  class MainWindowViewModel : INotifyPropertyChanged
  {
    private uint tabcount;
    public MainWindowViewModel()
    {
      tabcount = 0;
      Viewers = new ObservableCollection<Viewer>();
      AddViewer();
    }

    public ObservableCollection<Viewer> Viewers { get; private set; }
    private void AddViewer()
    {
      ++tabcount;
      Viewers.Add(new Viewer { Name = $"Tab{tabcount}", Action = $"Action{tabcount}", Data = new UserControl1ViewModel($"{tabcount}") });
    }


    private ICommand onQuit;
    public ICommand OnQuit { get { return onQuit ?? (onQuit = new Command(() => Quit())); } }
    private void Quit() { System.Windows.Application.Current.MainWindow.Close(); }

    private ICommand onNew;
    public ICommand OnNew { get { return onNew ?? (onNew = new Command(() => New())); } }
    private void New() { AddViewer(); }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    private void Notify(string property)
    {
      PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(property));
    }
    #endregion
  }

  class Viewer
  {
    public string Name { get; set; }
    public string Action { get; set; }
    public UserControl1ViewModel Data { get; set; }

    private ICommand onRun;
    public ICommand OnRun { get { return onRun ?? (onRun = new Command(() => Run())); } }
    private void Run() { System.Windows.MessageBox.Show($"Run: {Name} - {Action} ({(Data as UserControl1ViewModel).ID})"); }
  }

  class Command : ICommand
  {
    private Action action;
    public Command(Action call)
    {
      action = call;
    }
    public event EventHandler CanExecuteChanged;
    public bool CanExecute(object parameter){return true;}
    public void Execute(object parameter){ action(); }
  }
}