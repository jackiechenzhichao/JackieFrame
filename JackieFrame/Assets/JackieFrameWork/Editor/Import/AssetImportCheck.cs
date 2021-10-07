using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using UnityEngine.Profiling;

namespace fs
{
    /// <summary>
    /// 资源导入有效性审查
    /// </summary>
    public class AssetImportCheck : AssetPostprocessor
    {
        /// <summary>
        /// 声音
        /// </summary>
        void OnPostprocessAudio(AudioClip clip)
        {
            var path = assetImporter.assetPath;
            this.CheckAudioName(path);
            string full_path = Application.dataPath + "/" + path.Substring("Assets".Length);
            float mem = FileUtils.GetFileSize(full_path);//float mem = Profiler.GetRuntimeMemorySizeLong(clip) / 1024f / 1024f;
            //Debug.LogWarning(mem.ToString());
            if (mem > 10)
            {
                string msg = string.Format("{0}文件大小：{1}", path, mem);
                if (mem > 20)
                    Debug.LogError(msg);
                else
                    Debug.LogWarning(msg);
            }
        }
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                string ext = Path.GetExtension(path);
                if (AssetExtUtils.IsVideo(ext))
                {
                    string full_path = Application.dataPath + "/" + path.Substring("Assets".Length);
                    float mem = FileUtils.GetFileSize(full_path);
                    //Debug.LogWarning(mem.ToString());
                    if (mem > 20)
                    {
                        string msg = string.Format("{0}文件大小：{1}", path, mem);
                        if (mem > 50)
                            Debug.LogError(msg);
                        else
                            Debug.LogWarning(msg);
                    }
                }
            }
        }
        /// <summary>
        /// 音效命名的检测
        /// </summary>
        public void CheckAudioName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            string name = Path.GetFileNameWithoutExtension(path);
            char[] chars = name.ToCharArray();
            if(StringUtils.IsSpecialCharacter(chars[0]))
            {
                Debug.LogError("命名开头为特殊字符"+name);
            }
            for(int i = 0; i < chars.Length; i++)
            {
                if ((int)chars[i] == 32)
                {
                    Debug.LogError("文件名中存在空格" + name);
                    break;
                }
            }
        }
    }
}