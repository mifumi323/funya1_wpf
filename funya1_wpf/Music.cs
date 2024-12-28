using System.IO;
using System.Windows.Media;

namespace funya1_wpf
{
    public class Music
    {
        public MusicOptions Options = new();
        public MediaPlayer Player = new();
        public MusicInfo? Playing;

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

        public void Play(MusicInfo music)
        {
            Stop();
            if (!Options.IsEnabled || music.FilePath == "" || !File.Exists(music.FilePath))
            {
                return;
            }
            Player.Open(new Uri(music.FilePath));
            Player.Play();
            Playing = music;
        }

        public void Stop()
        {
            Player.Stop();
            Playing = null;
        }
    }
}
