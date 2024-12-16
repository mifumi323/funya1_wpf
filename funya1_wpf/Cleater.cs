using System.Windows;
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
        public int MovieCounter;
        public Color StageColor = Color.FromRgb(0, 0, 0);
        public BitmapSource Image = null!;
        public CroppedBitmap?[] croppedBitmaps = new CroppedBitmap?[10];

        public int Rest;
        public int RestMax;
        public int Friction;
        public int EndingType;

        public Secrets Secrets = new();
        public Options Options = new();
        public Resources Resources = new();
        public Misc Misc = new();
        public Random Random = new();

        public Status Status;
        public GameState GameState;

        private int MainLeft;
        private int MainTop;
        private int MainCenterX;
        private int MainCenterY;
        private int MainRight;
        private int MainBottom;
        private int MainIndexX;
        private int MainIndexY;
        public int SpeedX = 0;
        public int SpeedY = 0;
        public int JumpCharge;

        public bool PressedDownKey;
        public bool PressedUpKey;
        public HorizontalInput HorizontalInput;

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
            if (Map[CurrentStage].Data[MainIndexX, MainIndexY + 1] == 1)
            {
                if (MainBottom > (((MainIndexY + 1) * 32) - 1))
                {
                    return true;
                }
            }
            return false;
        }

        public void CheckFood()
        {
            // TODO: 実装
        }

        public void PlayMusic(string? MusicFile)
        {
            // TODO: 実装
            throw new NotImplementedException();
        }

        public void CollisionHorizontal()
        {
            // TODO: 実装
        }

        public bool TouchRight()
        {
            // TODO: 実装
            return false;
        }

        public bool TouchLeft()
        {
            // TODO: 実装
            return false;
        }

        public bool TouchTop()
        {
            // TODO: 実装
            return false;
        }

        public void MoveChara(int NewLeft, int NewTop)
        {
            int OffsetY = 0;

            MainLeft = NewLeft;
            MainTop = NewTop;
            MainCenterX = NewLeft + 14;
            MainCenterY = NewTop + 15;
            MainRight = NewLeft + 28;
            MainBottom = NewTop + 32;
            MainIndexX = MainCenterX / 32;
            MainIndexY = MainCenterY / 32;

            switch (Status)
            {
                case Status.Standing:
                    OffsetY = 2;
                    formMain.MineImage.Source = Resources.Stand;
                    break;
                case Status.Sitting:
                    OffsetY = 2;
                    formMain.MineImage.Source = Resources.Sit;
                    break;
                case Status.Charge:
                    // なんもやらんでええんか？
                    break;
                case Status.JumpingUp:
                    OffsetY = 0;
                    if (SpeedX < 0)
                    {
                        formMain.MineImage.Source = Resources.JumpL;
                    }
                    else if (SpeedX == 0)
                    {
                        formMain.MineImage.Source = Resources.Jump;
                    }
                    else
                    {
                        formMain.MineImage.Source = Resources.JumpR;
                    }
                    break;
                case Status.JumpingDown:
                    OffsetY = 5;
                    if (SpeedX < 0)
                    {
                        formMain.MineImage.Source = Resources.FallL;
                    }
                    else if (SpeedX == 0)
                    {
                        formMain.MineImage.Source = Resources.Fall;
                    }
                    else
                    {
                        formMain.MineImage.Source = Resources.FallR;
                    }
                    break;
                case Status.RunningL:
                    Misc.Change12321(ref AnimationCounter, 0, 2);
                    OffsetY = 2;
                    formMain.MineImage.Source = Resources.RunL[AnimationCounter];
                    break;
                case Status.RunningR:
                    Misc.Change12321(ref AnimationCounter, 0, 2);
                    OffsetY = 2;
                    formMain.MineImage.Source = Resources.RunR[AnimationCounter];
                    break;
                case Status.SlippingL:
                case Status.SlippingR:
                    OffsetY = 2;
                    break;
                case Status.WalkingL:
                    OffsetY = 2;
                    formMain.MineImage.Source = Resources.WalkL;
                    break;
                case Status.WalkingR:
                    OffsetY = 2;
                    formMain.MineImage.Source = Resources.WalkR;
                    break;
                case Status.Slepping:
                    // なんもやらんでええんか？
                    break;
                case Status.Smile:
                    // なんもやらんでええんか？
                    break;
                default:
                    break;
            }
            if (!TouchBottom() && Status != Status.JumpingUp)
            {
                Status = Status.JumpingDown;
            }

            Canvas.SetLeft(formMain.Mine, MainLeft - 2);
            Canvas.SetTop(formMain.Mine, MainBottom - 32 - OffsetY);
            Canvas.SetLeft(formMain.Stage, (int)((formMain.Client.Width / 2.0) - MainCenterX));
            Canvas.SetTop(formMain.Stage, (int)((formMain.Client.Height / 2.0) - MainCenterY));

            if (GameState == GameState.Playing)
            {
                CheckFood();
            }
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
            if (GameState == GameState.Playing)
            {
                formMain.Title = $"{Map[NextStage].Title}(残り{Rest}) - ふにゃ";
            }
            DrawTerrain(NextStage);
            for (int i = 1; i <= 5; i++)
            {
                if (i <= Map[NextStage].TotalFood)
                {
                    formMain.Foods[i].Visibility = Visibility.Visible;
                    Canvas.SetLeft(formMain.Foods[i], Map[NextStage].Food[i].x * 32);
                    Canvas.SetTop(formMain.Foods[i], Map[NextStage].Food[i].y * 32);
                }
                else
                {
                    formMain.Foods[i].Visibility = Visibility.Collapsed;
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
            Image = Resources.BlockData1;
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

        public void MainLoop()
        {
            switch (GameState)
            {
                case GameState.Playing:
                case GameState.Ending:
                case GameState.Ending2:
                    switch (Status)
                    {
                        case Status.Standing:
                            formMain.MineImage.Source = Random.Next(100) < 2 ? Resources.Wink : (ImageSource)Resources.Stand;
                            SpeedX = 0;
                            break;
                        case Status.Sitting:
                            SpeedX = 0;
                            break;
                        case Status.Charge:
                            JumpCharge++;
                            if (JumpCharge < 2)
                            {
                                SpeedY = 28;
                            }
                            else if (JumpCharge < 10)
                            {
                                SpeedY = 22;
                            }
                            else if (JumpCharge < 25)
                            {
                                SpeedY = 16;
                            }
                            else
                            {
                                SpeedY = 1;
                            }
                            break;
                        case Status.JumpingUp:
                            if (SpeedY < 0)
                            {
                                Status = Status.JumpingDown;
                            }
                            if (TouchTop())
                            {
                                Status = Status.JumpingDown;
                                SpeedY = Options.Gravity;
                            }
                            CollisionHorizontal();
                            MoveChara(MainLeft + SpeedX / 10, MainTop - SpeedY / 2);
                            SpeedY -= Options.Gravity;
                            break;
                        case Status.JumpingDown:
                            if (PressedDownKey)
                            {
                                if (SpeedY < 30)
                                {
                                    SpeedY += 2;
                                }
                            }
                            else
                            {
                                if (SpeedY < Options.Gravity * 2)
                                {
                                    SpeedY += Options.Gravity;
                                }
                                if ((SpeedY > Options.Gravity * 2 || Options.Gravity == 0) && PressedUpKey)
                                {
                                    SpeedY -= 2;
                                }
                                if (Options.Gravity == 0 && SpeedY < -30)
                                {
                                    SpeedY = -30;
                                }
                            }
                            switch (HorizontalInput)
                            {
                                case HorizontalInput.None:
                                    break;
                                case HorizontalInput.Left:
                                    SpeedX -= 5 * (Options.Reverse ? -1 : 1);
                                    break;
                                case HorizontalInput.Right:
                                    SpeedX += 5 * (Options.Reverse ? -1 : 1);
                                    break;
                                default:
                                    break;
                            }
                            SaturateSpeedX(100);
                            if (TouchTop() && SpeedY < 0)
                            {
                                PressedUpKey = false;
                                SpeedY = 0;
                            }
                            if (TouchBottom())
                            {
                                SpeedY = 0;
                                MainTop = MainIndexY * 32 + 2;
                                if (PressedDownKey)
                                {
                                    switch (HorizontalInput)
                                    {
                                        case HorizontalInput.None:
                                            Status = Status.Sitting;
                                            break;
                                        case HorizontalInput.Left:
                                            SpeedX = -20;
                                            Status = Status.WalkingL;
                                            break;
                                        case HorizontalInput.Right:
                                            SpeedX = 20;
                                            Status = Status.WalkingR;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (HorizontalInput)
                                    {
                                        case HorizontalInput.None:
                                            Status = Status.Standing;
                                            break;
                                        case HorizontalInput.Left:
                                            Status = Status.RunningL;
                                            break;
                                        case HorizontalInput.Right:
                                            Status = Status.RunningR;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            CollisionHorizontal();
                            MoveChara(MainLeft + SpeedX / 10, MainTop + SpeedY / 2);
                            break;
                        case Status.RunningL:
                            SpeedX -= Friction * (Options.Reverse ? -1 : 1);
                            SaturateSpeedX(100);
                            CollisionHorizontal();
                            MoveChara(MainLeft, MainTop + SpeedX / 10);
                            break;
                        case Status.RunningR:
                            SpeedX += Friction * (Options.Reverse ? -1 : 1);
                            SaturateSpeedX(100);
                            CollisionHorizontal();
                            MoveChara(MainLeft, MainTop + SpeedX / 10);
                            break;
                        case Status.SlippingL:
                            SpeedX += Friction;
                            if (SpeedX >= 0)
                            {
                                Status = Status.Standing;
                            }
                            CollisionHorizontal();
                            MoveChara(MainLeft, MainTop + SpeedX / 10);
                            break;
                        case Status.SlippingR:
                            SpeedX -= Friction;
                            if (SpeedX <= 0)
                            {
                                Status = Status.Standing;
                            }
                            CollisionHorizontal();
                            MoveChara(MainLeft, MainTop + SpeedX / 10);
                            break;
                        case Status.WalkingL:
                            SpeedX -= 5 * (Options.Reverse ? -1 : 1);
                            SaturateSpeedX(20);
                            CollisionHorizontal();
                            MoveChara(MainLeft, MainTop + SpeedX / 10);
                            break;
                        case Status.WalkingR:
                            SpeedX += 5 * (Options.Reverse ? -1 : 1);
                            SaturateSpeedX(20);
                            CollisionHorizontal();
                            MoveChara(MainLeft, MainTop + SpeedX / 10);
                            break;
                        case Status.Slepping:
                            break;
                        case Status.Smile:
                            break;
                        default:
                            break;
                    }
                    if (GameState == GameState.Ending)
                    {
                        MovieCounter++;
                        if (MovieCounter == 50)
                        {
                            Status = Status.RunningR;
                        }
                        else if (MovieCounter == 60)
                        {
                            Status = Status.SlippingR;
                        }
                        else if (MovieCounter == 90)
                        {
                            Status = Status.JumpingUp;
                            SpeedY = 16;
                        }
                        else if (MovieCounter == 100)
                        {
                            formMain.MineImage.Source = Resources.Happy;
                            Status = Status.Smile;
                        }
                        else if (MovieCounter == 110)
                        {
                            GameState = GameState.AllClear;
                            // PlayMusic(MusicFileEnding);
                            ShowMessage("All Clear!", MessageMode.Clear);
                        }
                    }
                    break;
                case GameState.Paused:
                    break;
                case GameState.Dying:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    break;
            }
        }

        private void SaturateSpeedX(int max)
        {
            if (SpeedX < -max)
            {
                SpeedX = -max;
            }
            if (SpeedX > max)
            {
                SpeedX = max;
            }
        }
    }
}
