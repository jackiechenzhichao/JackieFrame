using UnityEngine;
using System.Collections;

namespace fs
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T m_instance = null;

        public static T instance
        {
            get
            {
                if(!Utils.HasUnityScene())
                {
                    Debuger.LogError("没有场景的时候调用单例instance");
                    return null;
                }
                if (m_instance == null)
                {
                    try
                    {
                        T[] _instance = FindObjectsOfType<T>();
                        if (_instance != null)
                        {
                            if (_instance.Length == 1)
                            {
                                m_instance = _instance[0];
                            }
                            else if (_instance.Length > 1)
                            {
                                Debuger.LogError("存在多个单例:" + typeof(T).Name);
                                m_instance = _instance[0];
                            }
                        }

                        if (m_instance == null)
                        {
                            string instanceName = typeof(T).Name;
                            GameObject instanceGO = GameObject.Find(instanceName);
                            if (instanceGO == null)
                                instanceGO = new GameObject(instanceName);
                            m_instance = instanceGO.AddComponent<T>();
                        }
                    }
                    catch(System.Exception ex)
                    {
                        Debuger.LogError(ex.ToString());
                        m_instance = null;
                    }
                }
                return m_instance;
            }
        }

        protected virtual void Awake()
        {
            if (m_instance != null)
            {
                Debuger.LogWarning(string.Format("存在多个单例:{0}，移除旧的", typeof(T).Name));
                GameObject.Destroy(m_instance.GetComponent(typeof(T)));
            }
            m_instance = this as T;
        }
    }
    public abstract class DnotMonoSingleton<T> : MonoBehaviour where T : DnotMonoSingleton<T>
    {
        protected static T m_instance = null;

        public static T instance
        {
            get
            {
                if (m_instance == null)
                {
                    T[] _instance = FindObjectsOfType<T>();
                    if (_instance != null)
                    {
                        if (_instance.Length == 1)
                        {
                            m_instance = _instance[0];
                        }
                        else if (_instance.Length > 1)
                        {
                            Debuger.LogError("存在多个单例:" + typeof(T).Name);
                            m_instance = _instance[0];
                        }
                    }

                    if (m_instance == null)
                    {
                        string instanceName = typeof(T).Name;
                        GameObject instanceGO = GameObject.Find(instanceName);
                        if (instanceGO == null)
                            instanceGO = new GameObject(instanceName);
                        m_instance = instanceGO.AddComponent<T>();
                        DontDestroyOnLoad(instanceGO);
                    }
                }
                return m_instance;
            }
        }
        protected virtual void Awake()
        {
            if (typeof(T).Name != gameObject.name)
            {
                Debuger.LogError(string.Format("DnotMonoSingleton({0}) 不能直接挂到对象上", typeof(T).Name));
            }
        }
    }
}