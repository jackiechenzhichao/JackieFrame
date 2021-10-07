using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace fs
{
    /// <summary>
    /// 事件监听类
    /// </summary>
    public class UIGridEventListenter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public class UnityEvent<T0> : UnityEngine.Events.UnityEvent<T0>
        {

        }

        public UnityEvent<PointerEventData> mOnBeginDrag { get; private set; }
        public UnityEvent<PointerEventData> mOnEndDrag { get; private set; }
        public UnityEvent<PointerEventData> mOnDrag { get; private set; }

        private void Awake()
        {
            mOnBeginDrag = new UnityEvent<PointerEventData>();
            mOnEndDrag = new UnityEvent<PointerEventData>();
            mOnDrag = new UnityEvent<PointerEventData>();
        }

        #region 拖拽事件
        public void OnBeginDrag(PointerEventData eventData)
        {
            mOnBeginDrag.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            mOnEndDrag.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            mOnDrag.Invoke(eventData);
        }
        #endregion
    }
}