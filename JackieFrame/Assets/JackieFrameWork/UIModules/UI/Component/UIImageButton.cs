using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fs
{
    /// <summary>
    /// UI图片按钮
    /// @author hannibal
    /// @time 2016-2-25
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UIImageButton : UIButton, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum Status
        {
            Normal,     //正常状态
            Select,     //选择状态
            Disable,    //禁用状态
        }
        public Status m_BtnStatus = Status.Normal;  //按钮的状态默认为正常
        public Sprite m_NormalBtn;                  //正常时的图片
        public Sprite m_SelectBtn;                  //选择时的图片
        public Sprite m_DisableBtn;                 //禁用时的图片
        public bool m_SetNativeSize = true;         //是否显示原始比例

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

        #region 鼠标点击事件
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!m_IsVisible || !m_IsValid) return;
            SetStatus(Status.Select);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!m_IsVisible || !m_IsValid) return;
            SetStatus(Status.Normal);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!m_IsVisible || !m_IsValid) return;
            SetStatus(Status.Normal);
        }
        #endregion

        /// <summary>
        /// 设置图片按钮的使用与禁用状态
        /// </summary>
        /// <param name="is_Show">是否可以使用</param>
        public override void SetVisible(bool is_Show)
        {
            m_IsVisible = is_Show;
            if (!m_IsVisible)
            {
                this.SetStatus(Status.Disable);
            }
            else
            {
                this.SetStatus(Status.Normal);
            }
        }

        /// <summary>
        /// 设置图片按钮的状态
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

                case Status.Disable:
                    if (m_DisableBtn != null)
                    {
                        ImgComponent.sprite = m_DisableBtn;
                        if (m_SetNativeSize) ImgComponent.SetNativeSize();
                    }
                    m_BtnStatus = status;
                    break;
            }
        }
    }

    
#if UNITY_EDITOR
    [CustomEditor(typeof(UIImageButton))]
    public class UIImageButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("提示：如果m_ImgComponent 为空，则m_ImgComponent为当前GameObject的Image", MessageType.Info);
        }
    }
#endif
}