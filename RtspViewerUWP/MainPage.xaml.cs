using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RtspViewerUWP
{
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void NavigationViewControl_SelectionChanged(NavigationView _0, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                _ = contentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                NavigationViewItem selectedItem = (NavigationViewItem)args.SelectedItem;
                if (selectedItem != null)
                {
                    switch (selectedItem.Tag)
                    {
                        case "streams":
                            _ = contentFrame.Navigate(typeof(StreamsPage));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void NavigationViewControl_Loaded(object _0, RoutedEventArgs _1)
        {
            // set the initial SelectedItem 
            foreach (NavigationViewItemBase item in NavigationViewControl.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "streams")
                {
                    NavigationViewControl.SelectedItem = item;
                    break;
                }
            }
        }
    }
}
