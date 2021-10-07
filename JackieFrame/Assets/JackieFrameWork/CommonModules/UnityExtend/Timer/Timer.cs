using System;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    public enum eTimerTickType
    {
        FixedUpdate = 1,
        Update,
        LateUpdate,
    }

    /// <summary>
    /// 定时器管理器
    /// @author hannibal
    /// @time 2016-9-19
    /// </summary>
    public sealed class Timer : DnotMonoSingleton<Timer>
    {
        private uint m_idCounter = 0;
        private List<TimerEntity> m_Timers = new List<TimerEntity>();
        private List<TimerEntity> m_TimerPools = new List<TimerEntity>();
        
        private Dictionary<eTimerTickType, Delegate> m_DicTicks = new Dictionary<eTimerTickType, Delegate>();
        
        void FixedUpdate()
        {
            ProcessTick(eTimerTickType.FixedUpdate);
        }
        void Update()
        {
            float elapse = Time.deltaTime;
            Remove();

            for (int i = m_Timers.Count - 1; i >= 0; i--)
            {
                m_Timers[i].Update(elapse);
            }
            
            ProcessTick(eTimerTickType.Update);
        }
        void LateUpdate()
        {
            ProcessTick(eTimerTickType.LateUpdate);
        }
        /// <summary>
        /// 增加定时器
        /// </summary>
        /// <param name="rate">触发频率(单位s)</param>
        /// <param name="callBack">触发回调函数</param>
        /// <returns>新定时器id</returns>
        public uint AddLoop(float rate, Action callBack)
        {
            return AddTimer(rate, 0, callBack);
        }
        public uint AddLoop(float rate, Action<object> callBack, object info)
        {
            return AddTimer(rate, 0, callBack, info);
        }
        public uint AddOnce(float rate, Action callBack)
        {
            return AddTimer(rate, 1, callBack);
        }
        public uint AddOnce(float rate, Action<object> callBack, object info)
        {
            return AddTimer(rate, 1, callBack, info);
        }
        /// <summary>
        /// 增加定时器，可以指定循环次数
        /// </summary>
        /// <param name="rate">触发频率(单位s)</param>
        /// <param name="ticks">循环次数，如果是0则不会自动删除</param>
        /// <param name="callBack">触发回调函数</param>
        /// <param name="info">回调参数</param>
        /// <returns>新定时器id</returns>
        public uint AddTimer(float rate, int ticks, Action callBack)
        {
            if (rate < 0) rate = 0;
            TimerEntity newTimer = Spawn();
            newTimer.Init(++m_idCounter, rate, ticks, callBack);
            m_Timers.Add(newTimer);
            if (rate == 0) newTimer.Update(0);
            return newTimer.id;
        }
        public uint AddTimer(float rate, int ticks, Action<object> callBack, object info)
        {
            if (rate < 0) rate = 0;
            TimerEntity newTimer = Spawn();
            newTimer.Init(++m_idCounter, rate, ticks, callBack, info);
            m_Timers.Add(newTimer);
            if (rate == 0) newTimer.Update(0);
            return newTimer.id;
        }
        /// <summary>
        /// 下一帧执行一次
        /// </summary>
        /// <param name="callBack"></param>
        public void CallLater(Action callBack)
        {
            TimerEntity newTimer = Spawn();
            newTimer.Init(++m_idCounter, 0, 1, callBack);
            m_Timers.Add(newTimer);
        }
        public void CallLater(Action<object> callBack, object info)
        {
            TimerEntity newTimer = Spawn();
            newTimer.Init(++m_idCounter, 0, 1, callBack, info);
            m_Timers.Add(newTimer);
        }
        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="timerId">Timer GUID</param>
        public void RemoveTimer(uint timerId)
        {
            if (timerId == 0) return;
            //修改状态
            TimerEntity timer = null;
            for (int i = 0; i < m_Timers.Count; ++i)
            {
                timer = m_Timers[i];
                if (timer.id == timerId)
                {
                    timer.isActive = false;
                    break;
                }
            }
        }
        public void RemoveAllTimer()
        {
            m_Timers.ForEach((TimerEntity timer) => 
            {//只删除非循环定时器
                if (timer.mTicks != 0) this.RemoveTimer(timer.id);
            });
        }

        /// <summary>
        /// 移除过期定时器
        /// </summary>
        private void Remove()
        {
            for (int i = m_Timers.Count - 1; i >= 0; i--)
            {
                if (!m_Timers[i].isActive)
                {
                    Despawn(m_Timers[i]);
                    m_Timers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 内存信息
        /// </summary>
        /// <param name="is_print">失败输出日志</param>
        /// <returns></returns>
        public string ToString(bool is_print)
        {
            System.Text.StringBuilder st = new System.Text.StringBuilder();
            st.AppendLine("-------------------------");
            st.AppendLine("定时器使用情况:" + m_Timers.Count);
            if (is_print) Debuger.Log(st.ToString());
            return st.ToString();
        }

        #region tick
        /// <summary>
        /// 每帧调用
        /// </summary>
        public void AddTick(eTimerTickType type, System.Action handler)
        {
            if (handler == null) return;
            if (!this.m_DicTicks.ContainsKey(type))
            {
                this.m_DicTicks.Add(type, null);
            }
            this.m_DicTicks[type] = (Action)Delegate.Combine((Action)this.m_DicTicks[type], handler);
        }
        public void RemoveTick(eTimerTickType type, System.Action handler)
        {
            if (handler == null) return;
            if (!this.m_DicTicks.ContainsKey(type))
            {
                return;
            }
            this.m_DicTicks[type] = (Action)Delegate.Remove((Action)this.m_DicTicks[type], handler);
            if (this.m_DicTicks[type] == null) this.m_DicTicks.Remove(type);
        }
        private void ProcessTick(eTimerTickType type)
        {
            Delegate delegate2;
            if (this.m_DicTicks.TryGetValue(type, out delegate2))
            {
                if(delegate2 == null)
                {
                    m_DicTicks.Remove(type);
                    return;
                }
                Delegate[] invocationList = delegate2.GetInvocationList();
                if (invocationList == null) return;
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action action = invocationList[i] as Action;
                    if (action == null)
                    {
                        Debuger.LogError(string.Format("ProcessTick {0} error: types of parameters are not match.", type));
                    }
                    try
                    {
                        action();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError("委托调用出错:" + exception);
                    }
                }
            }
        }
        #endregion

        #region pools
        private TimerEntity Spawn()
        {
            TimerEntity newTimer = null;
            int poos_count = m_TimerPools.Count;
            if (poos_count > 0)
            {
                newTimer = m_TimerPools[poos_count - 1];
                m_TimerPools.RemoveAt(poos_count - 1);
            }
            if (newTimer == null)
            {
                newTimer = new TimerEntity();
            }
            return newTimer;
        }
        private void Despawn(TimerEntity timer)
        {
            if (timer == null) return;
            m_TimerPools.Add(timer);
        }
        #endregion
    }

    /// <summary>
    /// 定时器
    /// </summary>
    class TimerEntity
    {
        public uint id;
        public bool isActive;

        public float mRate;
        public int mTicks;
        public int mTicksElapsed;
        public float mLast;
        public Action mCallBack;
        public Action<object> mCallBack1;
        public object mInfo;

        public void Init(uint id_, float rate_, int ticks_, Action callback_)
        {
            id = id_;
            mRate = rate_ < 0 ? 0 : rate_;
            mTicks = ticks_ < 0 ? 0 : ticks_;
            mCallBack = callback_;
            mLast = 0;
            mTicksElapsed = 0;
            isActive = true;
        }

        public void Init(uint id_, float rate_, int ticks_, Action<object> callback_, object info)
        {
            id = id_;
            mRate = rate_ < 0 ? 0 : rate_;
            mTicks = ticks_ < 0 ? 0 : ticks_;
            mCallBack1 = callback_;
            mLast = 0;
            mTicksElapsed = 0;
            mInfo = info;
            isActive = true;
        }

        public void Update(float elapse)
        {
            mLast += elapse;

            if (isActive && mLast >= mRate)
            {
                try
                {
                    if (mInfo != null && mCallBack1 != null)
                        mCallBack1(mInfo);
                    else if(mCallBack != null)
                        mCallBack();
                }
                catch (System.Exception e)
                {
                    Debuger.LogException(e);
                }
                mLast = 0;
                mTicksElapsed++;

                if (mTicks > 0 && mTicksElapsed >= mTicks)
                {
                    Timer.instance.RemoveTimer(id);
                }
            }
        }
    }
}