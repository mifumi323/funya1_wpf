
namespace funya1_wpf
{
    public class MusicInfo
    {
        public string FilePath { get; set; } = "";
        public bool IsLoop { get; set; } = true;

        public MusicInfo Clone() => new()
        {
            FilePath = FilePath,
            IsLoop = IsLoop,
        };
    }
}
