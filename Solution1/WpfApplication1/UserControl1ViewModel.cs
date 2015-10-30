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
/* some refs and fragments
http://stackoverflow.com/questions/601826/multiple-usercontrol-instances-in-tabcontrol
http://stackoverflow.com/questions/1965355/how-to-add-new-user-control-in-tabcontrol-contenttemplate
            <TabControl.Resources>
                <DataTemplate x:Key="contentTemplate" x:Shared="False">
                    <local:ViewerWindow/>
                </DataTemplate>
                <Style TargetType="{x:Type TabItem}">
                    <Setter Property="Header" Value="{Binding Header}"/>
                    <Setter Property="ContentTemplate" Value="{StaticResource contentTemplate}"/>
                </Style>
            </TabControl.Resources>

        <TabControl IsSynchronizedWithCurrentItem="True" ItemsSource="{Binding Viewers}" SelectedIndex="0">
            <TabControl.Resources>
                <DataTemplate x:Key="contentTemplate" x:Shared="False">
                    <StackPanel Orientation="Vertical">
                        <Button Content="{Binding Action}" Width="100" Height="30" Command="{Binding OnRun}"/>
                        <local:UserControl1 Tag="{Binding Data}" />
                    </StackPanel>
                </DataTemplate>
                <Style TargetType="{x:Type TabItem}">
                    <Setter Property="Header" Value="{Binding Name}"/>
                    <Setter Property="ContentTemplate" Value="{StaticResource contentTemplate}"/>
                </Style>
            </TabControl.Resources>
        </TabControl>
*/
