using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace fs
{
    /// <summary>
    /// 通用标签组件 TabCell 父类
    /// </summary>
    public class UITabView_TabCell : UIViewBase
    {
        protected UIImageList m_ImgList;    //UI图片列表
        protected Text m_TxtTab;            //tabcell中的文本
        protected Button m_BtnTab;          //按钮
        protected CommonTabInfo m_data;     //通用tabcell数据信息，包括：名称、索引等
        protected Action<int> OnBtnHandler; //按钮回调事件
        
        void Awake()
        {
            m_ImgList = gameObject.GetChild("Img").GetComponent<UIImageList>();
            m_TxtTab = gameObject.GetChild("Txt").GetComponent<Text>();
            m_BtnTab = gameObject.GetComponent<Button>();
            ScrollRect parentRectScroll = gameObject.gameObject.GetComponentInParent<ScrollRect>();
            if (m_BtnTab == null && parentRectScroll != null)
            {
                Debuger.LogError("父对象有ScrollRect组件而当前TabCell缺少Button组件--当前cell控件名：" + gameObject.name);
            }
        }

        public override void Setup(params object[] info)
        {
            m_data = info[0] as CommonTabInfo;
            if(m_TxtTab!=null)
            m_TxtTab.text = m_data.name;
            OnSelect(false);
        }

        public void SetBtnHandler(Action<int> callback)
        {
            OnBtnHandler = callback;
        }

        void OnDestroy()
        {
            OnBtnHandler = null;
        }

        #region 可拓展方法
        /// <summary>
        /// 重写拓展OnSelect
        /// </summary>
        public virtual void OnSelect(bool b)
        {
            if (m_ImgList != null)
                m_ImgList.SetImage(b ? 1 : 0);

            if (!string.IsNullOrEmpty(m_data.select_color) && !string.IsNullOrEmpty(m_data.normal_color))

            if (m_TxtTab != null)
            {
                //m_TxtTab.color = Utility.HexToColor(b ? m_data.select_color : m_data.normal_color);
            }
        }
        #endregion

        #region 事件
        protected override void RegisterEvent()
        {
            if (m_BtnTab != null)
            {
                m_BtnTab.onClick.AddListener(OnBtnTab);
            }
            else
            {
                //UIHelper.AddEventListener(gameObject, eUIEventType.Click, OnBtnTab);
            }
        }

        protected override void UnRegisterEvent()
        {
            if (m_BtnTab != null)
            {
                m_BtnTab.onClick.RemoveListener(OnBtnTab);
            }
            else
            {
                //UIHelper.RemoveEventListener(gameObject, eUIEventType.Click, OnBtnTab);
            }
        }

        private void OnBtnTab()
        {
            if (OnBtnHandler != null)
                OnBtnHandler(m_data.index);
        }
        private void OnBtnTab(UIEventArgs args)
        {
            if (OnBtnHandler != null)
                OnBtnHandler(m_data.index);
        }
        #endregion
    }
}
