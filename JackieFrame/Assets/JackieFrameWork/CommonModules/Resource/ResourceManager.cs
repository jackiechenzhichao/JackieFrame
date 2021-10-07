using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace fs
{    
    /// <summary>
    /// 资源管理
    /// @author hannibal
    /// @time 2016-5-14
    /// </summary>
    public class ResourceManager : DnotMonoSingleton<ResourceManager>
    {
        public void Release(System.Action callback)
        {
            this.StartCoroutine(LocalResLoader.instance.UnloadUnusedAssets(callback));
        }

        public void UnloadAsset(Object obj)
        {
            if (obj == null) return;
            Resources.UnloadAsset(obj);
        }

        #region 加载
        public T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            System.Type type = typeof(T);
            if (type.ToString() == "UnityEngine.GameObject")
            {
                Debuger.LogError("预制体需要通过LoadGameObject加载:" + path);
                return null;
            }
            string full_path = path;
            try
            {
                Object res = LocalResLoader.instance.Load(full_path, type);
                return res as T;
            }
            catch(System.Exception e)
            {
                Debuger.LogException(e);
                return null;
            }
        }
        public void LoadResource<T>(string path, System.Action<T> callback) where T : UnityEngine.Object
        {
            System.Type type = typeof(T);
            if (type.ToString() == "UnityEngine.GameObject")
            {
                Debuger.LogError("预制体需要通过LoadGameObject加载:" + path);
                if (callback != null) callback(null);
                return;
            }
            string full_path = path;
            try
            {
                this.StartCoroutine
                (
                    LocalResLoader.instance.LoadAsync(full_path, type, (Object res) =>
                    {
                        if (callback != null) callback(res as T);
                    })
                );
            }
            catch (System.Exception e)
            {
                Debuger.LogException(e);
                if (callback != null) callback(null);
            }
        }
        public GameObject LoadGameObject(string path)
        {
            try
            {
                Object res = LocalResLoader.instance.Load(path);
                GameObject obj = GameObject.Instantiate(res) as GameObject;
                return obj;
            }
            catch (System.Exception e)
            {
                Debuger.LogException(e);
                return null;
            }
        }
        public GameObject LoadGameObject(string path, Vector3 position)
        {
            try
            {
                Object res = LocalResLoader.instance.Load(path);
                GameObject obj = GameObject.Instantiate(res, position, Quaternion.Euler(0,0,0)) as GameObject;
                return obj;
            }
            catch (System.Exception e)
            {
                Debuger.LogException(e);
                return null;
            }
        }
        public GameObject LoadGameObject(string path, Vector3 position, Quaternion rotation)
        {
            try
            {
                Object res = LocalResLoader.instance.Load(path);
            GameObject obj = GameObject.Instantiate(res, position, rotation) as GameObject;
            return obj;
            }
            catch (System.Exception e)
            {
                Debuger.LogException(e);
                return null;
            }
        }
        public GameObject LoadGameObject(string path, Vector3 position, Quaternion rotation, Transform parent)
        {
            try
            {
                Object res = LocalResLoader.instance.Load(path);
                GameObject obj = GameObject.Instantiate(res, position, rotation, parent) as GameObject;
                return obj;
            }
            catch (System.Exception e)
            {
                Debuger.LogException(e);
                return null;
            }
        }
        public GameObject LoadGameObject(string path, Transform parent)
        {
            try
            {
                Object res = LocalResLoader.instance.Load(path);
                GameObject obj = GameObject.Instantiate(res, parent, false) as GameObject;
                return obj;
            }
            catch (System.Exception e)
            {
                Debuger.LogException(e);
                return null;
            }
        }
        /// <summary>
        /// 批次加载
        /// </summary>
        /// <param name="listRes">需要加载的资源列表</param>
        /// <param name="progress">加载进度(已加载/总数)</param>
        public void LoadBatchResource(List<KeyValuePair<string, System.Type>> listRes, System.Action<int, int> progress)
        {
            if(listRes.Count == 0)
            {
                if (progress != null) progress(0, 0);
                return;
            }

            List<string> leaveRes = new List<string>();
            listRes.ForEach((KeyValuePair<string, System.Type> pair) => { leaveRes.Add(pair.Key); });

            foreach (var obj in listRes)
            {
                string full_path = obj.Key;
                try
                {
                    this.StartCoroutine
                    (
                        LocalResLoader.instance.LoadAsync(full_path, obj.Value, (Object res)=>
                        {
                            leaveRes.Remove(obj.Key);
                            if (progress != null) progress(listRes.Count - leaveRes.Count, listRes.Count);
                        })
                    );
                }
                catch (System.Exception e)
                {
                    Debuger.LogException(e);
                    if (progress != null) progress(listRes.Count - leaveRes.Count, listRes.Count);
                }
            }
        }
        #endregion

        #region 场景
        /// <summary>
        /// 加载场景
        /// TODO:上个场景如果加载中
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="progressCallback"></param>
        public void LoadScene(string scenePath, System.Action<uint> progressCallback, System.Action completeCallback
            , UnityEngine.SceneManagement.LoadSceneMode mode = UnityEngine.SceneManagement.LoadSceneMode.Single)
        {
            Debuger.Log("LoadScene:" + scenePath);
            string sceneName = Path.GetFileName(scenePath);
            this.StartCoroutine(AsynLoadScene(sceneName, mode, progressCallback, completeCallback));
        }
        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="scenePath"></param>
        public void UnLoadScene(string scenePath, System.Action callback)
        {
            Debuger.Log("UnLoadScene:" + scenePath);
            string name = Path.GetFileName(scenePath);
            this.StartCoroutine(AsyncUnloadScene(name, delegate ()
            {
                //ResourcesManager.instance.ReleaseAssetByName(scenePath);
                if (callback != null) callback();
            }));
        }
        /// <summary>
        /// 场景资源加载完成后，构建场景
        /// </summary>
        IEnumerator AsynLoadScene(string sceneName, UnityEngine.SceneManagement.LoadSceneMode mode, System.Action<uint> progressCallback, System.Action completeCallback)
        {
            float begin_time = Time.realtimeSinceStartup;
            Debuger.Log(string.Format("开始构建场景:{0}", sceneName));
            AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);//如果使用叠加模式，则灯光和雾效及天空盒会丢失
            uint progress = 0;
            while (op != null && !op.isDone)
            {
                progress = (uint)(op.progress * 100);
                if (progressCallback != null) progressCallback(progress);
                yield return null;
            }
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName));
            Debuger.Log(string.Format("场景构建完毕:{0}", Time.frameCount));
            if (completeCallback != null) completeCallback();
        }
        IEnumerator AsyncUnloadScene(string name, System.Action callback)
        {
            Debuger.Log(string.Format("开始卸载场景:{0}", name));
            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(name).IsValid())
            {
                //TODO:实际未卸载
                AsyncOperation op = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
                while (op != null && !op.isDone)
                {
                    yield return null;
                }
                if (callback != null) callback();
            }
            else
            {
                if (callback != null) callback();
            }
        }
        #endregion
    }
}
