using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Control;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace NotificationFlyoutSample
{

    public class NowPlayingPageViewModel : ObservableObject, INavigation
    {
        private string _artist;
        private bool _isPlaying;
        private bool _isPaused;
        private GlobalSystemMediaTransportControlsSession _session;
        private string _song;
        private BitmapImage _thumbnail;

        public bool IsPaused
        {
            get => _isPaused;
            set => SetProperty(ref _isPaused, value);
        }

        public string Artist
        {
            get => _artist;
            set => SetProperty(ref _artist, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value);
        }

        public string Song
        {
            get => _song;
            set => SetProperty(ref _song, value);
        }

        public async Task Next()
        {
            await _session.TrySkipNextAsync();
        }

        DispatcherQueue d;
        public async void OnNavigatedTo()
        {
            d = DispatcherQueue.GetForCurrentThread();

            var sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            _session = sessionManager.GetCurrentSession();
            if (_session != null)
            {
                _session.MediaPropertiesChanged += OnMediaPropertiesChanged;
            }
        }

        public BitmapImage Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        private async void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            var mediaProperties = await _session.TryGetMediaPropertiesAsync();

            await d.EnqueueAsync(async () => {
                Artist = mediaProperties.Artist;

                var foo = mediaProperties.Thumbnail;


                if (foo != null)
                {
                    var f = await foo.OpenReadAsync();
                    var image = new BitmapImage();
                    await image.SetSourceAsync(f);

                    Thumbnail = image;
                }
            });
      
        }

        public async Task Pause()
        {
            var result = await _session.TryPauseAsync().AsTask().ConfigureAwait(false);
            if (result)
            {
                IsPlaying = false;
            }
        }

        public async Task Play()
        {
            await _session.TryPlayAsync().AsTask().ConfigureAwait(false);
        }

        public async Task Previous()
        {
            await _session.TrySkipPreviousAsync().AsTask().ConfigureAwait(false);
        }

        private async Task UpdateNowPlaying()
        {
            var playbackInfo = _session.GetPlaybackInfo();
            var mediaProperties = await _session.TryGetMediaPropertiesAsync();

            Artist = mediaProperties.Artist;
            Song = mediaProperties.Title;

        }
    }
}
