using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RtspViewerUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private ObservableCollection<string> rtspUrls;
        private ApplicationDataContainer _localSettings;

        public SettingsPage()
        {
            this.InitializeComponent();

            System.Diagnostics.Debug.WriteLine("LocalSettings: initializing container");
            _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            rtspUrls = new ObservableCollection<string>();

            System.Diagnostics.Debug.WriteLine("LocalSettings: checking if rtspUrls key exists");
            if (_localSettings.Values.ContainsKey("rtspUrls"))
            {
                System.Diagnostics.Debug.WriteLine("LocalSettings: reading rtspUrls setting");
                String[] rtspUrlsArray = _localSettings.Values["rtspUrls"] as string[];
                rtspUrls = new ObservableCollection<string>(rtspUrlsArray);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            rtspUrls.Add(textBox.Text);
            System.Diagnostics.Debug.WriteLine(String.Join(",", rtspUrls));
            textBox.Text = "";
            System.Diagnostics.Debug.WriteLine("LocalSettings: writing rtspUrls setting");
            _localSettings.Values["rtspUrls"] = rtspUrls.ToArray();
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine((string)listBox.SelectedValue);
            rtspUrls.Remove((string)listBox.SelectedValue);
            if (rtspUrls.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("LocalSettings: removing rtspUrls setting");
                _localSettings.Values.Remove("rtspUrls");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("LocalSettings: writing rtspUrls setting");
                _localSettings.Values["rtspUrls"] = rtspUrls.ToArray();
            }
        }
    }
}
