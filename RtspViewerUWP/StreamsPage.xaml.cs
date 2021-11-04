using LibVLCSharp.Platforms.UWP;
using LibVLCSharp.Shared;
using System.Collections.Generic;
using System.Threading;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace RtspViewerUWP
{
    public sealed partial class StreamsPage : Page
    {
        public StreamsPage()
        {
            ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

            InitializeComponent();

            Core.Initialize();

            List<string> urls = new List<string>();

            if (_localSettings.Values.ContainsKey("rtspUrls"))
            {
                string[] rtspUrlsArray = _localSettings.Values["rtspUrls"] as string[];
                urls = new List<string>(rtspUrlsArray);
            }

            for (int i = 0; i < urls.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                string url = urls[i];
                VideoView videoView = new VideoView();
                Grid.SetRow(videoView, i);
                grid.Children.Add(videoView);

                videoView.Loaded += (_0, _1) =>
                {
                    LibVLC libVLC = new LibVLC(videoView.SwapChainOptions);
                    MediaPlayer mediaPlayer = new MediaPlayer(libVLC);
                    videoView.MediaPlayer = mediaPlayer;
                    _ = mediaPlayer.Play(new Media(libVLC, url, FromType.FromLocation));
                    mediaPlayer.EndReached += (_2, _3) =>
                    {
                        System.Diagnostics.Debug.WriteLine(url + " EndReached: Waiting to retry");
                        Thread.Sleep(30000);
                        System.Diagnostics.Debug.WriteLine("Retrying to connect.");
                        _ = ThreadPool.QueueUserWorkItem(_ => mediaPlayer.Play(new Media(libVLC, url, FromType.FromLocation)));
                    };
                    mediaPlayer.EncounteredError += (_2, _3) =>
                    {
                        System.Diagnostics.Debug.WriteLine(url + " EncounteredError: Waiting to retry");
                        Thread.Sleep(30000);
                        System.Diagnostics.Debug.WriteLine("Retrying to connect.");
                        _ = ThreadPool.QueueUserWorkItem(_ => mediaPlayer.Play(new Media(libVLC, url, FromType.FromLocation)));
                    };
                };
            }
        }
    }
}
