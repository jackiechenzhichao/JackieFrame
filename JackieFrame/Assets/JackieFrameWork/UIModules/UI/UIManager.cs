using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace fs
{
    /// <summary>
    /// ui特性
    /// @author hannibal
    /// @time 2014-12-4
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class UIPanelAttribute : System.Attribute
    {
        /**代码文件*/
        public System.Type mScript;
        /// <summary>
        /// 资源路径：如果是多语言，用{}包起来
        /// </summary>
        public string Path = string.Empty;
        /// <summary>
        /// 层级
        /// </summary>
        public int Layer = (int)eUILayer.TOOLS;
        /// <summary>
        /// 隐藏销毁：如果销毁，同时也会释放资源
        /// </summary>
        public bool HideDestroy = false;
        /// <summary>
        /// 是否异步加载
        /// </summary>
        public bool Async = false;
        /// <summary>
        /// 是否预加载资源
        /// </summary>
        public bool PreLoad = true;

        /// <summary>
        /// 是否影响子canva的order
        /// </summary>
        public bool CanvasSortingOrder = true;
        /// <summary>
        /// 是否影响子特效的order
        /// </summary>
        public bool RenderSortingOrder = true;
    }

    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /**多屏显示时的id*/
        public uint MultiScreenId = 0;
        [Space(5)]
        public Canvas UICanvas = null;
        public Camera UICamera = null;
        public Transform RootLayer = null;
        public Transform[] Layers = new Transform[(int)eUILayer.MAX];
        
        /**加载信息*/
        private Dictionary<System.Type, UIPanelAttribute> m_DicLoaderInfo = new Dictionary<System.Type, UIPanelAttribute>();

        /**加载中*/
        private List<System.Type> m_ListLoading = new List<System.Type>();
        /**构建过的UI*/
        private Dictionary<System.Type, UIPanel> m_DicUIPanel = new Dictionary<System.Type, UIPanel>();
        /**应用是否退出*/
        private bool m_IsApplicationQuit = false;

        /// <summary>
        /// 初始化
        /// </summary>
        void Awake()
        {
            this.RegisterEvent();
            UIShowManager.instance.AddUIManager(MultiScreenId, this);
        }
        private void OnApplicationQuit()
        {
            m_IsApplicationQuit = true;
        }
        void OnDestroy()
        {
            if(!m_IsApplicationQuit)
            {
                this.UnRegisterEvent();
                this.RemoveAllLoaderInfo();
                m_ListLoading.Clear();
                this.DestroyAll();
                UIShowManager.instance.RemoveUIManager(MultiScreenId);
            }
        }
        public void DestroyAll()
        {
            foreach (var obj in m_DicUIPanel)
            {
                if (obj.Value != null && obj.Value.gameObject != null) GameObject.DestroyImmediate(obj.Value.gameObject);
            }
            m_DicUIPanel.Clear();
        }
        
        #region 显示+关闭
        /// <summary>
        /// 显示入口：内部需要执行资源加载
        /// </summary>
        public void Show<T>(params object[] list) where T : UIPanel
        {
            System.Type type = typeof(T);
            Show(type, list);
        }
        public void Show(System.Type type, params object[] list)
        {
            Debuger.Log("显示界面:" + type.Name);
            ShowFromFile(type, null, list);
        }

        public void Show<T>(System.Action<UIPanel> callback, params object[] list) where T : UIPanel
        {
            System.Type type = typeof(T);
            Show(type, callback, list);
        }
        public void Show(System.Type type, System.Action<UIPanel> callback, params object[] list)
        {
            Debuger.Log("显示界面:" + type.Name);
            ShowFromFile(type, callback, list);
        }

        /// <summary>
        /// 显示入口：不需要加载资源
        /// </summary>
        public void Show<T>(GameObject obj, params object[] list) where T : UIPanel
        {
            System.Type type = typeof(T);
            Show(type, obj, list);
        }
        public void Show(System.Type type, GameObject obj, params object[] list)
        {
            if (obj == null)
            {
                Debuger.LogError("UIManager::Show - 传入obj为null");
                return;
            }
            Debuger.Log("显示界面:" + type.Name);
            ShowFromObj(type, obj, null, list);
        }

        public void Show<T>(GameObject obj, System.Action<UIPanel> callback, params object[] list) where T : UIPanel
        {
            System.Type type = typeof(T);
            Show(type, obj, callback, list);
        }
        public void Show(System.Type type, GameObject obj, System.Action<UIPanel> callback, params object[] list)
        {
            if (obj == null)
            {
                Debuger.LogError("UIManager::Show - 传入obj为null");
                if (callback != null) callback(null);
                return;
            }
            Debuger.Log("显示界面:" + type.Name);
            ShowFromObj(type, obj, callback, list);
        }

        private void ShowFromFile(System.Type type, System.Action<UIPanel> callback, params object[] list)
        {
            //加载中
            if (m_ListLoading.Contains(type))
            {
                Debuger.LogWarning("执行加载中:" + type.Name);
                if (callback != null) callback(null);
                return;
            }

            //从缓存中查找
            UIPanel panel;
            if (m_DicUIPanel.TryGetValue(type, out panel))
            {
                if (panel != null)
                {//直接显示
                    panel.SetVisible(true);
                    panel.Setup(list);
                    if (callback != null) callback(panel);
                    return;
                }
                else
                {
                    m_DicUIPanel.Remove(type);
                }
            }

            //获取加载数据
            UIPanelAttribute loader_info = this.GetLoaderInfo(type);
            if (loader_info == null)
            {
                if (callback != null) callback(null);
                return;
            }

            //资源加载完成
            System.Action<Object> OnLoadComplete = delegate (Object res)
            {
                m_ListLoading.Remove(type);
                if (res == null)
                {
                    Debuger.LogError("UIManager::ShowFromFile - res loader error:" + loader_info.Path);
                    if (callback != null) callback(null);
                    return;
                }
                //构建
                GameObject obj = GameObject.Instantiate(res) as GameObject;
                this.ShowFromObj(type, obj, callback, list);
            };

            //加载资源
            m_ListLoading.Add(type);
            if(loader_info.Async)
            {//异步
                ResourceManager.instance.LoadResource<Object>(ReplaceLanguagePath(loader_info.Path), (Object res)=>
                {
                    OnLoadComplete(res);
                });
            }
            else
            {//同步
                Object res = ResourceManager.instance.LoadResource<Object>(ReplaceLanguagePath(loader_info.Path));
                OnLoadComplete(res);
            }
        }
        private void ShowFromObj(System.Type type, GameObject obj, System.Action<UIPanel> callback, params object[] list)
        {
            string guid = type.Name;
            
            //从缓存中查找
            UIPanel panel;
            if (m_DicUIPanel.TryGetValue(type, out panel))
            {
                if (panel != null)
                {
                    panel.SetVisible(true);
                    if (callback != null) callback(panel);
                    return;
                }
                else
                {
                    m_DicUIPanel.Remove(type);
                }
            }

            //获取加载数据
            UIPanelAttribute loader_info = this.GetLoaderInfo(type);
            if (loader_info == null)
            {
                if (callback != null) callback(null);
                return;
            }

            //layer
            Transform layer = this.GetLayer(loader_info.Layer);
            if (layer == null)
            {
                layer = this.RootLayer;
            }
            if (layer == null)
            {
                Debuger.LogError("UIManager::ShowFromObj - not set layer");
                if (callback != null) callback(null);
                return;
            }
            obj.transform.SetParent(layer, false);
            
            //赋予脚本
            panel = obj.GetComponent(loader_info.mScript) as UIPanel;
            if (panel == null) panel = obj.AddComponent(loader_info.mScript) as UIPanel;
            m_DicUIPanel.Add(type, panel);
            panel.uid = guid;
            panel.PlayerId = MultiScreenId;
            panel.ViewType = type;

            //设置层级
            this.ResetSortingOrder(type);

            //执行逻辑初始化
            panel.Setup(list);

            //加载结束回调
            EventDispatcher.TriggerEvent(UIEventID.SHOW_GUI, guid);
            if (callback != null) callback(panel);
        }

        /// <summary>
        /// 关闭入口
        /// </summary>
        public bool Close<T>() where T : UIPanel
        {
            System.Type type = typeof(T);
            return Close(type);
        }
        public bool Close(System.Type type)
        {
            Debuger.Log("关闭界面:" + type.Name);
            UIPanelAttribute loaderInfo;
            loaderInfo = this.GetLoaderInfo(type);
            if (loaderInfo == null) return false;
            
            UIPanel panel;
            if (!m_DicUIPanel.TryGetValue(type, out panel) || panel == null)
            {
                m_DicUIPanel.Remove(type);
                return false;
            }

            if (loaderInfo.HideDestroy)
            {
                m_DicUIPanel.Remove(type);
                GameObject.DestroyImmediate(panel.gameObject);
                return true;
            }
            else
            {
                panel.SetVisible(false);
                return false;
            }
        }
        /// <summary>
        /// 关闭所有面板
        /// </summary>
        public void CloseAll(List<System.Type> exclude_list)
        {
            List<UIPanel> list = new List<UIPanel>(m_DicUIPanel.Values);
            for(int i = list.Count - 1; i >= 0; --i)
            {
                UIPanel obj = list[i];
                if (exclude_list != null && exclude_list.Contains(obj.ViewType)) continue;
                Close(obj.ViewType);
            }
        }
        #endregion

        #region 集合
        public UIPanel Find(System.Type type)
        {
            UIPanel panel;
            if (m_DicUIPanel.TryGetValue(type, out panel))
            {
                return panel;
            }
            return null;
        }
        public int GetUILayerID(System.Type type)
        {
            UIPanelAttribute info = this.GetLoaderInfo(type);
            if (info == null)
            {
                return -1;
            }
            return info.Layer;
        }
        #endregion

        #region 事件
        private void RegisterEvent()
        {
        }
        private void UnRegisterEvent()
        {
        }
        #endregion

        #region 渲染层级
        /// <summary>
        /// 重设UI渲染层级
        /// </summary>
        /// <param name="id"></param>
        private void ResetSortingOrder(System.Type type)
        {
            string id = type.Name;

            UIPanel currView = this.Find(type);
            if (currView == null) return;
            GameObject obj = currView.gameObject;

            currView.MaxSortingOrder = 0;
            int UILayerID = this.GetUILayerID(type);
            GameObject layer = this.GetLayer(UILayerID).gameObject;
            int maxCanvasSortingOrder = GetLayerMaxSortingOrder(layer);
            Canvas currCanvas = GameObjectUtils.GetOrCreateComponent<Canvas>(obj);
            currCanvas.overrideSorting = true;
            if (maxCanvasSortingOrder != -1)
            {
                if (maxCanvasSortingOrder == 0)
                {
                    currCanvas.sortingOrder = UILayerID * UIID.OrderLyaerInterval;
                    currView.MaxSortingOrder = UILayerID * UIID.OrderLyaerInterval;
                }
                else
                {
                    currCanvas.sortingOrder = maxCanvasSortingOrder + 5;
                    currView.MaxSortingOrder = maxCanvasSortingOrder + 5;
                }
            }

            // 加入组件
            GameObjectUtils.GetOrCreateComponent<GraphicRaycaster>(obj);
            GameObjectUtils.GetOrCreateComponent<CanvasGroup>(obj);

            // 调整下级canvas层级
            UIPanelAttribute loader_info = this.GetLoaderInfo(type);
            if (loader_info != null && loader_info.Layer != (int)eUILayer.TOOLS)
            {
                if (loader_info.RenderSortingOrder) currView.RefreshRenderSortOrder(obj, currCanvas.sortingOrder);
                if (loader_info.CanvasSortingOrder) currView.RefreshCanvasSortOrder(obj, currCanvas.sortingOrder);
            }
        }

        /// <summary>
        /// 获取当前节点下的最大sortingOrder值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="UILayerID"></param>
        /// <returns></returns>
        public int GetLayerMaxSortingOrder(GameObject obj)
        {
            if (obj == null) return -1;
            int maxSortingOrder = 0;
            int childCounts = obj.transform.childCount;
            for (int i = 0; i < childCounts; ++i)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                UIPanel currView = child.GetComponent<UIPanel>();
                if (currView == null) continue;
                if (maxSortingOrder < currView.MaxSortingOrder)
                {
                    maxSortingOrder = currView.MaxSortingOrder;
                }
            }
            return maxSortingOrder;
        }
        #endregion

        #region 加载信息
        public void AddLoaderInfo(UIPanelAttribute info)
        {
            if (!m_DicLoaderInfo.ContainsKey(info.mScript))
            {
                m_DicLoaderInfo.Add(info.mScript, info);
            }
        }
        public UIPanelAttribute GetLoaderInfo(System.Type type)
        {
            UIPanelAttribute info = null;
            if(!m_DicLoaderInfo.TryGetValue(type, out info))
            {//可能没有提取特性
                this.ExtractAttribute(type.Assembly);
                if (!m_DicLoaderInfo.TryGetValue(type, out info))
                {
                    return null;
                }
            }
            return info;
        }
        public bool HasLoaderInfo<T>() where T : UIPanel
        {
            System.Type type = typeof(T);
            return HasLoaderInfo(type);
        }
        public bool HasLoaderInfo(System.Type type)
        {
            return m_DicLoaderInfo.ContainsKey(type);
        }
        public void RemoveAllLoaderInfo()
        {
            m_DicLoaderInfo.Clear();
        }
        /// <summary>
        /// 提取特性
        /// </summary>
        private void ExtractAttribute(System.Reflection.Assembly assembly)
        {
            float start_time = Time.realtimeSinceStartup;
            //外部程序集
            List<System.Type> types = AttributeUtils.FindType<UIPanel>(assembly, false);
            if (types != null)
            {
                foreach (System.Type type in types)
                {
                    UIPanelAttribute ui_attr = AttributeUtils.GetClassAttribute<UIPanelAttribute>(type);
                    if (ui_attr == null) continue;
                    ui_attr.mScript = type;
                    this.AddLoaderInfo(ui_attr);
                }
            }
            Debuger.Log("UIManager:ExtractAttribute 提取特性用时:" + (Time.realtimeSinceStartup - start_time));
        }
        /// <summary>
        /// 替换语言路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ReplaceLanguagePath(string path)
        {
            if (path.IndexOf("{lang}") < 0) return path;

            string str_lang = GlobalID.LanguageType.ToString();
            string new_path = path.Replace("{lang}", str_lang);
            return new_path;
        }
        #endregion

        #region 预加载资源
        /// <summary>
        /// 预加载ui预制体资源
        /// </summary>
        /// <param name="sync">是否同步方式(true:同步,false:异步)</param>
        /// <param name="callback">加载回调(参数1：当前加载数量；参数2：总共需要加载的数量)</param>
        public static void PreLoad(bool sync = true, System.Action<int, int> progress = null)
        {
            List<string> all_loaders = new List<string>();

            //反射需要加载的内容
            System.Reflection.Assembly assembly = ReflectionUtils.GetCSharpAssembly();
            List<System.Type> types = AttributeUtils.FindType<UIPanel>(assembly, false);
            if (types != null)
            {
                foreach (System.Type type in types)
                {
                    UIPanelAttribute ui_attr = AttributeUtils.GetClassAttribute<UIPanelAttribute>(type);
                    if (ui_attr == null) continue;
                    if(ui_attr.PreLoad)
                    {
                        all_loaders.Add(ReplaceLanguagePath(ui_attr.Path));
                    }
                }
            }

            //加载
            if(all_loaders.Count == 0)
            {
                if (progress != null) progress(0,0);
                return;
            }
            if (sync)
            {
                for (int i = 0; i < all_loaders.Count; ++i)
                {
                    ResourceManager.instance.LoadResource<Object>(all_loaders[i]);
                    if (progress != null) progress(i+1, all_loaders.Count);
                }
            }
            else
            {
                List<KeyValuePair<string, System.Type>> listRes = new List<KeyValuePair<string, System.Type>>();
                all_loaders.ForEach(path => listRes.Add(new KeyValuePair<string, System.Type>(path, typeof(Object))));
                ResourceManager.instance.LoadBatchResource(listRes, progress);
            }
        }
        #endregion

        #region Layer
        public Transform GetLayer(int index)
        {
            if (index < 0 || index >= Layers.Length) return null;
            return Layers[index];
        }
        /// <summary>
        /// 把节点添加到层
        /// </summary>
        /// <param name="layer_id"></param>
        /// <param name="node"></param>
        public void AddChildNode(int layer_id, Transform node)
        {
            Transform parent_layer = GetLayer(layer_id);
            if (parent_layer == null)
                parent_layer = RootLayer;
            if (parent_layer != null)
            {
                node.SetParent(parent_layer, false);
            }
        }
        #endregion
    }
}