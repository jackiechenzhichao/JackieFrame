
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：ui显示入口
// 1. 支持多屏显示
// 创建时间：2019-07-22 09:00:22
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
	public class UIShowManager : DnotMonoSingleton<UIShowManager>
    {
        private Dictionary<uint, UIManager> m_AllUIManager = new Dictionary<uint, UIManager>();

        #region 显示关闭
        /// <summary>
        /// 显示入口：内部需要执行资源加载
        /// </summary>
        public void Show<T>(uint playerId = 0, params object[] list) where T : UIPanel
        {
            this.GetUIManager(playerId).Show<T>(list);
        }
        public void Show(System.Type type, uint playerId = 0, params object[] list)
        {
            this.GetUIManager(playerId).Show(type, list);
        }

        public void Show<T>(System.Action<UIPanel> callback, uint playerId = 0, params object[] list) where T : UIPanel
        {
            this.GetUIManager(playerId).Show<T>(callback, list);
        }
        public void Show(System.Type type, System.Action<UIPanel> callback, uint playerId = 0, params object[] list)
        {
            this.GetUIManager(playerId).Show(type, callback, list);
        }

        /// <summary>
        /// 显示入口：不需要加载资源
        /// </summary>
        public void Show<T>(GameObject obj, uint playerId = 0, params object[] list) where T : UIPanel
        {
            this.GetUIManager(playerId).Show<T>(obj, list);
        }
        public void Show(System.Type type, GameObject obj, uint playerId = 0, params object[] list)
        {
            this.GetUIManager(playerId).Show(type, obj, list);
        }

        public void Show<T>(GameObject obj, System.Action<UIPanel> callback, uint playerId = 0, params object[] list) where T : UIPanel
        {
            this.GetUIManager(playerId).Show<T>(obj, callback, list);
        }
        public void Show(System.Type type, GameObject obj, System.Action<UIPanel> callback, uint playerId = 0, params object[] list)
        {
            this.GetUIManager(playerId).Show(type, obj, callback, list);
        }

        /// <summary>
        /// 关闭入口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="playerId">多人模式用</param>
        /// <returns></returns>
        public bool Close<T>(uint playerId = 0) where T : UIPanel
        {
            UIManager mgr = this.GetUIManager(playerId);
            if (mgr == null) return false;
            return mgr.Close<T>();
        }
        public bool Close(System.Type type, uint playerId = 0)
        {
            UIManager mgr = this.GetUIManager(playerId);
            if (mgr == null) return false;
            return mgr.Close(type);
        }

        /// <summary>
        /// 关闭所有面板
        /// </summary>
        public void CloseAll(uint playerId = 0, List<System.Type> exclude_list = null)
        {
            UIManager mgr = this.GetUIManager(playerId);
            if (mgr == null) return;
            mgr.CloseAll(exclude_list);
        }

        public void DestroyAll(uint playerId = 0)
        {
            UIManager mgr = this.GetUIManager(playerId);
            if (mgr == null) return;
            mgr.DestroyAll();
        }
        #endregion

        #region 查询
        public UIPanel GetPanel<T>(uint playerId = 0) where T : UIPanel
        {
            UIManager mgr = this.GetUIManager(playerId);
            if (mgr == null) return null;
            return mgr.Find(typeof(T));
        }
        public UIPanel GetPanel(System.Type type, uint playerId = 0)
        {
            UIManager mgr = this.GetUIManager(playerId);
            if (mgr == null) return null;
            return mgr.Find(type);
        }
        /// <summary>
        /// 是否显示中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public bool IsPanelOpen<T>(uint playerId = 0) where T : UIPanel
        {
            return IsPanelOpen(typeof(T), playerId);
        }
        public bool IsPanelOpen(System.Type type, uint playerId = 0)
        {
            UIManager mgr = this.GetUIManager(playerId);
            if (mgr == null) return false;

            UIPanel panel = mgr.Find(type);
            if (panel != null && panel.isOpen)
                return true;
            return false;
        }
        #endregion

        #region uimanager管理
        public void AddUIManager(uint playerId, UIManager mgr)
        {
            if(m_AllUIManager.ContainsKey(playerId))
            {
                Debuger.LogError("已经存在ui管理器:" + playerId);
                return;
            }
            m_AllUIManager.Add(playerId, mgr);
        }
        public void RemoveUIManager(uint playerId)
        {
            m_AllUIManager.Remove(playerId);
        }
        public UIManager GetUIManager(uint playerId = 0)
        {
            UIManager mgr = null;
            if (m_AllUIManager.TryGetValue(playerId, out mgr))
                return mgr;
            Debuger.LogWarning("未找到uimanager:" + playerId);
            return null;
        }
        #endregion
    }
}
