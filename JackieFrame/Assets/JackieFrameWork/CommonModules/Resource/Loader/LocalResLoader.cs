using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

namespace fs
{
    /// <summary>
    /// Resource目录下资源加载
    /// @author hannibal
    /// @time 2014-10-22
    /// </summary>
    public class LocalResLoader : Singleton<LocalResLoader>
    {
#if UNITY_EDITOR
        private bool m_EnableLog = true;
#else
        private bool m_EnableLog = true;
#endif

        #region 加载
        /// <summary>
        /// 加载资源:同步方式
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">资源类型</param>
        /// <returns></returns>
        public Object Load(string path, System.Type type)
        {
            float time = Time.realtimeSinceStartup;
            Object obj = Resources.Load(path, type);
            if (obj == null) Debuger.LogError("[res]load failed：" + path);
            if (m_EnableLog) Debuger.Log("[load]load resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
            return obj;
        }
        public Object Load(string path)
        {
            float time = Time.realtimeSinceStartup;
            Object obj = Resources.Load(path);
            if (obj == null) Debuger.LogError("[res]load failed：" + path);
            if (m_EnableLog) Debuger.Log("[load]load resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
            return obj;
        }
        /// <summary>
        /// 加载资源:异步方式
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">资源类型</param>
        /// <param name="callback">加载完成回调</param>
        /// <returns></returns>
        public IEnumerator LoadAsync(string path, System.Type type, System.Action<Object> callback)
        {
            float time = Time.realtimeSinceStartup;
            ResourceRequest request = Resources.LoadAsync(path, type);
            request.completed += (delegate (AsyncOperation ao)
            {
                Object res = request.asset;
                if (res == null) Debuger.LogError("load res error, url: " + path);
                if (m_EnableLog) Debuger.Log("[load]load async resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
                if (callback != null) callback(res);
            });

            yield return null;
        }
        public IEnumerator LoadAsync(string path, System.Action<Object> callback)
        {
            float time = Time.realtimeSinceStartup;
            ResourceRequest request = Resources.LoadAsync(path);
            request.completed += (delegate (AsyncOperation ao)
            {
                Object res = request.asset;
                if (res == null)Debuger.LogError("load res error, url: " + path);
                if (m_EnableLog) Debuger.Log("[load]load async resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
                if (callback != null) callback(res);
            });

            yield return null;
        }
        /// <summary>
        /// 场景
        /// </summary>
        public void LoadScene(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            float time = Time.realtimeSinceStartup;
            UnityEngine.SceneManagement.SceneManager.LoadScene(path, mode);
            if (m_EnableLog) Debuger.Log("[load]load scene:" + path + " Time:" + (Time.realtimeSinceStartup - time));
        }
        public IEnumerator AsyncLoadScene(string path, System.Action<uint> progressCallback, System.Action completeCallback, LoadSceneMode mode = LoadSceneMode.Single)
        {
            float time = Time.realtimeSinceStartup;
            string sceneName = Path.GetFileName(path);
            AsyncOperation op = SceneManager.LoadSceneAsync(path, mode);//如果使用叠加模式，则灯光和雾效及天空盒会丢失
            uint progress = 0;
            while (op != null && !op.isDone)
            {
                progress = (uint)(op.progress * 100);
                if (progressCallback != null) progressCallback(progress);
                yield return null;
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            Debuger.Log(string.Format("场景构建完毕:{0}", Time.frameCount));
            if (completeCallback != null) completeCallback();
        }
        /// <summary>
        /// sprite
        /// </summary>
        public Sprite LoadSprite(string path)
        {
            float time = Time.realtimeSinceStartup;
            Sprite res = Resources.Load<Sprite>(path);
            if (res == null) Debuger.LogError("[res]load sprite failed:" + path);
            if (m_EnableLog) Debuger.Log("[load]load sprite:" + path + " Time:" + (Time.realtimeSinceStartup - time));
            return res;
        }
        public IEnumerator LoadAsyncSprite(string path, System.Action<Sprite> callback)
        {
            ResourceRequest request = Resources.LoadAsync<Sprite>(path);
            request.completed += (delegate (AsyncOperation ao)
            {
                Sprite res = request.asset as Sprite;
                if (res == null) Debuger.LogError("load res error, url: " + path);
                if (callback != null) callback(res);
            });
            yield return null;
        }
        /// <summary>
        /// 加载当前目录所有sprite
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Sprite[] LoadAllSprite(string path)
        {
            float time = Time.realtimeSinceStartup;
            Sprite[] res = Resources.LoadAll<Sprite>(path);
            if (m_EnableLog) Debuger.Log("[load]load resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
            return res;
        }
        /// <summary>
        /// 声音
        /// </summary>
        public AudioClip LoadSound(string path)
        {
            float time = Time.realtimeSinceStartup;
            AudioClip obj = Resources.Load<AudioClip>(path);
            if (obj == null) Debuger.LogError("[res]load sound failed：" + path);
            if (m_EnableLog) Debuger.Log("[load]load resource:" + path + " Time:" + (Time.realtimeSinceStartup - time));
            return obj;
        }
        public IEnumerator LoadAsyncSound(string path, System.Action<AudioClip> callback)
        {
            ResourceRequest request = Resources.LoadAsync<AudioClip>(path);
            request.completed += (delegate (AsyncOperation ao)
            {
                AudioClip res = request.asset as AudioClip;
                if (res == null) Debuger.LogError("load res error, url: " + path);
                if (callback != null) callback(res);
            });
            yield return null;
        }
#endregion

#region 释放
        public void UnloadAsset(Object obj)
        {
            Resources.UnloadAsset(obj);
        }
        /// <summary>
        /// 场景切换时需要调用，否则会存在无用的资源没有卸载干净
        /// </summary>
        public IEnumerator UnloadUnusedAssets(System.Action callback)
        {
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            while (!ao.isDone) yield return null;
            if (callback != null) callback();
        }
#endregion

        public bool EnableLog
        {
            set { m_EnableLog = value; }
        }
    }
}