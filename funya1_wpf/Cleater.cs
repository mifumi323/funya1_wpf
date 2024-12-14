﻿using System.Windows.Media;

namespace funya1_wpf
{
    public class Cleater(FormMain formMain)
    {
        public string StageFile = "";

        public MapData[] Map = new MapData[31]; // 0 To 30
        public string[,] MapText = new string[31, 40]; // 0 To 30, 0 To 39

        public int StageCount;
        public int CurrentStage;
        public int RemainFood;
        public int AnimationCounter;
        public Color StageColor = Color.FromRgb(0, 0, 0);

        public int Rest;
        public int RestMax;

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
            // TODO: 実装
        }

        public void SetMenuStage()
        {
            // TODO: 実装
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
