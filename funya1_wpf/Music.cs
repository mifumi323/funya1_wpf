using System.IO;
using System.Windows.Media;

namespace funya1_wpf
{
    public class Music
    {
        public MusicOptions Options = new();
        public MediaPlayer Player = new();
        public MusicInfo? Playing;

        public int Volume { get; set; }
        public int ActualVolume { get; private set; }
        public const int NormalVolume = 50;
        public const int SleepVolume = 10;

        public Music()
        {
            Player.MediaEnded += Player_MediaEnded;
        }

        private void Player_MediaEnded(object? sender, EventArgs e)
        {
            if (Playing != null && Playing.IsLoop)
            {
                Player.Position = TimeSpan.Zero;
                Player.Play();
            }
            else
            {
                Stop();
            }
        }

        public void Play(MusicInfo music, int volume = NormalVolume)
        {
            Stop();
            if (!Options.IsEnabled || music.FilePath == "" || !File.Exists(music.FilePath))
            {
                return;
            }
            Volume = volume;
            ActualVolume = volume;
            Player.Open(new Uri(music.FilePath));
            Player.Volume = Volume / 100.0;
            Player.Play();
            Playing = music;
        }

        public void Stop()
        {
            Player.Stop();
            Playing = null;
        }

        public void OnFrame()
        {
            if (Playing == null)
            {
                return;
            }
            if (ActualVolume > Volume)
            {
                ActualVolume--;
                Player.Volume = ActualVolume / 100.0;
            }
            else if (ActualVolume < Volume)
            {
                ActualVolume++;
                Player.Volume = ActualVolume / 100.0;
            }
        }
    }
}
