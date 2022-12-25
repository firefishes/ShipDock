using ShipDock.Commons;
using ShipDock.Notices;
using ShipDock.Ticks;
using System;

namespace ShipDock.Applications
{
    public class TimeUpdater : MethodUpdater
    {
        /// <summary>
        /// 获取一个新的定时器，但在其功能完成后不自动销毁
        /// </summary>
        /// <param name="totalTime"></param>
        /// <param name="method"></param>
        /// <param name="cancelCondition"></param>
        /// <param name="repeats"></param>
        /// <returns></returns>
        public static TimeUpdater GetTimeUpdater(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            return new TimeUpdater(totalTime, method, cancelCondition, repeats);
        }

        /// <summary>
        /// 创建一个新的定时器，并在完成其功能后自动销毁
        /// </summary>
        /// <param name="totalTime"></param>
        /// <param name="method"></param>
        /// <param name="cancelCondition"></param>
        /// <param name="repeat"></param>
        /// <returns></returns>
        public static TimeUpdater New(float totalTime, Action method, Func<bool> cancelCondition = default, int repeat = 1)
        {
            TimeUpdater timer = GetTimeUpdater(totalTime, method, cancelCondition, repeat);
            timer.Start(true);
            return timer;
        }

        private int mRepeats;
        private Func<bool> mCancelChecker;

        public int TotalRepeats { get; private set; }
        public float Time { get; private set; }
        public float TotalTime { get; private set; }
        public Action Completion { get; private set; }
        public bool Repeatable { get; private set; }
        public bool HasStart { get; private set; }
        public bool IsTimeCounting { get; private set; }
        public bool IsAutoDispose { get; set; }
        public bool IsStarted { get; private set; }
        public bool IsPause { get; private set; }

        public bool IsCompleteCycle
        {
            get
            {
                return HasStart && !IsTimeCounting;
            }
        }

        public TimeUpdater(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            Recreate(totalTime, method, cancelCondition, repeats);
        }

        public override void Reclaim()
        {
            base.Reclaim();

            Stop();

            TotalRepeats = 0;
            Completion = default;
            mCancelChecker = default;
        }

        public void Recreate(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            Time = 0f;
            TotalRepeats = repeats;
            TotalTime = totalTime;
            Completion = method;
            mRepeats = repeats;
            Repeatable = mRepeats > 0;
            mCancelChecker = cancelCondition;

            UpdaterNotice.RemoveSceneUpdater(this);
        }

        public void Start()
        {
            HasStart = true;
            IsTimeCounting = true;
            IsStarted = true;
            if (IsPause)
            {
                IsPause = false;
            }
            else
            {
                UpdaterNotice.AddSceneUpdater(this);
            }
        }

        public void Start(bool autoDispose)
        {
            IsAutoDispose = autoDispose;

            Start();
        }

        public void Restart()
        {
            Recreate(TotalTime, Completion, mCancelChecker, TotalRepeats);
            Start();
        }

        public void Pause()
        {
            if (IsStarted)
            {
                HasStart = false;
                IsTimeCounting = false;
                IsPause = true;
            }
            else { }
        }

        public void Stop()
        {
            Pause();

            Time = 0;
            IsStarted = false;
            IsPause = false;

            UpdaterNotice.RemoveSceneUpdater(this);
        }

        public override void OnUpdate(int dTime)
        {
            base.OnUpdate(dTime);

            if (HasStart)
            {
                TimeCountting(dTime);
            }
            else { }
        }

        /// <summary>
        /// 是否还能重复计时
        /// </summary>
        private bool ShouldRepeat()
        {
            mRepeats--;
            bool result = mRepeats > 0;
            if (!result)
            {
                Stop();
            }
            else { }
            return result;
        }

        /// <summary>
        /// 检测定时器退出条件
        /// </summary>
        private bool CheckCancel()
        {
            bool result = false;
            if (mCancelChecker.Invoke())
            {
                result = true;
                Stop();
            }
            else { }
            return result;
        }

        private void TimeCountting(int dTime)
        {
            if (IsPause || !IsStarted)
            {
                return;
            }
            else { }

            float t = (float)dTime / UpdatesCacher.UPDATE_CACHER_TIME_SCALE;
            Time += t;
            if (Time >= TotalTime)
            {
                IsTimeCounting = false;
                if (Repeatable && !ShouldRepeat())
                {
                    Completion?.Invoke();
                    if (IsAutoDispose)
                    {
                        Reclaim();
                    }
                    else { }
                }
                else
                {
                    if (mCancelChecker != default)
                    {
                        CheckCancelInNoRepeats();
                    }
                    else
                    {
                        TimerPermanent();
                    }
                }
            }
            else
            {
                if (mCancelChecker != default)
                {
                    CheckCancelInTimeCounting();
                }
                else { }
            }
        }

        /// <summary>
        /// 设置定时器永久重复的计时器持续新一轮周期所需的值
        /// </summary>
        private void TimerPermanent()
        {
            IsTimeCounting = true;
            Completion?.Invoke();
            Time -= TotalTime;
        }

        /// <summary>
        /// 在定时器计时完成一个周期检测取消条件
        /// </summary>
        private void CheckCancelInNoRepeats()
        {
            bool flag = CheckCancel();//附带退出条件的定时器

            if (flag)
            {
                if (IsAutoDispose)
                {
                    Reclaim();
                }
                else
                {
                    Stop();
                }
            }
            else
            {
                TimerPermanent();
            }
        }

        /// <summary>
        /// 在定时器计时过程中检测取消条件
        /// </summary>
        private void CheckCancelInTimeCounting()
        {
            bool flag = CheckCancel();//附带退出条件的定时器

            if (flag)
            {
                if (IsAutoDispose)
                {
                    Reclaim();
                }
                else
                {
                    Stop();
                }
            }
            else { }
        }

        public void SetComplete(Action method)
        {
            Completion += method;
        }

        public void SetTimeOffset(float time)
        {
            Time = time;
        }
    }
}