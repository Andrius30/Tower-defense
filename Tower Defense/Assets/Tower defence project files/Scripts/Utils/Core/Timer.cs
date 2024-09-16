using System;
using UnityEngine;

namespace Andrius.Core.Timers
{

    public class Timer
    {
        float startTime = 0;
        bool isDone = false;
        bool ignoreTimeScale = false;
        Action onDone = null;
        Action<object[]> onDoneWithParams = null;
        object[] param = default;

        #region Builder
        public Timer BuildWithStartTime(float startTime)
        {
            this.startTime = startTime;
            return this;
        }
        public Timer BuildWithIgnoreTimeScale(bool ignoreTimeScale)
        {
            this.ignoreTimeScale = ignoreTimeScale;
            return this;
        }
        public Timer BuildWithIsDone(bool isDone)
        {
            this.isDone = isDone;
            return this;
        }
        public Timer BuildWithOnDone(Action onDone)
        {
            this.onDone = onDone;
            return this;
        }
        public Timer BuildWithOnDoneWithParams(Action<object[]> onDoneWithParams)
        {
            this.onDoneWithParams = onDoneWithParams;
            return this;
        }
        public Timer BuildWithParam(object[] param)
        {
            this.param = param;
            return this;
        }
        public Timer Build()
        {
            return this;
        }
        public void StartTimer()
        {
            if (isDone) return;
            if (startTime > 0)
            {
                if (ignoreTimeScale)
                {
                    startTime -= Time.unscaledDeltaTime;
                }
                else
                {
                    startTime -= Time.deltaTime;
                }
            }
            else
            {
                startTime = 0;
                isDone = true;
                if (param != null)
                {
                    onDoneWithParams?.Invoke(param);
                }
                onDone?.Invoke();
            }
        }
        #endregion

        public void SetTimer(float time, bool done)
        {
            this.startTime = time;
            this.isDone = done;
        }
        public void SetTimer(float time, bool done, object[] param)
        {
            this.startTime = time;
            this.isDone = done;
            this.param = param;
        }
        public bool IsDone() => isDone;
        public float GetCurrentTime() => startTime;
    }
}
