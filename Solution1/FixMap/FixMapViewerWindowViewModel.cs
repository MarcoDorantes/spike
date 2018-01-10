using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace FixMap
{
  class FixMapViewerWindowViewModel
  {
    private ICommand onSubscribe;
    private ICommand onUnsubscribe;
    private ICommand onExport;
    private ICommand onClear;
    private readonly Encoding encoding;
    private SolaceViewerConfigurationSection config;
    private FixMapViewer owner;

    public FixMapViewerWindowViewModel(FixMapViewer owner)
    {
      count = 0;
      this.owner = owner;
      encoding = Encoding.GetEncoding("ISO-8859-1");
      ClearUI();
return;
      config = GetConfiguration();
      ConfiguredTopics = new ObservableCollection<ConfiguredTopic>();
      ConfiguredHosts = new ObservableCollection<ConfiguredHost>();

      foreach (TopicCollectionElement topic in config.Topics)
      {
        ConfiguredTopics.Add(new ConfiguredTopic(topic));
      }
      foreach (HostCollectionElement host in config.Hosts)
      {
        ConfiguredHosts.Add(new ConfiguredHost(host));
      }
      IncludeDestination = true;
      isIncludeDestinationEnabled = true;

      isConnected = false;
    }

    private int _received;
    private int received { get { return _received; } set { _received = value; NotifyChange("Received"); this.owner.Name = Received; } }
    public string Received { get { return _received.ToString("N0"); } }

    public ICommand OnSubscribe
    {
      get { return onSubscribe ?? (onSubscribe = new CommandHandler(() => Subscribe(), (parameter) => { return true; })); }
    }
    public ICommand OnUnsubscribe
    {
      get { return onUnsubscribe ?? (onUnsubscribe = new CommandHandler(() => Unsubscribe(), (parameter) => { return true; })); }
    }
    public ICommand OnExport
    {
      get { return onExport ?? (onExport = new CommandHandler(() => Export(), (parameter) => { return true; })); }
    }
    public ICommand OnClear
    {
      get { return onClear ?? (onClear = new CommandHandler(() => ClearUI(), (parameter) => { return true; })); }
    }
    public ObservableCollection<string> Messages { get; private set; }
    public ObservableCollection<string> Notices { get; private set; }
    public bool IncludeDestination { get; set; }
    private bool isIncludeDestinationEnabled;
    public bool IsIncludeDestinationEnabled { get { return isIncludeDestinationEnabled; } set { isIncludeDestinationEnabled = value; NotifyChange(nameof(IsIncludeDestinationEnabled)); } }

    private bool isConnected;
    public bool IsConnected { get { return isConnected; } set { isConnected = value; NotifyChange(nameof(IsConnected)); NotifyChange(nameof(IsDisconnected)); } } //IsDisconnected = !value; 
    public bool IsDisconnected { get { return !isConnected; } } //set { isDisconnected = value; NotifyChange(nameof(IsDisconnected)); }

    #region Topic selection
    private ConfiguredTopic selectedTopic;
    public ConfiguredTopic SelectedTopic { get { return selectedTopic; } set { selectedTopic = value; NotifyChange("SelectedTopic"); } }
    public class ConfiguredTopic
    {
      private TopicCollectionElement topic;
      public ConfiguredTopic(TopicCollectionElement topic)
      {
        this.topic = topic;
      }
      public string Topicpath { get { return topic.Path; } }
      public string Description { get { return topic.Description; } }
      public string DisplayName { get { return string.Format("{1} ↔ {0}", topic.Path, topic.Description); } }
    }
    public ObservableCollection<ConfiguredTopic> ConfiguredTopics { get; set; }
    #endregion
    #region Host selection
    private ConfiguredHost selectedHost;
    public ConfiguredHost SelectedHost { get { return selectedHost; } set { selectedHost = value; NotifyChange(nameof(SelectedHost)); } }
    public class ConfiguredHost
    {
      private HostCollectionElement host;
      public ConfiguredHost(HostCollectionElement host)
      {
        this.host = host;
      }
      public string Name { get { return host.Name; } }
      public string IP { get { return host.SessionHost; } }
      public string User { get { return host.SessionUsername; } }
      public string Password { get { return host.SessionPassword; } }
      public string VPN { get { return host.SessionVpnName; } }
      public string ClientPrefix { get { return host.ClientNamePrefix; } }
      public string ClientDescription { get { return host.ClientDescription; } }
    }
    public ObservableCollection<ConfiguredHost> ConfiguredHosts { get; set; }
    #endregion

    //private SolaceAccessLibrary.SolaceSessionTopicDispatcher dispatcher;
    private CancellationTokenSource cancel;
    private int count;

    private void Subscribe()
    {
    }

    internal void Unsubscribe() { }

    /*private void DisposeCurrentSubscription()
    {
      if (cancel != null)
      {
        cancel.Cancel();
        cancel.Dispose();
        cancel = null;
      }
      if (dispatcher != null)
      {
        if (IncludeDestination || ShowMetadata)
        {
          dispatcher.OnMessage -= SelectedSource_OnMessage;
        }
        else
        {
          dispatcher.OnMessageAttachment -= SelectedSource_OnMessageAttachment;
        }
        dispatcher.Stop();
        dispatcher.Dispose();
        dispatcher = null;
      }
    }*/
    private void Export()
    {
      try
      {
        if (SelectedTopic == null || string.IsNullOrWhiteSpace(SelectedTopic.Topicpath))
        {
          MessageBox.Show("Topic path cannot be empty.");
          return;
        }
        Func<string, string> cleanFilename = (fname) =>
        {
          foreach (char c in System.IO.Path.GetInvalidFileNameChars())
          {
            fname = fname.Replace(new string(c, 1), "");
          }
          return fname;
        };
        string topicpath = SelectedTopic.Topicpath;
        bool isFirstMessageJSONmap = isJSONDictionary();
        string choice = "csv";
        if (isFirstMessageJSONmap)
        {
          switch (MessageBox.Show("Do you want Excel format?", "Exported file format", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
          {
            case MessageBoxResult.Yes:
              choice = "csv";
              break;
            case MessageBoxResult.No:
              choice = "txt";
              break;
            case MessageBoxResult.Cancel:
              return;
          }
        }
        var ext = isFirstMessageJSONmap ? choice : "txt";
        int tail = topicpath.EndsWith("/>") ? 2 : 0;
        //Notify(string.Format("Topic_{0}", topicpath.Substring(0, topicpath.Length - tail).Replace('/', '_')));
        var basefilename = new System.IO.FileInfo(cleanFilename((string.Format("Topic_{0}", topicpath.Substring(0, topicpath.Length - tail).Replace('/', '_')))));
        var selectedFile = new Microsoft.Win32.SaveFileDialog();
        selectedFile.FileName = cleanFilename(string.Format("{0}_{1}.{2}", basefilename, DateTime.Now.ToString("MMMd_HHmm"), ext));
        selectedFile.DefaultExt = "." + ext;
        selectedFile.Filter = ext == "csv" ? "Microsoft Office Excel CSV documents |*.csv" : "Text file |*.txt";
        bool? answer = selectedFile.ShowDialog();
        if (!answer.HasValue || answer.Value == false)
        {
          return;
        }
        string filename = string.Format("{0}\\{1}.{2}", System.IO.Path.GetDirectoryName(selectedFile.FileName), cleanFilename(System.IO.Path.GetFileNameWithoutExtension(selectedFile.SafeFileName)), ext);
        using (var writer = new System.IO.StreamWriter(filename))
        {
          bool heads = true;
          Messages.ToArray().Aggregate(writer, (whole, next) => { whole.WriteLine(ext == "csv" ? writeDictionary(next, ref heads) : next); return whole; });
        }
        OpenExcelFile(filename, ext);
      }
      catch (Exception ex)
      {
        Notify(ex);
        MessageBox.Show(ex.Message, ex.GetType().FullName, MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
    private void OpenExcelFile(string filename, string ext)
    {
      string logline = string.Format("File saved to:\n{0}", filename);
      Notify(logline);
      if (MessageBoxResult.Yes == MessageBox.Show(string.Format("{0}\n\nDo you want to open it now?", logline), "Export to file", MessageBoxButton.YesNo, MessageBoxImage.Question))
      {
        try
        {
          var excel = new System.Diagnostics.ProcessStartInfo(ext == "csv" ? "EXCEL.EXE" : "NOTEPAD.EXE", string.Format("\"{0}\"", filename));
          System.Diagnostics.Process.Start(excel);
        }
        catch (Exception ex)
        {
          string log = string.Format("There was a problem and the file cannot be opened:\n{0}", filename);
          Notify(ex, log);
          MessageBox.Show(string.Format("{0}\n\n. Please check your local file associations", log), "Export to file", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
    }
    private string writeDictionary(string msg, ref bool heads)
    {
      try
      {
        var properties = deserialize(msg);
        var result = new StringBuilder();
        if (heads)
        {
          foreach (string k in properties.Keys)
          {
            result.AppendFormat("{0},", k);
          }
          result.AppendLine();
          heads = false;
        }
        foreach (string k in properties.Keys)
        {
          result.AppendFormat("{0},", properties[k]);
        }
        return result.ToString();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(string.Format("{0}: {1}\n{2}", ex.GetType().FullName, ex.Message, ex.StackTrace));
        return msg;
      }
    }
    private bool isJSONDictionary()
    {
      try
      {
        if (Messages.Count <= 0) return false;
        string msg = Messages[0];

        var dic = deserialize(msg);
        return dic != null && dic.GetType() == typeof(Dictionary<string, object>);
      }
      catch (Exception)
      {
        return false;
      }
    }
    private IDictionary<string, object> deserialize(string message)
    {
      return null;// Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
    }
    private void ClearUI()
    {
      count = 0;
      this.received = count;
      if (Messages == null)
      {
        Messages = new ObservableCollection<string>();
      }
      else Messages.Clear();
      if (Notices == null)
      {
        Notices = new ObservableCollection<string>();
      }
      else Notices.Clear();
    }

    #region InterplayContract.IMonitor
    public void Notify(string notice)
    {
      var log = string.Format("[{0}] {1}", DateTime.Now.ToString("s"), notice);
      Trace.WriteLine(log);
      Application.Current.Dispatcher.Invoke(() => this.Notices.Add(log));
    }

    public void Notify(Exception exception)
    {
      var time = DateTime.Now.ToString("s");
      var log = string.Format("[{0}] Exception: {1}", time, exception);
      Trace.WriteLine(log);
      Application.Current.Dispatcher.Invoke(delegate { this.Notices.Add(string.Format("[{0}] {1}: {2}", time, exception.GetType().FullName, exception.Message)); });

      if (exception.Data == null)
      {
        Environment.FailFast("Host process SolaceViewer is corrupt: exception.Data is null.", exception);
      }
    }

    public void Notify(Exception exception, string data)
    {
      var when = DateTime.Now.ToString("s");
      Trace.WriteLine(string.Format("[{0}] Exception ({2}): {1}", when, exception, data));
      var log = string.Format("[{0}] {1}: {2} {3}", when, exception.GetType().FullName, exception.Message, data);
      Application.Current.Dispatcher.Invoke(delegate { this.Notices.Add(log); });

      if (exception.Data == null)
      {
        Environment.FailFast("Host process SolaceViewer is corrupt: exception.Data is null.", exception);
      }
    }

    /*public void Notify(InterplayContract.MonitorEvent eventid, string message, DateTime when, object correlationKey, string eventType, string info, int responseCode, Guid? sessionKey)
    {
      var log = $"ViewMonitor.Notify: {sessionKey} ({eventid}) - {message}";
      Trace.WriteLine(log);
      Application.Current.Dispatcher.Invoke(delegate { this.Notices.Add(log); });
    }*/

    public void Start() { }
    public void Stop() { }
    public void Dispose()
    {
      //DisposeCurrentSubscription();
    }
    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void NotifyChange(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
    public static SolaceViewerConfigurationSection GetConfiguration()
    {
      SolaceViewerConfigurationSection result = System.Configuration.ConfigurationManager.GetSection("SolaceViewerConfigurationSection") as SolaceViewerConfigurationSection;
      if (result == null)
      {
        throw new System.Configuration.ConfigurationErrorsException("SolaceViewerConfigurationSection is not properly configured.");
      }
      return result;
    }
  }

  class CommandHandler : ICommand
  {
    private Action action;
    private Func<object, bool> isExecutionValid;
    public CommandHandler(Action action, Func<object, bool> valid)
    {
      this.action = action;
      this.isExecutionValid = valid;
    }
    public bool CanExecute(object parameter)
    {
      return isExecutionValid(parameter);
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
      action();
    }
  }

  #region ConfigSection
  public class SolaceViewerConfigurationSection : System.Configuration.ConfigurationSection
  {
    [System.Configuration.ConfigurationProperty("Hosts", IsRequired = true)]
    [System.Configuration.ConfigurationCollection(typeof(HostCollection), AddItemName = "Host")]
    public HostCollection Hosts { get { return (HostCollection)this["Hosts"]; } }

    [System.Configuration.ConfigurationProperty("Topics", IsRequired = true)]
    [System.Configuration.ConfigurationCollection(typeof(HostCollection), AddItemName = "Topic")]
    public TopicCollection Topics { get { return (TopicCollection)this["Topics"]; } }
  }
  public class HostCollection : System.Configuration.ConfigurationElementCollection
  {
    protected override System.Configuration.ConfigurationElement CreateNewElement()
    {
      return new HostCollectionElement();
    }
    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
    {
      return ((HostCollectionElement)element).Name;
    }

    public HostCollectionElement GetElement(string name)
    {
      System.Configuration.ConfigurationElement element = BaseGet(name);
      HostCollectionElement result = element as HostCollectionElement;
      return result;
    }
  }
  public class TopicCollection : System.Configuration.ConfigurationElementCollection
  {
    protected override System.Configuration.ConfigurationElement CreateNewElement()
    {
      return new TopicCollectionElement();
    }
    protected override object GetElementKey(System.Configuration.ConfigurationElement element)
    {
      return ((TopicCollectionElement)element).Path;
    }

    public TopicCollectionElement GetElement(string name)
    {
      System.Configuration.ConfigurationElement element = BaseGet(name);
      TopicCollectionElement result = element as TopicCollectionElement;
      return result;
    }
  }
  public class HostCollectionElement : System.Configuration.ConfigurationElement
  {
    [System.Configuration.ConfigurationProperty("Name", IsRequired = true, Options = System.Configuration.ConfigurationPropertyOptions.IsRequired)]
    public string Name { get { return (string)this["Name"]; } }

    [System.Configuration.ConfigurationProperty("SessionHost", IsRequired = true)]
    public string SessionHost { get { return (string)this["SessionHost"]; } }

    [System.Configuration.ConfigurationProperty("SessionUsername", IsRequired = true)]
    public string SessionUsername { get { return (string)this["SessionUsername"]; } }

    [System.Configuration.ConfigurationProperty("SessionPassword", IsRequired = true)]
    public string SessionPassword { get { return (string)this["SessionPassword"]; } }
    [System.Configuration.ConfigurationProperty("SessionVpnName", IsRequired = true)]
    public string SessionVpnName { get { return (string)this["SessionVpnName"]; } }
    [System.Configuration.ConfigurationProperty("ClientNamePrefix")]
    public string ClientNamePrefix { get { return (string)this["ClientNamePrefix"]; } }

    [System.Configuration.ConfigurationProperty("ClientDescription")]
    public string ClientDescription { get { return (string)this["ClientDescription"]; } }
  }
  public class TopicCollectionElement : System.Configuration.ConfigurationElement
  {
    [System.Configuration.ConfigurationProperty("Path", IsRequired = true, Options = System.Configuration.ConfigurationPropertyOptions.IsRequired)]
    public string Path { get { return (string)this["Path"]; } }

    [System.Configuration.ConfigurationProperty("Description", IsRequired = true, Options = System.Configuration.ConfigurationPropertyOptions.IsRequired)]
    public string Description { get { return (string)this["Description"]; } }
  }
  #endregion
}