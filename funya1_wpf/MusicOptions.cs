namespace funya1_wpf
{
    public class MusicOptions
    {
        public bool IsEnabled { get; set; } = true;
        public MusicInfo Playing { get; set; } = new();
        public MusicInfo Clear { get; set; } = new();
        public MusicInfo Ending { get; set; } = new();
        public MusicInfo Missing { get; set; } = new();
        public MusicInfo GameOver { get; set; } = new();

        public MusicOptions Clone() => new()
        {
            IsEnabled = IsEnabled,
            Playing = Playing.Clone(),
            Clear = Clear.Clone(),
            Ending = Ending.Clone(),
            Missing = Missing.Clone(),
            GameOver = GameOver.Clone()
        };
    }
}
