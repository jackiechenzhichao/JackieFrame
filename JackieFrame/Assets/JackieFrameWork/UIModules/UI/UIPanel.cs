using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;

namespace fs
{
    /// <summary>
    /// UI弹出窗口基类
    /// 锚点命名:TopLeft,TopCenter,TopRight,MidLeft,MidCenter,MidRight,BottomCenter,BottomLeft,BottomRight
    /// @author hannibal
    /// @time 2019-7-3
    /// </summary>
    public abstract class UIPanel : UIViewBase
    {
        /**界面ID*/
        protected string m_uid;
        /**多屏显示时的id*/
        protected uint m_PlayerId = 0;
        /**是否打开中*/
        protected bool m_IsOpen;
        /**排序用order*/
        protected int m_MaxSortingOrder;
        /**关闭回调*/
        protected Action m_CloseCallback = null;

        #region 基本方法
        protected override void OnEnable()
        {
            m_IsOpen = true;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            m_IsOpen = false;
            if (m_CloseCallback != null)
            {
                m_CloseCallback();
            }
            base.OnDisable();
        }

        /// <summary>
        /// 关闭回调
        /// </summary>
        public void OnClosed(Action on_close)
        {
            m_CloseCallback = on_close;
        }
        #endregion

        #region get/set
        public string uid
        {
            get { return m_uid; }
            set { m_uid = value; }
        }
        public uint PlayerId
        {
            get { return m_PlayerId; }
            set { m_PlayerId = value; }
        }
        public bool isOpen
        {
            get { return m_IsOpen; }
            set { m_IsOpen = value; }
        }
        public int MaxSortingOrder
        {
            get { return m_MaxSortingOrder; }
            set { m_MaxSortingOrder = value; }
        }
        #endregion
    }
}