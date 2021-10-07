using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace fs
{
    /// <summary>
    /// UI状态按钮
    /// @author hannibal
    /// @time 2016-2-25
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UISwitchButton : UIButton, IPointerClickHandler
    {
        public enum Status
        {
            Normal,     //正常状态
            Select,     //选中状态
        }
        public bool m_AutoSwitch = true;            //是否自动切换状态
        public Status m_BtnStatus = Status.Normal;  //图片按钮状态，默认为正常状态
        public Sprite m_NormalBtn;                  //正常状态的图片
        public Sprite m_SelectBtn;                  //选中时的图片
        public bool m_SetNativeSize = true;         //是否显示原始图片比例
        private Image m_ImgComponent = null;        //图片组件
        private Image ImgComponent
        {
            get
            {
                if (m_ImgComponent == null) m_ImgComponent = GetComponent<Image>();
                return m_ImgComponent;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (m_NormalBtn == null) Debuger.LogError("没有设置按钮基础状态");
            ImgComponent.raycastTarget = true;
        }

        protected override void OnEnable()
        {
            SetStatus(m_BtnStatus);
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 当鼠标点击时
        /// </summary>
        /// <param name="eventData">鼠标点击事件</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!m_AutoSwitch || !m_IsValid) return;
            switch (m_BtnStatus)
            {
                case Status.Normal:
                    SetStatus(Status.Select);
                    break;
                case Status.Select:
                    SetStatus(Status.Normal);
                    break;
            }
        }

        /// <summary>
        /// 设置选择按钮的状态
        /// </summary>
        /// <param name="status">状态类型</param>
        public void SetStatus(Status status)
        {
            switch (status)
            {
                case Status.Normal:
                    if (m_NormalBtn != null)
                    {
                        ImgComponent.sprite = m_NormalBtn;
                        if (m_SetNativeSize) ImgComponent.SetNativeSize();
                    }
                    m_BtnStatus = status;
                    break;
                case Status.Select:
                    if (m_SelectBtn != null)
                    {
                        ImgComponent.sprite = m_SelectBtn;
                        if (m_SetNativeSize) ImgComponent.SetNativeSize();
                    }
                    m_BtnStatus = status;
                    break;
            }
        }
    }
}