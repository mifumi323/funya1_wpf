using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace funya1_wpf
{
    public class Cleater(FormMain formMain)
    {
        public string StageFile = "";

        public RangeArray<MapData> Map = new(0, 30, i => new MapData());
        public string[,] MapText = new string[31, 40]; // 0 To 30, 0 To 39

        public int StageCount;
        public int CurrentStage;
        public int RemainFood;
        public int AnimationCounter;
        public Color StageColor = Color.FromRgb(0, 0, 0);
        public BitmapSource Image = null!;
        public CroppedBitmap?[] croppedBitmaps = new CroppedBitmap?[10];

        public int Rest;
        public int RestMax;
        public int Friction;
        public int EndingType;

        public Secrets Secrets = new();
        public Options Options = new();

        public Status Status;
        public GameState GameState;

        public int SpeedX = 0;
        public int SpeedY = 0;


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
        }

        public void SetStage()
        {
            foreach (var map in Map)
            {
                var StageNumber = map.Key;
                var MapValue = map.Value;
                for (int x = 0; x <= MapValue.Width; x++)
                {
                    for (int y = 0; y <= MapValue.Height; y++)
                    {
                        int n =
                            string.IsNullOrEmpty(MapText[StageNumber, y]) ? 0 :
                            y >= MapText[StageNumber, y].Length ? 0 :
                            MapText[StageNumber, y][x] < '0' ? 0 :
                            MapText[StageNumber, y][x] > '9' ? 0 :
                            MapText[StageNumber, y][x] - '0';
                        MapValue.Data[x, y] = n;
                    }
                }
            }
        }

        public void StartStage(int NextStage)
        {
            if (GameState != GameState.Ending && GameState != GameState.Ending2)
            {
                GameState = GameState.Playing;
            }
            if (GameState != GameState.Playing)
            {
                formMain.Title = $"{Map[NextStage].Title}(残り{Rest})";
            }
            DrawTerrain(NextStage);
            for (int i = 1; i <= 5; i++)
            {
                if (i <= Map[NextStage].TotalFood)
                {
                    formMain.Foods[i].Visibility = System.Windows.Visibility.Visible;
                    Canvas.SetLeft(formMain.Foods[i], Map[NextStage].Food[i].x * 32);
                    Canvas.SetTop(formMain.Foods[i], Map[NextStage].Food[i].y * 32);
                }
                else
                {
                    formMain.Foods[i].Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            RemainFood = Map[NextStage].TotalFood;
            CurrentStage = NextStage;
            // Me(68) = 0
            SpeedX = 0;
            SpeedY = 0;
            //Me(90) = 0
            //Me(72) = 0
            Status = Status.Standing;
            MoveChara((32 * Map[NextStage].StartX) + 2, (32 * Map[NextStage].StartY) - 4);
            ResumeGame();
            // PlayMusic(MusicFilePlaying);
        }

        private void DrawTerrain(int NextStage)
        {
            int terrainWidth = 32 * (Map[NextStage].Width + 1);
            formMain.Stage.Width = terrainWidth;
            int terrainHeight = 32 * (Map[NextStage].Height + 1);
            formMain.Stage.Height = terrainHeight;

            var terrainImage = new RenderTargetBitmap(terrainWidth, terrainHeight, 96, 96, PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                for (int x = 0; x <= Map[NextStage].Width; x++)
                {
                    for (int y = 0; y <= Map[NextStage].Height; y++)
                    {
                        CroppedBitmap? imageSource = croppedBitmaps[Map[NextStage].Data[x, y]];
                        if (imageSource != null)
                        {
                            dc.DrawImage(imageSource, new System.Windows.Rect(x * 32, y * 32, 32, 32));
                        }
                    }
                }
            }
            terrainImage.Render(dv);

            formMain.Stage.Background = new ImageBrush(terrainImage);
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
            foreach (var map in Map)
            {
                for (int x = 0; x <= 39; x++)
                {
                    for (int y = 0; y <= 39; y++)
                    {
                        map.Value.Data[x, y] = 0;
                    }
                }
            }
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
            LoadSampleImage();
        }

        private void LoadSampleImage()
        {
            Image = (formMain.SampleTerrain.Source as BitmapSource)!;
            for (int i = 0; i < 10; i++)
            {
                croppedBitmaps[i] = (i + 1) * 32 <= Image.Width ? new(Image, new(i * 32, 0, 32, 32)) : null;
            }
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
