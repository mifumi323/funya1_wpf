using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace funya1_wpf
{
    public class SaveData(Music music, Results Results, Options Options)
    {
        public void Load()
        {
            // 存在チェック
            var settingsFile = GetFilePath();
            if (!File.Exists(settingsFile))
            {
                LoadFromRegistry();
                return;
            }

            // 読み込み
            var lines = File.ReadAllLines(settingsFile);
            var dictionary = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var p = line.Split('=', 2);
                var key = p[0];
                var value = p[1];
                dictionary[key] = value;
            }

            // 関数
            string LoadString(string key, string defaultValue) => dictionary.TryGetValue(key, out var value) ? value : defaultValue;
            int LoadInt(string key, int defaultValue) => int.TryParse(LoadString(key, defaultValue.ToString()), out var value) ? value : defaultValue;
            double LoadDouble(string key, double defaultValue) => double.TryParse(LoadString(key, defaultValue.ToString()), out var value) ? value : defaultValue;
            bool LoadBool(string key, bool defaultValue) => bool.TryParse(LoadString(key, defaultValue.ToString()), out var value) ? value : defaultValue;

            // 基本オプション
            Options.ScreenSize = (ScreenSize)LoadInt("ScreenSize", (int)Options.ScreenSize);
            Options.WindowState = (WindowState)LoadInt("WindowState", (int)Options.WindowState);
            Options.WindowWidth = LoadDouble("WindowWidth", Options.WindowWidth);
            Options.WindowHeight = LoadDouble("WindowHeight", Options.WindowHeight);
            Options.Interval = LoadInt("Interval", Options.Interval);
            Options.Gravity = LoadInt("Gravity", Options.Gravity);
            Options.Reverse = LoadBool("Reverse", Options.Reverse);

            // 音楽
            music.Options.IsEnabled = LoadBool("MusicEnabled", music.Options.IsEnabled);
            music.Options.Playing.FilePath = LoadString("MusicPlaying", music.Options.Playing.FilePath);
            music.Options.Playing.IsLoop = LoadBool("MusicPlayingLoop", music.Options.Playing.IsLoop);
            music.Options.Clear.FilePath = LoadString("MusicClear", music.Options.Clear.FilePath);
            music.Options.Clear.IsLoop = LoadBool("MusicClearLoop", music.Options.Clear.IsLoop);
            music.Options.Ending.FilePath = LoadString("MusicEnding", music.Options.Ending.FilePath);
            music.Options.Ending.IsLoop = LoadBool("MusicEndingLoop", music.Options.Ending.IsLoop);
            music.Options.Missing.FilePath = LoadString("MusicMissing", music.Options.Missing.FilePath);
            music.Options.Missing.IsLoop = LoadBool("MusicMissingLoop", music.Options.Missing.IsLoop);
            music.Options.GameOver.FilePath = LoadString("MusicGameOver", music.Options.GameOver.FilePath);
            music.Options.GameOver.IsLoop = LoadBool("MusicGameOverLoop", music.Options.GameOver.IsLoop);

            // リザルト
            Results.GetTotal = LoadInt("GetTotal", Results.GetTotal);
            Results.Smile = LoadBool("Smile", Results.Smile);
            Results.SpeedSet = LoadBool("SpeedSet", Results.SpeedSet);
            Results.StageSelect = LoadBool("StageSelect", Results.StageSelect);
            Results.GravitySet = LoadBool("GravitySet", Results.GravitySet);
            Results.ZeroGStage = LoadBool("ZeroGStage", Results.ZeroGStage);
            Results.Reverse = LoadBool("ReverseSet", Results.Reverse);
        }

        private void LoadFromRegistry()
        {
            if (TryGetVb6Setting("Get Total", out var GetTotal) && int.TryParse(GetTotal, out var GetTotalInt))
            {
                Results.GetTotal = GetTotalInt;
            }
            if (TryGetVb6Setting("Smile", out var Smile) && bool.TryParse(Smile, out var SmileBool))
            {
                Results.Smile = SmileBool;
            }
            if (TryGetVb6Setting("Speed Set", out var SpeedSet) && bool.TryParse(SpeedSet, out var SpeedSetBool))
            {
                Results.SpeedSet = SpeedSetBool;
            }
            if (TryGetVb6Setting("Stage Select", out var StageSelect) && bool.TryParse(StageSelect, out var StageSelectBool))
            {
                Results.StageSelect = StageSelectBool;
            }
            if (TryGetVb6Setting("Gravity Set", out var GravitySet) && bool.TryParse(GravitySet, out var GravitySetBool))
            {
                Results.GravitySet = GravitySetBool;
            }
            if (TryGetVb6Setting("ZeroG Stage", out var ZeroGStage) && bool.TryParse(ZeroGStage, out var ZeroGStageBool))
            {
                Results.ZeroGStage = ZeroGStageBool;
            }
            if (TryGetVb6Setting("Reverse", out var Reverse) && bool.TryParse(Reverse, out var ReverseBool))
            {
                Results.Reverse = ReverseBool;
            }
            if (TryGetVb6Setting("Music", out var MusicEnabled) && bool.TryParse(MusicEnabled, out var MusicBool))
            {
                music.Options.IsEnabled = MusicBool;
            }
            if (TryGetVb6Setting("Music File Playing", out var MusicFilePlaying))
            {
                music.Options.Playing.FilePath = MusicFilePlaying;
            }
            if (TryGetVb6Setting("Music File Clear", out var MusicFileClear))
            {
                music.Options.Clear.FilePath = MusicFileClear;
            }
            if (TryGetVb6Setting("Music File Ending", out var MusicFileEnding))
            {
                music.Options.Ending.FilePath = MusicFileEnding;
            }
            if (TryGetVb6Setting("Music File Missing", out var MusicFileMissing))
            {
                music.Options.Missing.FilePath = MusicFileMissing;
            }
            if (TryGetVb6Setting("Music File Game Over", out var MusicFileGameOver))
            {
                music.Options.GameOver.FilePath = MusicFileGameOver;
            }
        }

        private static bool TryGetVb6Setting(string key, out string value)
        {
            var registryKey = Registry.CurrentUser.OpenSubKey("Software\\VB and VBA Program Settings\\funya\\Settings");
            if (registryKey == null)
            {
                value = "";
                return false;
            }
            var registryValue = registryKey.GetValue(key);
            if (registryValue == null)
            {
                value = "";
                return false;
            }
            var stringValue = registryValue.ToString();
            if (stringValue == null)
            {
                value = "";
                return false;
            }
            value = stringValue;
            return true;
        }

        public void Save()
        {
            // 存在チェック
            var settingsFile = GetFilePath();
            var settingsDir = Path.GetDirectoryName(settingsFile);
            if (settingsDir != null && !Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            File.WriteAllLines(settingsFile,
            [
                // 基本オプション
                $"ScreenSize={(int)Options.ScreenSize}",
                $"WindowState={(int)Options.WindowState}",
                $"WindowWidth={Options.WindowWidth}",
                $"WindowHeight={Options.WindowHeight}",
                $"Interval={Options.Interval}",
                $"Gravity={Options.Gravity}",
                $"Reverse={Options.Reverse}",

                // 音楽
                $"MusicEnabled={music.Options.IsEnabled}",
                $"MusicPlaying={music.Options.Playing.FilePath}",
                $"MusicPlayingLoop={music.Options.Playing.IsLoop}",
                $"MusicClear={music.Options.Clear.FilePath}",
                $"MusicClearLoop={music.Options.Clear.IsLoop}",
                $"MusicEnding={music.Options.Ending.FilePath}",
                $"MusicEndingLoop={music.Options.Ending.IsLoop}",
                $"MusicMissing={music.Options.Missing.FilePath}",
                $"MusicMissingLoop={music.Options.Missing.IsLoop}",
                $"MusicGameOver={music.Options.GameOver.FilePath}",
                $"MusicGameOverLoop={music.Options.GameOver.IsLoop}",

                // リザルト
                $"GetTotal={Results.GetTotal}",
                $"Smile={Results.Smile}",
                $"SpeedSet={Results.SpeedSet}",
                $"StageSelect={Results.StageSelect}",
                $"GravitySet={Results.GravitySet}",
                $"ZeroGStage={Results.ZeroGStage}",
                $"ReverseSet={Results.Reverse}",
            ]);
        }

        public static string GetFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MifuminSoft", "funya", "settings.ini");
        }
    }
}
