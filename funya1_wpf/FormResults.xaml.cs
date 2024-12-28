using System.Windows;
using System.Windows.Controls;

namespace funya1_wpf
{
    /// <summary>
    /// FormResults.xaml の相互作用ロジック
    /// </summary>
    public partial class FormResults : Window
    {
        public Results Results { get; set; }

        public class Record(int number, string name, string description, string condition, bool isCleared)
        {
            public string SecretText { get; private set; } = $"秘密機能 {number}";
            public string NameText { get; private set; } = $"- {(isCleared ? name : new string('？', name.Length))} -";
            public int Number { get; private set; } = number;
            public string Name { get; private set; } = isCleared ? name : new string('？', name.Length);
            public string Description { get; private set; } = isCleared ? description : new string('？', description.Length);
            public string Condition { get; private set; } = $"条件：{condition}";
            public bool IsCleared { get; private set; } = isCleared;
        }
        public Record[] Records { get; private set; }

        public FormResults(Results results)
        {
            Results = results;
            Records =
            [
                new(1, "スマイル", "Enterキーでわらうよ", "どこでもいいのでステージをクリア", results.Smile),
                new(2, "スピード", "オプション→スピード", "サンプルステージ以外をノーミスクリア", results.Smile),
                new(3, "セレクト", "ゲーム→指定ステージからスタート", "バナナを500個集めてサンプルステージ以外をクリア", results.StageSelect),
                new(4, "グラビティ", "オプション→重力", "バナナを1000個以上集めてサンプルステージ以外をクリア", results.GravitySet),
                new(5, "ゼロ", "ゼロGステージ追加", "バナナを3000個以上集めてどこでもいいのでステージをクリア", results.ZeroGStage),
                new(6, "リバース", "オプション→反操作", "バナナを5000個以上集めてどこでもいいのでノーミスクリア", results.Reverse),
            ];
            InitializeComponent();
        }

        private void BananaCount_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateBananaCount();
        }

        private void UpdateBananaCount()
        {
            if (int.TryParse(BananaCount.Text, out int count) && count != Results.GetTotal)
            {
                if (0 <= count && count <= Results.GetTotalMax && MessageBox.Show(this, "えっ、書き換えちゃっていいの？", "ほんとにほんとに？", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                {
                    Results.GetTotal = count;
                }
            }
            BananaCount.Text = Results.GetTotal.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RecordsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecordsView.SelectedItem is Record record)
            {
                ConditionText.Content = record.Condition;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateBananaCount();
        }
    }
}
