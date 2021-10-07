using System;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 由EventDispatcher持有，请不用直接使用此类。
    /// </summary>
    public class EventController
    {
        //永久性的消息，在Cleanup的时候，这些消息的响应是不会被清理的。 
        private List<string> m_permanentEvents = new List<string>();
        private Dictionary<string, Delegate> m_theRouter = new Dictionary<string, Delegate>();

        #region 注册参数类型不同的消息调用
        public void AddEventListener<T>(string eventType, Action<T> handler)
        {
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType] = (Action<T>) Delegate.Combine((Action<T>) this.m_theRouter[eventType], handler);
        }

        public void AddEventListener(string eventType, Action handler)
        {
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType] = (Action) Delegate.Combine((Action) this.m_theRouter[eventType], handler);
        }

        public void AddEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType] = (Action<T, U>) Delegate.Combine((Action<T, U>) this.m_theRouter[eventType], handler);
        }

        public void AddEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType] = (Action<T, U, V>) Delegate.Combine((Action<T, U, V>) this.m_theRouter[eventType], handler);
        }

        public void AddEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            this.OnListenerAdding(eventType, handler);
            this.m_theRouter[eventType] = (Action<T, U, V, W>) Delegate.Combine((Action<T, U, V, W>) this.m_theRouter[eventType], handler);
        }
        #endregion

        /// <summary>
        /// 清空事件列表中的所有事件
        /// </summary>
        public void Cleanup()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, Delegate> pair in this.m_theRouter)
            {
                bool flag = false;
                foreach (string str in this.m_permanentEvents)
                {
                    if (pair.Key == str)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list.Add(pair.Key);
                }
            }
            foreach (string str in list)
            {
                this.m_theRouter.Remove(str);
            }
        }
        
        /// <summary>
        /// 判断是否存在该事件
        /// </summary>
        /// <param name="eventType">事件的名称</param>
        /// <returns></returns>
        public bool ContainsEvent(string eventType)
        {
            return this.m_theRouter.ContainsKey(eventType);
        }

        /// <summary>
        /// 添加永久性的消息，在Cleanup的时候，这些消息的响应是不会被清理的。
        /// </summary>
        /// <param name="eventType">事件名称</param>
        public void MarkAsPermanent(string eventType)
        {
            this.m_permanentEvents.Add(eventType);
        }

        /// <summary>
        /// 监听事件的添加是否异常
        /// </summary>
        /// <param name="eventType">事件的名称</param>
        /// <param name="listenerBeingAdded">事件的类型</param>
        private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!this.m_theRouter.ContainsKey(eventType))
            {
                this.m_theRouter.Add(eventType, null);
            }
            Delegate delegate2 = this.m_theRouter[eventType];
            if ((delegate2 != null) && (delegate2.GetType() != listenerBeingAdded.GetType()))
            {
                throw new EventException(string.Format("Try to add not correct event {0}. Current type is {1}, adding type is {2}.", eventType, delegate2.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="eventType">事件的名称</param>
        private void OnListenerRemoved(string eventType)
        {
            if (this.m_theRouter.ContainsKey(eventType) && (this.m_theRouter[eventType] == null))
            {
                this.m_theRouter.Remove(eventType);
            }
        }

        /// <summary>
        /// 监听事件的移除是否异常
        /// </summary>
        /// <param name="eventType">事件的名称</param>
        /// <param name="listenerBeingAdded">事件的类型</param>
        private bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (!this.m_theRouter.ContainsKey(eventType))
            {
                return false;
            }
            Delegate delegate2 = this.m_theRouter[eventType];
            if ((delegate2 != null) && (delegate2.GetType() != listenerBeingRemoved.GetType()))
            {
                throw new EventException(string.Format("Remove listener {0}\" failed, Current type is {1}, adding type is {2}.", eventType, delegate2.GetType(), listenerBeingRemoved.GetType()));
            }
            return true;
        }

        #region 移除参数类型不同的消息调用
        public void RemoveEventListener<T>(string eventType, Action<T> handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType] = (Action<T>) Delegate.Remove((Action<T>) this.m_theRouter[eventType], handler);
                this.OnListenerRemoved(eventType);
            }
        }

        public void RemoveEventListener(string eventType, Action handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType] = (Action) Delegate.Remove((Action) this.m_theRouter[eventType], handler);
                this.OnListenerRemoved(eventType);
            }
        }

        public void RemoveEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType] = (Action<T, U>) Delegate.Remove((Action<T, U>) this.m_theRouter[eventType], handler);
                this.OnListenerRemoved(eventType);
            }
        }

        public void RemoveEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType] = (Action<T, U, V>) Delegate.Remove((Action<T, U, V>) this.m_theRouter[eventType], handler);
                this.OnListenerRemoved(eventType);
            }
        }

        public void RemoveEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            if (this.OnListenerRemoving(eventType, handler))
            {
                this.m_theRouter[eventType] = (Action<T, U, V, W>) Delegate.Remove((Action<T, U, V, W>) this.m_theRouter[eventType], handler);
                this.OnListenerRemoved(eventType);
            }
        }
        #endregion

        #region 触发不同参数的消息调用
        public void TriggerEvent(string eventType)
        {
            Delegate delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                Delegate[] invocationList = delegate2.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action action = invocationList[i] as Action;
                    if (action == null)
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                    }
                    try
                    {
                        action();
                    }
                    catch (Exception exception)
                    {
                        // Log output
                        Debug.LogError("委托调用出错:"+exception);
                    }
                }
            }
        }

        public void TriggerEvent<T>(string eventType, T arg1)
        {
            Delegate delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                Delegate[] invocationList = delegate2.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action<T> action = invocationList[i] as Action<T>;
                    if (action == null)
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                    }
                    try
                    {
                        action(arg1);
                    }
                    catch (Exception exception)
                    {
                        // Log output
                        Debug.LogError("委托调用出错:" + exception);
                    }
                }
            }
        }

        public void TriggerEvent<T, U>(string eventType, T arg1, U arg2)
        {
            Delegate delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                Delegate[] invocationList = delegate2.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action<T, U> action = invocationList[i] as Action<T, U>;
                    if (action == null)
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                    }
                    try
                    {
                        action(arg1, arg2);
                    }
                    catch (Exception exception)
                    {
                        // Log output
                        Debug.LogError("委托调用出错:" + exception);
                    }
                }
            }
        }

        public void TriggerEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            Delegate delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                Delegate[] invocationList = delegate2.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action<T, U, V> action = invocationList[i] as Action<T, U, V>;
                    if (action == null)
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                    }
                    try
                    {
                        action(arg1, arg2, arg3);
                    }
                    catch (Exception exception)
                    {
                        // Log output
                        Debug.LogError("委托调用出错:" + exception);
                    }
                }
            }
        }

        public void TriggerEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            Delegate delegate2;
            if (this.m_theRouter.TryGetValue(eventType, out delegate2))
            {
                Delegate[] invocationList = delegate2.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    Action<T, U, V, W> action = invocationList[i] as Action<T, U, V, W>;
                    if (action == null)
                    {
                        throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                    }
                    try
                    {
                        action(arg1, arg2, arg3, arg4);
                    }
                    catch (Exception exception)
                    {
                        // Log output
                        Debug.LogError("委托调用出错:" + exception);
                    }
                }
            }
        }
        #endregion

        public Dictionary<string, Delegate> TheRouter
        {
            get
            {
                return this.m_theRouter;
            }
        }
    }
}

