using UnityEngine;
using UnityEngine.UI;
using System.Collections;  
using UnityEngine.EventSystems;  
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// ui事件
    /// @author hannibal
    /// @time 2014-10-22
    /// </summary>
    public sealed class UIEventListener
        : MonoBehaviour
        , IBeginDragHandler
        , IDragHandler
        , IEndDragHandler
        , IPointerClickHandler
        , IPointerDownHandler
        , IPointerUpHandler
        , IPointerEnterHandler
        , IPointerExitHandler
        , IUpdateSelectedHandler
        , ISelectHandler
        , IDeselectHandler
    {
        public delegate void EventDelegate(UIEventArgs args);
        private Dictionary<int, EventDelegate[]> m_UIEventHandleList = new Dictionary<int, EventDelegate[]>();

        static public UIEventListener Get(GameObject go)
        {
            if (go == null) return null;

            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null)
            {
                listener = go.AddComponent<UIEventListener>();
            }
            return listener;
        }

        #region 事件重载
        public void OnBeginDrag(PointerEventData eventData)
        {
            OnHandler(eUIEventType.BeginDrag, eventData);
        }
        public void OnDrag(PointerEventData eventData)
        {
            OnHandler(eUIEventType.Drag, eventData);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            OnHandler(eUIEventType.DragOut, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnHandler(eUIEventType.Click, eventData);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            OnHandler(eUIEventType.Down, eventData);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            OnHandler(eUIEventType.Up, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHandler(eUIEventType.Enter, eventData);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            OnHandler(eUIEventType.Exit, eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnHandler(eUIEventType.Select, eventData);
        }
        public void OnUpdateSelected(BaseEventData eventData)
        {
            OnHandler(eUIEventType.UpdateSelect, eventData);
        }
        public void OnDeselect(BaseEventData eventData)
        {
            OnHandler(eUIEventType.Deselect, eventData);
        }
        #endregion

        #region 事件监听
        /// <summary>
        /// 监听事件
        /// NOTE:如果Add时设置了优先级，移除时也需要设置相同优先级
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="callback">响应函数</param>
        /// <param name="priority">优先级，优先级越高越先响应</param>
        public void AddEventListener(eUIEventType type, EventDelegate callback, eUIEventPriority priority = eUIEventPriority.Normal)
        {
            Debug.Assert(priority >= eUIEventPriority.Normal && priority <= eUIEventPriority.High);
            EventDelegate[] queue = null;
            if(!this.m_UIEventHandleList.TryGetValue((int)type, out queue))
            {
                queue = new EventDelegate[(int)eUIEventPriority.High+1];
                this.m_UIEventHandleList[(int)type] = queue;
            }
            queue[(int)priority] += callback;
        }
        public void RemoveEventListener(eUIEventType type, EventDelegate callback, eUIEventPriority priority = eUIEventPriority.Normal)
        {
            Debug.Assert(priority >= eUIEventPriority.Normal && priority <= eUIEventPriority.High);
            EventDelegate[] queue = null;
            if (!this.m_UIEventHandleList.TryGetValue((int)type, out queue))
            {
                return;
            }
            queue[(int)priority] -= callback;
        }
        public void ClearEventListener(eUIEventType type)
        {
            this.m_UIEventHandleList[(int)type] = null;
        }
        public void ClearEventListener()
        {
            foreach(var obj in this.m_UIEventHandleList)
            {
                for(int i = 0; i < obj.Value.Length; ++i)
                {
                    obj.Value[i] = null;
                }
            }
        }
        private void OnHandler(eUIEventType type, BaseEventData eventData)
        {
            if (!Interactable) return;

            EventDelegate[] queue = null;
            if (!this.m_UIEventHandleList.TryGetValue((int)type, out queue))
            {
                return;
            }
            UIEventArgs args = new UIEventArgs();
            args.type = type;
            args.target = gameObject;
            args.data = eventData;
            for (eUIEventPriority priority = eUIEventPriority.High; priority >= eUIEventPriority.Normal; --priority)
            {
                if (queue[(int)priority] != null) queue[(int)priority](args);
            }
        }
        #endregion

        private bool Interactable
        {
            get
            {
                var selectable = gameObject.GetComponent<Selectable>();
                return selectable == null ? true : selectable.interactable;
            }
        }
    }

    /// <summary>
    /// 事件参数
    /// </summary>
    public struct UIEventArgs
    {
        public eUIEventType type;
        public GameObject target;
        public BaseEventData data;
    }

    /// <summary>
    /// ui事件类型
    /// </summary>
    public enum eUIEventType
    {
        Click = 1,
        Down,
        Up,
        Enter,
        Exit,
        BeginDrag,
        Drag,
        DragOut,
        Select,
        UpdateSelect,
        Deselect,

        Max,
    }
    /// <summary>
    /// 事件优先级
    /// </summary>
    public enum eUIEventPriority
    {
        Normal = 0,
        Above,
        High,
    }
}