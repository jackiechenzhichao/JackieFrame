using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 非弹出子界面，一般是嵌入到UIPanel
    /// @author hannibal
    /// @time 2019-7-3
    /// </summary>
    public abstract class UIViewBase : MonoBehaviour
    {
        private UIManager m_UIManager = null;
        private System.Type m_Type = null;

        public static T Show<T>(GameObject obj) where T : UIViewBase
        {
            System.Type type = typeof(T);
            UIViewBase currView = obj.GetComponent(type) as UIViewBase;
            if (currView == null) currView = obj.AddComponent(type) as UIViewBase;
            return currView as T;
        }
        public static T Show<T>(Transform parent, string file) where T : UIViewBase
        {
            Object res = ResourceManager.instance.LoadResource<Object>(file);
            if(res == null)
            {
                Debuger.LogError("资源加载失败:" + file);
                return null;
            }
            GameObject obj = GameObject.Instantiate(res, parent, false) as GameObject;
            return Show<T>(obj);
        }

        #region 基本方法
        /// <summary>
        /// 初始化界面， 外部数据传入
        /// </summary>
        public abstract void Setup(params object[] info);

        protected virtual void OnEnable()
        {
            RegisterEvent();
        }
        protected virtual void OnDisable()
        {
            UnRegisterEvent();
        }
        /// <summary>
        /// 事件
        /// </summary>
        protected abstract void RegisterEvent();
        protected abstract void UnRegisterEvent();
        
        public virtual void SetVisible(bool is_Show)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(is_Show);
            }
        }

        public UIManager UIMgr
        {
            get
            {
                if(m_UIManager == null)m_UIManager = gameObject.GetComponentInParent<UIManager>();
                return m_UIManager;
            }
        }
        public System.Type ViewType
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        #endregion

        #region 层级
        /// <summary>
        /// 调整层级
        /// </summary>
        /// <param name="obj">需要调整的obj，会递归调整所有子节点的order</param>
        public bool RefreshSortOrder(GameObject obj)
        {
            UIPanel panel = obj.GetComponentInParent<UIPanel>();
            if (panel == null) return false;

            Canvas canvas = panel.GetComponent<Canvas>();
            if (canvas == null) return false;

            int sort_order = canvas.sortingOrder;
            this.RefreshRenderSortOrder(obj, sort_order);
            this.RefreshCanvasSortOrder(obj, sort_order);

            return true;
        }
        public void RefreshRenderSortOrder(GameObject obj, int order)
        {
            if (obj == null) return;

            List<Transform> child_list = new List<Transform>();
            int child_count = obj.transform.childCount;
            for (int j = 0; j < child_count; ++j)
                child_list.Add(obj.transform.GetChild(j));

            int i = 0;
            while (i < child_list.Count)
            {
                Transform child = child_list[i++];
                Renderer render = child.GetComponent<Renderer>();
                if(render != null)
                {//求1000的模，只是为了保留原有的order设置
                    render.sortingOrder = render.sortingOrder % UIID.OrderLyaerInterval + order;
                }

                for (int j = 0; j < child.childCount; j++)
                {
                    Transform child_node = child.GetChild(j);
                    child_list.Add(child_node);
                }
            }
        }

        public void RefreshCanvasSortOrder(GameObject obj, int order)
        {
            if (obj == null) return;

            List<Transform> child_list = new List<Transform>();
            int child_count = obj.transform.childCount;
            for (int j = 0; j < child_count; ++j)
                child_list.Add(obj.transform.GetChild(j));

            int i = 0;
            while (i < child_list.Count)
            {
                Transform child = child_list[i++];
                Canvas canva = child.GetComponent<Canvas>();
                if (canva != null && canva.overrideSorting)
                {//求1000的模，只是为了保留原有的order设置
                    canva.sortingOrder = canva.sortingOrder % UIID.OrderLyaerInterval + order;
                }

                for (int j = 0; j < child.childCount; j++)
                {
                    Transform child_node = child.GetChild(j);
                    child_list.Add(child_node);
                }
            }
        }
        #endregion
    }
}