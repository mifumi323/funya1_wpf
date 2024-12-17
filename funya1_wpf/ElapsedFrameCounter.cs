namespace funya1_wpf
{
    /// <summary>
    /// 経過フレームカウンター
    /// </summary>
    public class ElapsedFrameCounter
    {
        /// <summary>FPS</summary>
        double fps;
        /// <summary>前回のTick数</summary>
        long previousTicks = 0;
        /// <summary>1フレーム当たりのTick数</summary>
        long ticksPerFrame;

        /// <summary>FPSを設定または取得します</summary>
        public double Fps
        {
            get { return fps; }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0.0);
                fps = value;
                if (fps == 0)
                {
                    ticksPerFrame = 0;
                }
                else
                {
                    ticksPerFrame = (long)(10000000 / fps);
                }
            }
        }

        /// <summary>
        /// 経過フレーム数の上限を取得または設定します。
        /// </summary>
        public long FrameLimit { get; set; } = 10;

        public ElapsedFrameCounter(double fps = 60.0)
        {
            Fps = fps;
            Reset();
        }

        public void Reset() => previousTicks = DateTime.Now.Ticks;

        /// <summary>
        /// 経過フレーム数を取得します。
        /// </summary>
        /// <param name="step">取得と同時にフレームを進めるかどうか</param>
        /// <returns>経過フレーム数</returns>
        public long GetElapsedFrames(bool step = false)
        {
            // 経過時間の所得
            var elapsedTicks = GetElapsedTicks();
            var elapsedFrames = elapsedTicks / ticksPerFrame;

            // ちょっとでも時間経過していれば1フレームは返すようにする
            if (elapsedFrames == 0)
            {
                if (elapsedTicks > 0)
                {
                    elapsedTicks = 1;
                }
            }

            // フレーム経過処理
            if (elapsedFrames > 0)
            {
                if (elapsedTicks <= ticksPerFrame * FrameLimit)
                {
                    if (step)
                    {
                        previousTicks += ticksPerFrame * elapsedFrames;
                    }
                    return elapsedFrames;
                }
                else
                {
                    if (step)
                    {
                        Reset();
                    }
                    return FrameLimit;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 前回からの経過時間を取得します。
        /// </summary>
        /// <returns></returns>
        public long GetElapsedTicks() => DateTime.Now.Ticks - previousTicks;

        /// <summary>
        /// フレームを進めます。
        /// </summary>
        public void Step(long frames = 1)
        {
            var elapsedTicks = GetElapsedTicks();
            if (elapsedTicks <= ticksPerFrame * FrameLimit)
            {
                previousTicks += ticksPerFrame * frames;
            }
            else
            {
                Reset();
            }
        }
    }
}
