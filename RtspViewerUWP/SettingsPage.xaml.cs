using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RtspViewerUWP
{
    public sealed partial class SettingsPage : Page
    {
        private readonly ObservableCollection<string> _rtspUrls = new ObservableCollection<string>();
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public SettingsPage()
        {
            InitializeComponent();

            if (_localSettings.Values.ContainsKey("rtspUrls"))
            {
                string[] rtspUrlsArray = _localSettings.Values["rtspUrls"] as string[];
                _rtspUrls = new ObservableCollection<string>(rtspUrlsArray);
            }
        }

        private void AddUrl()
        {
            _rtspUrls.Add(textBox.Text);
            textBox.Text = "";
            _localSettings.Values["rtspUrls"] = _rtspUrls.ToArray();
        }

        private void Button_Click(object _0, RoutedEventArgs _1)
        {
            AddUrl();
        }

        private void MenuFlyoutItem_Click(object _0, RoutedEventArgs _1)
        {
            _ = _rtspUrls.Remove((string)listBox.SelectedValue);
            if (_rtspUrls.Count == 0)
            {
                _ = _localSettings.Values.Remove("rtspUrls");
            }
            else
            {
                _localSettings.Values["rtspUrls"] = _rtspUrls.ToArray();
            }
        }

        private void textBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                AddUrl();
            }
        }
    }
}
