using LibVLCSharp.Platforms.UWP;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RtspViewerUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StreamsPage : Page
    {
        public StreamsPage()
        {
            ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            InitializeComponent();
            //var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            //coreTitleBar.ExtendViewIntoTitleBar = true;
            //Window.Current.SetTitleBar(NavigationViewControl);

            Core.Initialize();

            List<string> urls = new List<string>();

            if (_localSettings.Values.ContainsKey("rtspUrls"))
            {
                string[] rtspUrlsArray = _localSettings.Values["rtspUrls"] as string[];
                urls = new List<string>(rtspUrlsArray);
            }

            //urls = new List<string>
            //{
            //    "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"
            //};

            for (int i = 0; i < urls.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                string _url = urls[i];
                VideoView _videoView = new VideoView();
                Grid.SetRow(_videoView, i);

                grid.Children.Add(_videoView);

                System.Diagnostics.Debug.WriteLine(_url + " videoview added");

                _videoView.Loaded += (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine(_url + " videoview loaded");
                    LibVLC _libVLC = new LibVLC(_videoView.SwapChainOptions);
                    MediaPlayer _mediaPlayer = new MediaPlayer(_libVLC);
                    _videoView.MediaPlayer = _mediaPlayer;
                    //_mediaPlayer.Play(new Media(_libVLC, new Uri(_url), new string[] { ":no-audio" }));
                    _mediaPlayer.Play(new Media(_libVLC, _url, FromType.FromLocation));
                    _mediaPlayer.EndReached += (_sender, _e) =>
                    {
                        System.Diagnostics.Debug.WriteLine(_url + " EndReached: Waiting to retry");
                        Thread.Sleep(30000);
                        System.Diagnostics.Debug.WriteLine("Retrying to connect.");
                        ThreadPool.QueueUserWorkItem(_ => _mediaPlayer.Play(new Media(_libVLC, _url, FromType.FromLocation)));
                    };
                    _mediaPlayer.EncounteredError += (_sender, _e) =>
                    {
                        System.Diagnostics.Debug.WriteLine(_url + " EncounteredError: Waiting to retry");
                        Thread.Sleep(30000);
                        System.Diagnostics.Debug.WriteLine("Retrying to connect.");
                        ThreadPool.QueueUserWorkItem(_ => _mediaPlayer.Play(new Media(_libVLC, _url, FromType.FromLocation)));
                    };
                };
            }
        }
    }
}
