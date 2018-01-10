using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace FixMap
{
  class MainWindowViewModel
  {
    private ICommand onNewViewer;
    private ICommand onExit;
    private uint viewer_count;

    public MainWindowViewModel()
    {
      viewer_count = 0;
      Viewers = new ObservableCollection<FixMapViewer>();
      AddViewer();
    }

    public ObservableCollection<FixMapViewer> Viewers { get; private set; }

    public ICommand OnExit
    {
      get { return onExit ?? (onExit = new CommandHandler(() => ExitApp(), (parameter) => { return true; })); }
    }
    public ICommand OnNewViewer
    {
      get { return onNewViewer ?? (onNewViewer = new CommandHandler(() => NewViewer(), (parameter) => { return true; })); }
    }

    private void NewViewer() { AddViewer(); }

    private void ExitApp()
    {
      UnsubscribeAllViewersAndTerminateProcess();
    }

    private void AddViewer()
    {
      ++viewer_count;
      Viewers.Add(new FixMapViewer($"Viewer{viewer_count}"));
    }

    private void UnsubscribeAllViewersAndTerminateProcess()
    {
      MessageBoxResult answer = MessageBox.Show("Are you sure about closing the whole application?", "Terminate current process", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
      if (answer != MessageBoxResult.Yes)
      {
        return;
      }
      foreach (var viewer in Viewers)
      {
        viewer.Data.Unsubscribe();
      }
      MessageBox.Show("The process is about to close. Please click OK.", "Terminate current process", MessageBoxButton.OK, MessageBoxImage.Information);
      Application.Current.MainWindow.Close();
    }
  }

  class FixMapViewer : INotifyPropertyChanged
  {
    private readonly string name;
    public FixMapViewer(string name)
    {
      Data = new FixMapViewerWindowViewModel(this);
      this.name = name;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string Name
    {
      get
      {
        string received = "";
        if (Data != null)
        {
          received = $" ({Data.Received})";
        }
        return $"{name}{received}";
      }
      set
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
      }
    }
    public FixMapViewerWindowViewModel Data { get; set; }
  }
}