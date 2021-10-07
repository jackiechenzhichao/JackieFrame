using System;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
    /// <summary>
    /// 下拉列表控件 列表项是一个tabView 所以泛型类型是TabView_TabCell 
    /// 显示列表的时候会在控件所在uipanelbase的layer下创建一个blocaker全屏遮罩 sortorder = uipanelbase.maxsortorder+1
    /// 下拉列表同时也会挂上canvas组件 sortorder = uipanelbase.maxsortorder+2
    /// @author liyang
    /// @time 2019-4-8
    /// </summary>
    public class UIDropdown<T>  where T : UITabView_TabCell, new()
    {
        // UI
        protected Image m_Btn;
        protected Text m_Txt;
        protected GameObject m_OptionRoot;

        // 指定参数
        protected Transform m_RootNode;
        private string m_OptionCellPath;
        private int m_ParentSortOrder;

        // tabview
        private UITabView<T> m_TabView;
        private CommonTabInfo[] m_TabInfos;
        private int m_CurIndex = -1;
        public int CurIndex { get { return m_CurIndex; } set { m_CurIndex = value; } }
        ///<summary> 选中选项事件 </summary>
        public Action<int> OnChangeOptionHandler;
        ///<summary> 展开列表事件 </summary>
        public Action<int> OnTabSpreadHandler;
        //打开，关闭
        public Action<bool> OnOpen;

        private GameObject m_Blocker; // 全屏遮罩
        private int m_ParentLayerId;
        private Transform m_ParentLayer;

        public UIDropdown(Transform rootNode, string cellPath)
        {
            this.m_RootNode = rootNode;
            this.m_OptionCellPath = cellPath;

            this.Awake();
            this.RegisterEvent();
        }

        protected virtual void Awake()
        {
            m_Btn = m_RootNode.GetChild("ImgBtn").GetComponent<Image>();
            m_Txt = m_RootNode.GetChild("Txt").GetComponent<Text>();
            m_OptionRoot = m_RootNode.GetChild("OptionRoot").gameObject;

            m_TabView = new UITabView<T>(m_RootNode.GetChild("Content"), m_OptionCellPath);
            m_TabView.OnChangeTabHandler = SetTab;
        }

        public void Setup(CommonTabInfo[] tabInfos, int parentLayerId, int curIndex = 0)
        {
            this.m_ParentLayerId = parentLayerId;

            UIManager ui_manager = m_RootNode.GetComponentInParent<UIManager>();
            if(ui_manager == null)
            {
                Debuger.LogError("需要显示在uimanager下");
            }
            this.m_ParentLayer = ui_manager.GetLayer(this.m_ParentLayerId);
            this.m_ParentSortOrder = ui_manager.GetLayerMaxSortingOrder(this.m_ParentLayer.gameObject);

            m_CurIndex = -1; //每次setup 重置CurIndex
            m_TabInfos = tabInfos;
            SetTab(curIndex);
            OnHide();
        }

        public void Destroy()
        {
            this.UnRegisterEvent();
            m_RootNode = null;
            OnChangeOptionHandler = null;
            if (m_TabView != null)
                m_TabView.Destroy();
        }

        public void SetTab(int index)
        {
            if (m_CurIndex == index) return;
            m_CurIndex = index;

            CommonTabInfo curTabInfo = null;
            for (int i = 0; i < m_TabInfos.Length ; i++)
            {
                if (m_TabInfos[i].index == m_CurIndex)
                {
                    curTabInfo = m_TabInfos[i];
                    break;
                }
            }

            if (curTabInfo != null)
                m_Txt.text = curTabInfo.name;

            if (OnChangeOptionHandler != null)
            {
                OnChangeOptionHandler(m_CurIndex);
            }
                
        }

        #region 事件
        
        protected virtual void RegisterEvent()
        {
            UIHelper.AddEventListener(m_Btn.gameObject, eUIEventType.Click, OnBtn);
        }

        protected virtual void UnRegisterEvent()
        {
            UIHelper.RemoveEventListener(m_Btn.gameObject, eUIEventType.Click, OnBtn);
        }

        private void OnBtn(UIEventArgs evt)
        {
            OnShow();
        }

        // 显示下拉选项
        protected virtual void OnShow()
        {
            m_OptionRoot.SetActive(true);
            m_TabView.SetVisible(true);
            m_TabView.Setup(m_TabInfos, m_CurIndex);

            m_Blocker = CreateBlocker(m_ParentSortOrder + 1);
            SetDropdownRootSortOrder(m_ParentSortOrder + 2);

            // 展开事件
            if (OnTabSpreadHandler != null)
                OnTabSpreadHandler(m_TabView.CurIndex);
            if (OnOpen != null)
                OnOpen(true);
        }

        // 隐藏下拉选项
        protected virtual void OnHide()
        {
            m_OptionRoot.SetActive(false);
            m_TabView.SetVisible(false);
            if (m_Blocker != null)
                GameObject.DestroyImmediate(m_Blocker);
            if (OnOpen != null)
                OnOpen(false);
        }

        #endregion

        // 设置 下拉列表的SortOrder 比 blocker大1
        private void SetDropdownRootSortOrder(int sortOrder)
        {
            // 加入组件
            Canvas dropdownCanvas = m_OptionRoot.GetOrCreateComponent<Canvas>();
            m_OptionRoot.GetOrCreateComponent<GraphicRaycaster>();
            m_OptionRoot.GetOrCreateComponent<CanvasGroup>();

            // 调整canvas层级
            dropdownCanvas.overrideSorting = true;
            dropdownCanvas.sortingOrder = sortOrder;
        }

        // 创建一个全屏遮盖button 挂载在Layer下面
        private GameObject CreateBlocker(int sortOrder)
        {
            // 创建一个GameObject
            GameObject blocker = new GameObject("DropdownBlocker");
            // 设置父对象   
            RectTransform blockerRect = blocker.AddComponent<RectTransform>();
            blockerRect.SetParent(m_ParentLayer, false);
            // 设置尺寸 锚点
            blockerRect.anchorMin = Vector3.zero;
            blockerRect.anchorMax = Vector3.one;
            blockerRect.sizeDelta = Vector2.zero;

            Image blockerImage = blocker.AddComponent<Image>();
            blockerImage.color = Color.clear;

            Button blockerButton = blocker.AddComponent<Button>();
            blockerButton.onClick.AddListener(OnHide);

            // 加入组件
            Canvas blockerCanvas = GameObjectUtils.GetOrCreateComponent<Canvas>(blocker);
            GameObjectUtils.GetOrCreateComponent<GraphicRaycaster>(blocker);
            GameObjectUtils.GetOrCreateComponent<CanvasGroup>(blocker);

            // 调整canvas层级
            blockerCanvas.overrideSorting = true;
            blockerCanvas.sortingOrder = sortOrder;

            return blocker;
        }

        public void Enable(bool value)
        {
            this.m_Btn.raycastTarget = value;
        }
    }
}
