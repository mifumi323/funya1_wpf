﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace funya1_wpf
{
    public class Cleater(FormMain formMain, Music music, Results Results, Options Options, Resources Resources) : StageData(Resources.BlockData1)
    {
        public int CurrentStage;
        public int RemainFood;
        public int AnimationCounter;
        public int MovieCounter;

        public int Rest;

        public Misc Misc = new();
        public Random Random = new();

        public Status Status;
        private GameState gameState;

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
        public int Sleepy;

        public bool PressedDownKey;
        public bool PressedUpKey;
        public HorizontalInput HorizontalInput;

        public void Ending2()
        {
            GameState = GameState.Ending2;
        }

        public void ChangeMineImage(ImageSource image)
        {
            if (image != formMain.MineImage.Source)
            {
                formMain.MineImage.Source = image;
            }
        }

        public void AllClear()
        {
            formMain.Title = $"{Map[CurrentStage].Title}(オールクリア) - ふにゃ";
            if (EndingType == 1)
            {
                Ending();
                return;
            }
            else if (EndingType == 2)
            {
                Ending2();
            }
            else
            {
                ChangeMineImage(Resources.Happy);
                music.Play(music.Options.Ending);
                formMain.ShowMessage("All Clear!", MessageMode.Clear);
                GameState = GameState.AllClear;
            }
            if (!Results.Smile)
            {
                Results.Smile = true;
                formMain.ShowMessage("Secret 1", MessageMode.Clear, "秘密機能 1 - スマイル -");
            }
            if (Rest == RestMax && StageFile != "" && !Results.SpeedSet)
            {
                Results.SpeedSet = true;
                formMain.ShowMessage("Secret 2", MessageMode.Clear, "秘密機能 2 - スピード -");
            }
            if (Results.GetTotal >= 500 && StageFile != "" && !Results.StageSelect)
            {
                Results.StageSelect = true;
                formMain.ShowMessage("Secret 3", MessageMode.Clear, "秘密機能 3 - セレクト -");
            }
            if (Results.GetTotal >= 1000 && StageFile != "" && !Results.GravitySet)
            {
                Results.GravitySet = true;
                formMain.ShowMessage("Secret 4", MessageMode.Clear, "秘密機能 4 - グラビティ -");
            }
            if (Results.GetTotal >= 3000 && !Results.ZeroGStage)
            {
                Results.ZeroGStage = true;
                formMain.ShowMessage("Secret 5", MessageMode.Clear, "秘密機能 5 - ゼロ -");
                CopyZeroGStage();
            }
            if (Results.GetTotal >= 5000 && !Results.Reverse && Rest == RestMax)
            {
                Results.Reverse = true;
                formMain.ShowMessage("Perfect!", MessageMode.Clear, "秘密機能 6 - リバース -");
            }
            formMain.UpdateMenuItems();
        }

        public static bool CopyZeroGStage()
        {
            var result = false;
            result |= CopyFileFromResource("ZeroG.stg");
            result |= CopyFileFromResource("NonOil.bmp");
            return result;
        }

        private static bool CopyFileFromResource(string filename)
        {
            var uri = new Uri($"/Resources/{filename}", UriKind.Relative);
            var info = Application.GetResourceStream(uri);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "funyaMaker", filename);
            // ファイルが存在しない場合のみコピー
            if (!File.Exists(path))
            {
                using var fileStream = File.Create(path);
                info.Stream.CopyTo(fileStream);
                return true;
            }
            return false;
        }

        public void Ending()
        {
            Map[0].MaxX = 13;
            Map[0].MaxY = 9;
            for (var r = 0; r <= 3; r++)
            {
                Map[0].ImportLine(r, "55555555555555");
            }
            Map[0].ImportLine(4, "11111111555555");
            Map[0].ImportLine(5, "00000000555555");
            Map[0].ImportLine(6, "11111111111111");
            for (var r = 7; r <= 9; r++)
            {
                Map[0].ImportLine(r, "55555555555555");
            }
            Map[0].StartX = 5;
            Map[0].StartY = 5;
            Map[0].TotalFood = 1;
            Map[0].Food[1].x = 255;
            GameState = GameState.Ending;
            StartStage(0);
            music.Stop();
        }

        public ControlMode ControlMode =>
            Status == Status.Charge ? ControlMode.Charge :
            Status == Status.JumpingUp || Status == Status.JumpingDown ? ControlMode.InAir :
            ControlMode.Ground;

        public GameState GameState
        {
            get => gameState;
            set
            {
                gameState = value;
                PressedDownKey = false;
                PressedUpKey = false;
                HorizontalInput = HorizontalInput.None;
            }
        }

        public void Die()
        {
            Rest--;
            ChangeMineImage(Resources.Death);
            if (Rest == 0)
            {
                GameState = GameState.GameOver;
                music.Play(music.Options.GameOver);
                formMain.Title = $"{Map[CurrentStage].Title}(ゲームオーバー) - ふにゃ";
                formMain.ShowMessage("Game Over!", MessageMode.Dying);
                if (Results.GetTotal >= 10)
                {
                    formMain.ShowMessage("Continue?", MessageMode.GameOver);
                }
            }
            else
            {
                GameState = GameState.Dying;
                music.Play(music.Options.Missing);
                formMain.Title = $"{Map[CurrentStage].Title}(残り{Rest}) - ふにゃ";
                formMain.ShowMessage("Miss!", MessageMode.Dying);
                formMain.OnMessageClose = _ => StartStage(CurrentStage);
            }
        }

        public void ResumeGame()
        {
            if (GameState == GameState.Paused)
            {
                GameState = GameState.Playing;
                music.Play(music.Options.Playing);
            }
        }

        public void Pause()
        {
            if (GameState == GameState.Playing)
            {
                GameState = GameState.Paused;
                music.Stop();
                if (Status == Status.JumpingUp)
                {
                    if (SpeedX < 0)
                    {
                        ChangeMineImage(Resources.JumpLG);
                    }
                    else if (SpeedX == 0)
                    {
                        ChangeMineImage(Resources.JumpG);
                    }
                    else
                    {
                        ChangeMineImage(Resources.JumpRG);
                    }
                }
                else if (Status == Status.JumpingDown)
                {
                    if (SpeedX < 0)
                    {
                        ChangeMineImage(Resources.FallLG);
                    }
                    else if (SpeedX == 0)
                    {
                        ChangeMineImage(Resources.FallG);
                    }
                    else
                    {
                        ChangeMineImage(Resources.FallRG);
                    }
                }
                else
                {
                    Status = Status.Standing;
                    ChangeMineImage(Resources.funyaG);
                }
            }
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
            for (int i = 1; i <= Map[CurrentStage].TotalFood; i++)
            {
                if (Map[CurrentStage].Food[i].x == MainIndexX && Map[CurrentStage].Food[i].y == MainIndexY)
                {
                    if (formMain.Foods[i].Visibility == Visibility.Visible)
                    {
                        RemainFood--;
                        Results.GetTotal++;
                        if (Results.GetTotal > Results.GetTotalMax)
                        {
                            Results.GetTotal = Results.GetTotalMax;
                        }
                        formMain.Foods[i].Visibility = Visibility.Collapsed;
                        if (RemainFood == 0)
                        {
                            ChangeMineImage(Resources.Happy);
                            GameState = GameState.Clear;
                            music.Play(music.Options.Clear);
                            formMain.Title = $"{Map[CurrentStage].Title}(クリア) - ふにゃ";
                            formMain.ShowMessage("Clear!", MessageMode.Clear);
                            formMain.OnMessageClose = _ =>
                            {
                                if (CurrentStage == StageCount)
                                {
                                    AllClear();
                                }
                                else
                                {
                                    StartStage(CurrentStage + 1);
                                    GameState = GameState.Playing;
                                }
                            };
                        }
                        return;
                    }
                }
            }
            if (Map[CurrentStage].Data[MainIndexX, MainIndexY] >= 2 || MainIndexX == 0 || MainIndexY == 0 || MainIndexX == Map[CurrentStage].MaxX || MainIndexY == Map[CurrentStage].MaxY + 2)
            {
                Die();
            }
        }

        public void CollisionHorizontal()
        {
            var Touched = false;
            if (TouchRight() && SpeedX >= 0)
            {
                SpeedX = 0;
                MainLeft = (MainIndexX * 32) + 4;
                Touched = true;
            }
            if (TouchLeft() && SpeedX <= 0)
            {
                SpeedX = 0;
                MainLeft = 32 * MainIndexX;
                Touched = true;
            }
            if (Touched)
            {
                MoveChara(MainLeft, MainTop);
            }
        }

        public bool TouchRight()
        {
            if (Map[CurrentStage].Data[MainIndexX + 1, MainIndexY] == 1)
            {
                if (MainRight > ((MainIndexX + 1) * 32) - 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TouchLeft()
        {
            if (Map[CurrentStage].Data[MainIndexX - 1, MainIndexY] == 1)
            {
                if (MainLeft < (MainIndexX * 32) + 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TouchTop()
        {
            if (Map[CurrentStage].Data[MainIndexX, MainIndexY - 1] == 1)
            {
                if (MainTop < ((MainIndexY * 32) + 1))
                {
                    return true;
                }
            }
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
                    ChangeMineImage(Resources.Stand);
                    break;
                case Status.Sitting:
                    OffsetY = 2;
                    ChangeMineImage(Resources.Sit);
                    break;
                case Status.Charge:
                    // なんもやらんでええんか？
                    break;
                case Status.JumpingUp:
                    OffsetY = 0;
                    if (SpeedX < 0)
                    {
                        ChangeMineImage(Resources.JumpL);
                    }
                    else if (SpeedX == 0)
                    {
                        ChangeMineImage(Resources.Jump);
                    }
                    else
                    {
                        ChangeMineImage(Resources.JumpR);
                    }
                    break;
                case Status.JumpingDown:
                    OffsetY = 5;
                    if (SpeedX < 0)
                    {
                        ChangeMineImage(Resources.FallL);
                    }
                    else if (SpeedX == 0)
                    {
                        ChangeMineImage(Resources.Fall);
                    }
                    else
                    {
                        ChangeMineImage(Resources.FallR);
                    }
                    break;
                case Status.RunningL:
                    Misc.Change12321(ref AnimationCounter, 0, 2);
                    OffsetY = 2;
                    ChangeMineImage(Resources.RunL[AnimationCounter]);
                    break;
                case Status.RunningR:
                    Misc.Change12321(ref AnimationCounter, 0, 2);
                    OffsetY = 2;
                    ChangeMineImage(Resources.RunR[AnimationCounter]);
                    break;
                case Status.SlippingL:
                case Status.SlippingR:
                    OffsetY = 2;
                    break;
                case Status.WalkingL:
                    OffsetY = 2;
                    ChangeMineImage(Resources.WalkL);
                    break;
                case Status.WalkingR:
                    OffsetY = 2;
                    ChangeMineImage(Resources.WalkR);
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

        public void StartStage(int NextStage)
        {
            formMain.CloseMessage();
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
            SpeedX = 0;
            SpeedY = 0;
            Sleepy = 0;
            Status = Status.Standing;
            MoveChara((32 * Map[NextStage].StartX) + 2, (32 * Map[NextStage].StartY) - 4);
            ResumeGame();
            music.Play(music.Options.Playing);
            formMain.UpdateMenuStage();
        }

        private void DrawTerrain(int NextStage)
        {
            Map[NextStage].DrawTerrainInPanel(formMain.Stage, MapChips);
        }

        public bool GameStart()
        {
            if (string.IsNullOrEmpty(StageFile))
            {
                SampleStage();
            }
            else
            {
                if (!LoadFile())
                {
                    return false;
                }
            }
            SetMenuStage();
            Rest = RestMax;
            StartStage(CurrentStage);
            formMain.Client.Background = new SolidColorBrush(StageColor);

            return true;
        }

        public void SampleStage()
        {
            StageCount = 1;
            Map[1].Title = "サンプルステージ";
            Map[1].MaxX = 15;
            Map[1].MaxY = 8;
            Map[1].StartX = 4;
            Map[1].StartY = 6;
            Map[1].ImportLine(0, "2211111200000000");
            Map[1].ImportLine(1, "2000000200000000");
            Map[1].ImportLine(2, "1000111222200002");
            Map[1].ImportLine(3, "2000000002222001");
            Map[1].ImportLine(4, "2100000102222001");
            Map[1].ImportLine(5, "2200000100000001");
            Map[1].ImportLine(6, "2211000100011111");
            Map[1].ImportLine(7, "2222111100010000");
            Map[1].ImportLine(8, "0000000111110000");
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

        public void SetMenuStage()
        {
            CurrentStage = 1;
            formMain.UpdateMenuStage();
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
                            ChangeMineImage(Random.Next(100) < 2 ? Resources.Wink : (ImageSource)Resources.Stand);
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
                            if (Options.Gravity == 0)
                            {
                                Status = Status.JumpingDown;
                                SpeedY = -SpeedY;
                            }
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
                            MoveChara(MainLeft + SpeedX / 10, MainTop);
                            break;
                        case Status.RunningR:
                            SpeedX += Friction * (Options.Reverse ? -1 : 1);
                            SaturateSpeedX(100);
                            CollisionHorizontal();
                            MoveChara(MainLeft + SpeedX / 10, MainTop);
                            break;
                        case Status.SlippingL:
                            SpeedX += Friction;
                            if (SpeedX >= 0)
                            {
                                Status = Status.Standing;
                            }
                            CollisionHorizontal();
                            MoveChara(MainLeft + SpeedX / 10, MainTop);
                            break;
                        case Status.SlippingR:
                            SpeedX -= Friction;
                            if (SpeedX <= 0)
                            {
                                Status = Status.Standing;
                            }
                            CollisionHorizontal();
                            MoveChara(MainLeft + SpeedX / 10, MainTop);
                            break;
                        case Status.WalkingL:
                            SpeedX -= 5 * (Options.Reverse ? -1 : 1);
                            SaturateSpeedX(20);
                            CollisionHorizontal();
                            MoveChara(MainLeft + SpeedX / 10, MainTop);
                            break;
                        case Status.WalkingR:
                            SpeedX += 5 * (Options.Reverse ? -1 : 1);
                            SaturateSpeedX(20);
                            CollisionHorizontal();
                            MoveChara(MainLeft + SpeedX / 10, MainTop);
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
                            ChangeMineImage(Resources.Happy);
                            Status = Status.Smile;
                        }
                        else if (MovieCounter == 110)
                        {
                            EndingType = 0;
                            AllClear();
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

        public void OnKeyDown(Key key)
        {
            if (GameState != GameState.Playing)
            {
                return;
            }
            if (ControlMode == ControlMode.Ground)
            {
                if (IsUpKey(key))
                {
                    if (SpeedX < 0)
                    {
                        ChangeMineImage(Resources.WalkL);
                    }
                    else if (SpeedX == 0)
                    {
                        ChangeMineImage(Resources.Sit);
                    }
                    else if (SpeedX > 0)
                    {
                        ChangeMineImage(Resources.WalkR);
                    }
                    JumpCharge = 0;
                    SpeedY = 28;
                    Status = Status.Charge;
                }
                else if (IsDownKey(key))
                {
                    ChangeMineImage(Resources.Sit);
                    Status = Status.Sitting;
                }
                else if (IsLeftKey(key))
                {
                    if (Status is Status.Sitting or Status.WalkingL or Status.WalkingR)
                    {
                        Status = Status.WalkingL;
                    }
                    else
                    {
                        Status = Status.RunningL;
                    }
                }
                else if (IsRightKey(key))
                {
                    if (Status is Status.Sitting or Status.WalkingL or Status.WalkingR)
                    {
                        Status = Status.WalkingR;
                    }
                    else
                    {
                        Status = Status.RunningR;
                    }
                }
                else if (IsSmileKey(key) && Results.Smile)
                {
                    ChangeMineImage(Resources.Happy);
                    Status = Status.Smile;
                }
            }
            else
            {
                if (IsLeftKey(key))
                {
                    HorizontalInput = HorizontalInput.Left;
                }
                else if (IsRightKey(key))
                {
                    HorizontalInput = HorizontalInput.Right;
                }
                else if (IsUpKey(key))
                {
                    PressedUpKey = true;
                    PressedDownKey = false;
                }
                else if (IsDownKey(key))
                {
                    PressedDownKey = true;
                    PressedUpKey = false;
                }
            }
        }

        public void OnKeyUp(Key key)
        {
            if (GameState != GameState.Playing)
            {
                return;
            }
            if (ControlMode == ControlMode.Ground)
            {
                if (IsDownKey(key))
                {
                    Status = Status.Standing;
                    MoveChara(MainLeft, MainTop);
                }
                else if (IsLeftKey(key))
                {
                    if (Status == Status.RunningL)
                    {
                        if (SpeedX != 0)
                        {
                            Status = Status.SlippingL;
                        }
                        else
                        {
                            Status = Status.Standing;
                        }
                    }
                    else if (Status == Status.WalkingL)
                    {
                        ChangeMineImage(Resources.Sit);
                        Status = Status.Sitting;
                    }
                }
                else if (IsRightKey(key))
                {
                    if (Status == Status.RunningR)
                    {
                        if (SpeedX != 0)
                        {
                            Status = Status.SlippingR;
                        }
                        else
                        {
                            Status = Status.Standing;
                        }
                    }
                    else if (Status == Status.WalkingR)
                    {
                        ChangeMineImage(Resources.Sit);
                        Status = Status.Sitting;
                    }
                }
                else if (IsSmileKey(key))
                {
                    if (Results.Smile)
                    {
                        ChangeMineImage(Resources.Happy);
                        Status = Status.Standing;
                    }
                }
            }
            else
            {
                if (Status == Status.Charge)
                {
                    if (IsUpKey(key))
                    {
                        ChangeMineImage(Resources.Jump);
                        Status = Status.JumpingUp;
                    }
                }
            }
            if (IsLeftKey(key) || IsRightKey(key))
            {
                HorizontalInput = HorizontalInput.None;
            }
            else if (IsUpKey(key))
            {
                PressedUpKey = false;
            }
            else if (IsDownKey(key))
            {
                PressedDownKey = false;
            }
        }

        private static bool IsUpKey(Key key) => key is Key.Space or Key.Up or Key.W;

        private static bool IsDownKey(Key key) => key is Key.Down or Key.S;

        private static bool IsLeftKey(Key key) => key is Key.Left or Key.A;

        private static bool IsRightKey(Key key) => key is Key.Right or Key.D;

        private static bool IsSmileKey(Key key) => key is Key.Enter;

        public void Sleep()
        {
            if (GameState == GameState.Playing)
            {
                Sleepy++;
                if (Status == Status.Standing)
                {
                    if (Sleepy == 30)
                    {
                        Status = Status.Slepping;
                        ChangeMineImage(Resources.Sleep);
                    }
                }
                else if (Status == Status.Slepping)
                {
                    ChangeMineImage(Sleepy % 2 == 0 ? Resources.Sleep : Resources.Sleep2);
                }
                else
                {
                    Sleepy = 0;
                }
            }
            else
            {
                Sleepy = 0;
            }
        }
    }
}
