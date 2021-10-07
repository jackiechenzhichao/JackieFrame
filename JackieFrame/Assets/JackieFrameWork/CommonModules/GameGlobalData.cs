
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：游戏数据
// 创建时间：2019-08-30 10:41:10
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fs
{
    /// <summary>
    /// 游戏数据
    /// </summary>
    public class GameGlobalData : ScriptableObject
    {
        /// <summary>
        /// 包名称
        /// </summary>
        public string PackageName;
        /// <summary>
        /// 发布时间(系统时间)
        /// </summary>
        public string BuildTime;
        /// <summary>
        /// 版本号
        /// </summary>
        public string BundleVersion;
        /// <summary>
        /// 编译版本号
        /// </summary>
        public uint BundleVersionCode;
        /// <summary>
        /// 编译选项
        /// </summary>
        public string BuildArgs = string.Empty;
        /// <summary>
        /// 验证包的唯一性
        /// </summary>
        public bool CheckPackageIdentifier = false;
        /// <summary>
        /// 防止游戏最小化
        /// </summary>
        public bool SafeGameMinimize = false;

        /// <summary>
        /// 接受鼠标/键盘消息
        /// </summary>
        public bool RecvMouseIO = true;

#if UNITY_EDITOR
        public static GameGlobalData Create()
        {
            string file_name = "GameGlobalData";
            string path = "/Resources/AssetData/";
            GameGlobalData asset = ScriptableObject.CreateInstance<GameGlobalData>();

            if (!System.IO.Directory.Exists(Application.dataPath + path))
                System.IO.Directory.CreateDirectory(Application.dataPath + path);

            AssetDatabase.CreateAsset(asset, "Assets" + path + file_name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
#endif
        public static GameGlobalData Get()
        {
            GameGlobalData res = Resources.Load<GameGlobalData>("AssetData/GameGlobalData");
            return res;
        }
    }
}
