using LibVLCSharp.Shared;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using Widgets.Common;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace Radio
{
    public class RadioViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly LibVLC _libVLC;
        public readonly MediaPlayer _mediaPlayer;
        public ObservableCollection<Radio> RadioList { get; set; }
        private readonly System.Timers.Timer _metaDataTimer = new(2000);
        private int _reconnectionAttempt = 0;
        private readonly int _reconnectionAttemptMax = 10;

        public struct SettingsStruct
        {
            public ObservableCollection<Radio> RadioList { get; set; }
            public int RadioIndex { get; set; }
            public int Volume { get; set; }
            public float FontSize { get; set; }
            public SolidColorBrush PrimaryColor { get; set; }
            public SolidColorBrush PrimaryColorLight { get; set; }
            public SolidColorBrush SecondaryColor { get; set; }
            public SolidColorBrush SecondaryColorLight { get; set; }
            public SolidColorBrush TextColor { get; set; }
        }

        public static SettingsStruct Default => new()
        {
            RadioList = [
                new Radio{
                    Name = "Soho Radio",
                    Url = "https://sohoradiomusic.doughunt.co.uk:8010/320mp3",
                    Description = "Live",
                }
            ],
            Volume = 100,
            RadioIndex = 0,
            FontSize = 14,
            PrimaryColor = new SolidColorBrush(Colors.Brown),
            PrimaryColorLight = new SolidColorBrush(Colors.RosyBrown),
            SecondaryColor = new SolidColorBrush(Color.FromRgb(33, 33, 33)),
            SecondaryColorLight = new SolidColorBrush(Colors.LightGray),
            TextColor = new SolidColorBrush(Colors.White),
        };

        private SettingsStruct _settings = Default;
        public SettingsStruct Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }

        private Geometry _volumeIcon = IconPaths.SoundLow();
        public Geometry VolumeIcon
        {
            get { return _volumeIcon; }
            set
            {
                if (_volumeIcon != value)
                {
                    _volumeIcon = value;
                    OnPropertyChanged(nameof(VolumeIcon));
                }
            }
        }

        private Geometry _playStopIcon = IconPaths.Play();
        public Geometry PlayStopIcon
        {
            get { return _playStopIcon; }
            set
            {
                if (_playStopIcon != value)
                {
                    _playStopIcon = value;
                    OnPropertyChanged(nameof(PlayStopIcon));
                }
            }
        }

        public Geometry ForwardIcon { get; } = IconPaths.Forward();
        public Geometry BackwardIcon { get; } = IconPaths.Backward();
        public Geometry NotaSingleIcon { get; } = IconPaths.NotaSingle();
        public Geometry NotaDualIcon { get; } = IconPaths.NotaDual();

        private int _volume;
        public int Volume
        {
            get { return _volume; }
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    _mediaPlayer.Volume = value;
                    Mute = _volume < 0;
                    SetVolumeIcon(_volume);
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        private bool _mute;
        public bool Mute
        {
            get { return _mute; }
            set
            {
                if (_mute != value)
                {
                    _mute = value;
                    _mediaPlayer.Mute = value;
                    var volume = _mute ? 0 : Volume;
                    SetVolumeIcon(volume);
                    OnPropertyChanged(nameof(Mute));
                }
            }
        }

        private Radio? _selectedRadio;
        public Radio? SelectedRadio
        {
            get => _selectedRadio;
            set
            {
                _selectedRadio = value;
                OnPropertyChanged(nameof(SelectedRadio));
            }
        }


        private string? _mediaTitle;
        public string? MediaTitle
        {
            get { return _mediaTitle; }
            set
            {
                if (_mediaTitle != value)
                {
                    _mediaTitle = string.IsNullOrEmpty(value) ? value : " 🎵 " + value + " 🎵 ";
                    OnPropertyChanged($"{nameof(MediaTitle)}");
                }
            }
        }

        private string? _mediaNowPlaying;
        public string? MediaNowPlaying
        {
            get { return _mediaNowPlaying; }
            set
            {
                if (_mediaNowPlaying != value)
                {
                    _mediaNowPlaying = string.IsNullOrEmpty(value) ? value : value + " 🎵 ";
                    OnPropertyChanged($"{nameof(MediaNowPlaying)}");
                }
            }
        }

        private string? _mediaGenre;
        public string? MediaGenre
        {
            get { return _mediaGenre; }
            set
            {
                if (_mediaGenre != value)
                {
                    _mediaGenre = string.IsNullOrEmpty(value) ? value : value + " 🎵 ";
                    OnPropertyChanged($"{nameof(MediaGenre)}");
                }
            }
        }

        public RadioViewModel(SettingsStruct settings)
        {
            string? dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Core.Initialize(dllDirectory + "\\libvlc\\win-x64");

            //var options = new[] { "--network-caching=2000", "--demux=avformat", "--codec=avcodec" };
            var options = new[] { "--network-caching=2000" };

            _libVLC = new LibVLC(options);
            _mediaPlayer = new MediaPlayer(_libVLC);

            _libVLC.Log += (sender, e) =>
            {
                //Debug.WriteLine(e.Module + " " + e.SourceFile + " " + e.Message);
                if (e.Level == LibVLCSharp.Shared.LogLevel.Error)
                {
                    Logger.Info($"{LibVLCSharp.Shared.LogLevel.Error}: {e.Message}");
                    MediaTitle = e.Message;
                    MediaNowPlaying = "";
                    MediaGenre = "";
                }
                //Debug.WriteLine($"[LibVLC] {e.Level}: {e.Message}");
            };

            _mediaPlayer.Playing += (sender, e) =>
            {
                _metaDataTimer.Start();
                PlayStopIcon = IconPaths.Pause();
            };

            _mediaPlayer.Stopped += (sender, e) =>
            {
                _metaDataTimer.Stop();
                PlayStopIcon = IconPaths.Play();
            };

            _mediaPlayer.Opening += (sender, e) =>
            {
                _metaDataTimer.Stop();
                PlayStopIcon = IconPaths.Stop();
                MediaTitle = "Radio is opening...";
                MediaNowPlaying = "";
                MediaGenre = "";
            };

            _mediaPlayer.Buffering += (sender, e) =>
            {
                if (e.Cache == 100)
                {
                    _reconnectionAttempt = 0;
                }
            };

            _mediaPlayer.EndReached += async (sender, e) =>
            {
                _metaDataTimer.Stop();
                PlayStopIcon = IconPaths.Stop();
                MediaNowPlaying = "";
                MediaGenre = "";

                await Task.Delay(5000);

                if (_reconnectionAttempt < _reconnectionAttemptMax)
                {
                    MediaTitle = "Reconnecting...";
                    _reconnectionAttempt++;
                    Play();
                }
                else
                {
                    MediaTitle = "Stream Ended...";
                    _reconnectionAttempt = 0;
                }
            };


            Settings = settings;
            RadioList = Settings.RadioList;
            if (Settings.RadioIndex >= 0 && Settings.RadioIndex < RadioList.Count)
            {
                SelectedRadio = RadioList[Settings.RadioIndex];
            }
            Volume = _mediaPlayer.Volume;
            Mute = _mediaPlayer.Mute;
            Media_MetaUpdater();
        }

        public void Play()
        {
            if (SelectedRadio == null) return;

            if (PlayerState() != VLCState.Opening ||
                PlayerState() != VLCState.Buffering)
            {
                var streamUrl = SelectedRadio.Url;
                var media = new Media(_libVLC, streamUrl, FromType.FromLocation);
                media.Parse(MediaParseOptions.ParseNetwork, -1);
                _mediaPlayer.Play(media);
            }
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
        }

        public VLCState PlayerState()
        {
            return _mediaPlayer.State;
        }

        private void SetVolumeIcon(int volume)
        {
            if (volume > 50)
            {
                VolumeIcon = IconPaths.SoundHigh();
            }
            else if (volume > 0)
            {
                VolumeIcon = IconPaths.SoundLow();
            }
            else
            {
                VolumeIcon = IconPaths.SoundMute();
            }
        }

        //private void Media_MetaChanged(object? sender, MediaMetaChangedEventArgs e)
        //{
        //    if (_mediaPlayer.Media != null)
        //    {
        //        MediaTitle = _mediaPlayer.Media.Meta(MetadataType.Title);
        //        MediaNowPlaying = _mediaPlayer.Media.Meta(MetadataType.NowPlaying);
        //        MediaGenre = _mediaPlayer.Media.Meta(MetadataType.Genre);
        //    }
        //}

        private void Media_MetaUpdater()
        {
            _metaDataTimer.Elapsed += (sender, args) =>
            {
                if (_mediaPlayer.Media != null)
                {
                    MediaTitle = _mediaPlayer.Media.Meta(MetadataType.Title);
                    MediaNowPlaying = _mediaPlayer.Media.Meta(MetadataType.NowPlaying);
                    MediaGenre = _mediaPlayer.Media.Meta(MetadataType.Genre);
                }
            };
            _metaDataTimer.Start();
        }

        public void Dispose()
        {
            _mediaPlayer?.Dispose();
            _metaDataTimer?.Dispose();
            GC.SuppressFinalize(this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
