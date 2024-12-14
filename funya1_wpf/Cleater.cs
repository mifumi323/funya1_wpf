using System.Windows.Media;

namespace funya1_wpf
{
    public class Cleater(FormMain formMain)
    {
        public string StageFile = "";

        public RangeArray<MapData> Map = new(0, 31, i => new MapData());
        public string[,] MapText = new string[31, 40]; // 0 To 30, 0 To 39

        public int StageCount;
        public int CurrentStage;
        public int RemainFood;
        public int AnimationCounter;
        public Color StageColor = Color.FromRgb(0, 0, 0);

        public int Rest;
        public int RestMax;
        public int Friction;
        public int EndingType;

        public Secrets Secrets = new();
        public Options Options = new();

        public Status Status;
        public GameState GameState;

        public void Ending2()
        {
            GameState = GameState.Ending2;
        }

        public void AllClear()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void Ending()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public ControlMode ControlMode =>
            Status == Status.Charge ? ControlMode.Charge :
            Status == Status.JumpingUp || Status == Status.JumpingDown ? ControlMode.InAir :
            ControlMode.Ground;

        public void Die()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void ResumeGame()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void Pause()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public bool TouchBottom()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void CheckFood()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void PlayMusic(string? MusicFile)
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void CollisionHorizontal()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public bool TouchRight()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public bool TouchLeft()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public bool TouchTop()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void MoveChara(int NewLeft, int NewTop)
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void SetStage()
        {
            // TODO: 実装
        }

        public void StartStage(int NextStage)
        {
            // TODO: 実装
        }

        public void LoadFile()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void GameStart()
        {
            if (string.IsNullOrEmpty(StageFile))
            {
                SampleStage();
            }
            else
            {
                LoadFile();
            }
            SetMenuStage();
            Rest = RestMax;
            ResetStage();
            SetStage();
            StartStage(CurrentStage);
            formMain.Client.Background = new SolidColorBrush(StageColor);
        }

        public void ResetStage()
        {
            // TODO: 実装
        }

        public void SampleStage()
        {
            StageCount = 1;
            Map[1].Title = "サンプルステージ";
            Map[1].Width = 15;
            Map[1].Height = 8;
            Map[1].StartX = 4;
            Map[1].StartY = 6;
            MapText[1, 0] = "2211111200000000";
            MapText[1, 1] = "2000000200000000";
            MapText[1, 2] = "1000111222200002";
            MapText[1, 3] = "2000000002222001";
            MapText[1, 4] = "2100000102222001";
            MapText[1, 5] = "2200000100000001";
            MapText[1, 6] = "2211000100011111";
            MapText[1, 7] = "2222111100010000";
            MapText[1, 8] = "0000000111110000";
            Map[1].TotalFood = 3;
            Map[1].Food[1].x = 1;
            Map[1].Food[1].y = 3;
            Map[1].Food[2].x = 6;
            Map[1].Food[2].y = 1;
            Map[1].Food[3].x = 11;
            Map[1].Food[3].y = 3;
            RestMax = 5;
            Friction = 10;
            EndingType = 0;
        }

        public void SetMenuStage()
        {
            CurrentStage = 1;
            formMain.UpdateMenuStage();
        }

        public void ShowMessage(string MessageText, MessageMode Mode)
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void StopMusic()
        {
            // TODO: 実装
            throw new NotImplementedException();
        }
    }
}
