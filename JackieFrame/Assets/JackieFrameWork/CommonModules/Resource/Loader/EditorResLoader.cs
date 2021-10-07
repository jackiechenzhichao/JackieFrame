using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fs
{
    /// <summary>
    /// 非Resource目录下资源加载
    /// @author hannibal
    /// @time 2018-10-22
    /// </summary>
    public class EditorResLoader : ScriptableObject
    {
        public Object Load(string path)
        {
#if UNITY_EDITOR
            Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (asset != null)
            {
                return asset;
            }
            else
            {
                Debuger.LogError("无法找到资源:" + path);
                return null;
            }
#else
                return null;
#endif
        }
        public Object Load(string path, System.Type type)
        {
#if UNITY_EDITOR
            Object asset = AssetDatabase.LoadAssetAtPath(path, type);
            if (asset != null)
            {
                return asset;
            }
            else
            {
                Debuger.LogError("无法找到资源:" + path);
                return null;
            }
#else
                return null;
#endif
        }

        public void UnloadAsset(Object obj)
        {
        }
    }
}