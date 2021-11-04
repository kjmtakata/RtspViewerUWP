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
            InitializeComponent();

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            List<string> urls = new List<string>();

            if (localSettings.Values.ContainsKey("rtspUrls"))
            {
                string[] rtspUrlsArray = localSettings.Values["rtspUrls"] as string[];
                urls = new List<string>(rtspUrlsArray);
            }

            for (int i = 0; i < urls.Count; i++)
            {
                string url = urls[i];

                VideoView videoView = new VideoView();
                grid.Children.Add(videoView);

                grid.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(videoView, i);

                videoView.Loaded += (_0, _1) =>
                {
                    LibVLC libVLC = new LibVLC(videoView.SwapChainOptions);
                    MediaPlayer mediaPlayer = new MediaPlayer(libVLC);
                    videoView.MediaPlayer = mediaPlayer;
                    _ = mediaPlayer.Play(new Media(libVLC, url, FromType.FromLocation));
                    mediaPlayer.EndReached += (_2, _3) => DelayRestartMediaPlayer(libVLC, mediaPlayer);
                    mediaPlayer.EncounteredError += (_2, _3) => DelayRestartMediaPlayer(libVLC, mediaPlayer);
                };
            }
        }

        private void DelayRestartMediaPlayer(LibVLC libVLC, MediaPlayer mediaPlayer)
        {
            System.Diagnostics.Debug.WriteLine(mediaPlayer.Media.Mrl + " failed: Waiting to retry");
            Thread.Sleep(30000);
            System.Diagnostics.Debug.WriteLine("Retrying to connect.");
            _ = ThreadPool.QueueUserWorkItem(_ => mediaPlayer.Play(new Media(libVLC, mediaPlayer.Media.Mrl, FromType.FromLocation)));
        }
    }
}
