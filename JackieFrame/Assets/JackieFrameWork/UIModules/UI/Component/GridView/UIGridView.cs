using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 对UGUIGridWrapContent 进行 针对于项目的封装
    /// </summary>
    public class UIGridView<T> where T : UIViewBase, new()
    {
        public UIGridWrapContent m_GridWrapContent;
        private string m_FilePath;

        public Action<int, T> m_DisplayAction;

        public UIGridWrapContentConfig m_Config;
        private Dictionary<GameObject, T> m_GoDic = new Dictionary<GameObject, T>();

        public UIGridView(UIGridWrapContent pGridWrapContent, string pFilePath)
        {
            m_GridWrapContent = pGridWrapContent;
            m_FilePath = pFilePath;

            m_Config = new UIGridWrapContentConfig();
            m_Config.mDisplayCellAction = OnDisplayCell;
            m_Config.mConcealCellAction = OnConcealCell;
            m_Config.mCreateFunc = OnCreateCell;
        }

        public void Setup()
        {
            m_GridWrapContent.Show(m_Config);
        }

        public void Destroy()
        {
            foreach (var tKv in m_GoDic)
            {
                T tInstance = tKv.Value;
                GameObject.DestroyImmediate(tInstance.gameObject);
            }
        }

        private GameObject OnCreateCell()
        {
            T tInstance = UIViewBase.Show<T>(m_GridWrapContent.transform, m_FilePath);

            GameObject tGo = tInstance.gameObject;
            m_GoDic.Add(tGo, tInstance);

            return tInstance.gameObject;
        }

        private void OnDisplayCell(int pDataIndex, GameObject pGO)
        {
            if (m_DisplayAction == null)
                return;

            T tInstance = m_GoDic[pGO];
            if (tInstance != null) tInstance.SetVisible(true);
            m_DisplayAction(pDataIndex, tInstance);
        }
        /// <summary>
        /// 隐藏
        /// </summary>
        private void OnConcealCell(int pDataIndex, GameObject pGO)
        {
            T tInstance = m_GoDic[pGO];
            if (tInstance != null) tInstance.SetVisible(false);
        }

        #region 辅助函数
        /// <summary>
        /// 根据数据索引获取 继承 UIViewBase 的组件
        /// </summary>
        /// <param name="pDataIindex"></param>
        /// <returns></returns>
        public void GetClassInstanceByDataIndex(int pDataIindex, Action<T> pReturnAction)
        {
            m_GridWrapContent.GetGoByDataIndex(pDataIindex, (tGo) =>
            {
                if (pReturnAction == null || tGo == null)
                    return;

                T tInstance = null;
                if (m_GoDic.ContainsKey(tGo))
                    tInstance = m_GoDic[tGo];

                pReturnAction(tInstance);
            });
        }
        #endregion

    }
}
