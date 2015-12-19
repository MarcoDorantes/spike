/*
        private ICommand onTrackQueue;
        public ICommand OnTrackQueue
        {
            get { return onTrackQueue ?? (onTrackQueue = new CommandHandler(() => TrackQueueHashSpace(), (parameter) => { return true; })); }
        }
        private void TrackQueueHashSpace()
        {
            if (queue_tracking_window == null)
            {
                queue_tracking_window = new TrackHashSpaceWindow();
                //queue_tracking_window.Parent=
            }
            queue_tracking_window.ShowDialog();
        }

*/
using SqlWriterAgent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostConsoleView
{
    class TrackHashSpaceWindowViewModel
    {
        private HostCollectionElement host;
        public TrackHashSpaceWindowViewModel()
        {
            QueuesStats = new ObservableCollection<QueueStats>();

            #region read config
            ThreadDispatcherConfigurationSection ThreadDispatcherSettings = System.Configuration.ConfigurationManager.GetSection("ThreadDispatcherConfigurationSection") as ThreadDispatcherConfigurationSection;
            if (ThreadDispatcherSettings == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("ThreadDispatcherConfigurationSection is not properly configured.");
            }
            //ThreadDispatcherSettings.Monitor.EngineID,
            host = ThreadDispatcherSettings.SourceHosts.GetElement(ThreadDispatcherSettings.Monitor.Host);
            //MessageBox.Show($"{host.SessionHost} | {host.SessionUsername} | {host.SessionVpnName}");
            #endregion

            QueuesStats.Add(new QueueStats("Bmv.FixWriter00", 0.1M));
            QueuesStats.Add(new QueueStats("Bmv.FixWriter01", 1.3M));
            QueuesStats.Add(new QueueStats("Bmv.FixWriter02", 0M));
        }

        //public string Host => host.SessionHost;
        public ObservableCollection<QueueStats> QueuesStats { get; private set; }
        
        public string GetSourceHeader(string title) => $"{title} | Source: VPN {host.SessionVpnName} Host {host.SessionHost} User {host.SessionUsername}";
    }

    class QueueStats : INotifyPropertyChanged
    {
//Select to track | Queue name | High-Water Mark (HWM) | Message count
/*
            Tracking for Bmv.FixWriter05...
            Queue: Bmv.FixWriter05
            Message count:              1,947
                    HashID 34: 1025
                            Symbol ELEKTRA *: 1023
                            Symbol CETETRC ISHRS: 2
                    HashID 35: 447
                            Symbol AMX L: 367
                            Symbol AXTEL CPO: 80
                    HashID 33: 474
                            Symbol GCARSO A1: 474
                    HashID 32: 1
                            Symbol FSLR *: 1
*/

        public QueueStats(string name, decimal hwm)
        {
            Name = name;
            this.hwm = hwm;
            //IsTracked = hwm > 0M;
            count = 12345678;
        }

        //private bool isTracked;
        //public bool IsTracked { get { return isTracked; } set { isTracked = value; NotifyChange("IsTracked"); } }

        public string Name { get; private set; }


        private decimal _hwm;
        private decimal hwm { get { return _hwm; } set { _hwm= value; NotifyChange("HWM"); } }
        public string HWM { get { return hwm.ToString("F12"); } }


        private uint _count;
        private uint count { get { return _count; } set { _count = value; NotifyChange("MessageCount "); } }
        public string MessageCount { get { return count.ToString("N0"); } }



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}