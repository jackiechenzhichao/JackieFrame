using System;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 通用标签组件控制脚本， 泛型指定tabCell预设的控制脚本 需要继承TabView_TabCell
    /// 指定的GameObject上可以挂载GridLayoutGroup 控制cell布局
    /// @author liyang
    /// @time 219-3-15
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UITabView<T> where T : UITabView_TabCell, new()
    {
        public Transform m_RootNode;        // tab根节点
        private string m_cellPath;          //tabcell的路径
        private List<UITabView_TabCell> m_TabCellViews = new List<UITabView_TabCell>();     
        public CommonTabInfo[] m_TabInfos;

        private int m_CurIndex = -1;
        /// <summary> 当前tab索引 </summary>
        public int CurIndex { get { return m_CurIndex; } set { m_CurIndex = value; } }

        public Action<int> OnChangeTabHandler;
        
        public UITabView(Transform rootNode, string cellPath)
        {
            this.m_RootNode = rootNode;
            this.m_cellPath = cellPath;
        }

        /// <summary>
        /// 设置tab数据
        /// </summary>
        /// <param name="tabInfos"></param>
        public void Setup(CommonTabInfo[] tabInfos, int curIndex = 0)
        {
            m_CurIndex = -1;
            m_TabInfos = tabInfos;
            for (int i = 0; i < tabInfos.Length; i++)
            {
                T t = null;
                if (i < m_TabCellViews.Count)
                    t = (T)m_TabCellViews[i];
                else
                {
                    t = UIViewBase.Show<T>(m_RootNode, m_cellPath);
                    m_TabCellViews.Add(t);
                }
                t.Setup(tabInfos[i]);
                t.SetBtnHandler(SetTab);
            }

            SetTab(curIndex);
        }

        public void SetTab(int index)
        {
            if (m_CurIndex == index) return;
            m_CurIndex = index;

            for (int i = 0; i < m_TabCellViews.Count; i++)
            {
                m_TabCellViews[i].OnSelect(m_TabInfos[i].index == index);
            }
            if (OnChangeTabHandler != null)
                OnChangeTabHandler(index);
        }

        public void Destroy()
        {
            for (int i = 0; i < m_TabCellViews.Count; i++)
            {
                GameObject.DestroyImmediate(m_TabCellViews[i].gameObject);
            }
            m_TabCellViews.Clear();
            m_RootNode = null;
            OnChangeTabHandler = null;
            m_TabInfos = null;
        }

        protected void OnEnable()
        {
            for (int i = 0; i < m_TabCellViews.Count; i++)
            {
                m_TabCellViews[i].SetVisible(true);
            }
        }

        protected void OnDisable()
        {
            for (int i = 0; i < m_TabCellViews.Count; i++)
            {
                m_TabCellViews[i].SetVisible(false);
            }
        }

        /// <summary>
        /// 切换显示
        /// </summary>
        public void SetVisible(bool b)
        {
            if (m_RootNode != null && m_RootNode.gameObject.activeSelf != b)
            {
                if (!b)
                    this.OnDisable();
                else
                    this.OnEnable();
                m_RootNode.gameObject.SetActive(b);
            }
        }
    }

    /// <summary>
    /// 通用tab数据
    /// </summary>
    public class CommonTabInfo
    {
        public int index;                               //tab的索引
        public string name;                             //tab的名称
        public string select_color = string.Empty;      //选中的颜色
        public string normal_color = string.Empty;      //正常的颜色

        #region 初始化tab信息
        public CommonTabInfo(int index, string name)
        {
            this.name = name;
            this.index = index;
        }
        public CommonTabInfo(int index, string name, string s_color, string n_color)
        {
            this.name = name;
            this.index = index;
            this.select_color = s_color;
            this.normal_color = n_color;
        }
        #endregion
    }
}
