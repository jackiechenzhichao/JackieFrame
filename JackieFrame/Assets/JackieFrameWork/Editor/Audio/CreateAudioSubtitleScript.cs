//=======================================================
// 作者：fs
// 描述：自动创建AudioSubtitleData脚本
// 创建时间：2019-07-10 11:05:41
//=======================================================
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Text;

namespace fs
{
    public class CreateAudioSubtitleScript
    {
        //会导致在Unity编辑器下右键文件夹比较慢，先注释
        //[MenuItem("Assets/自动创建AudioSubtitleAsset脚本", true)]
        //private static bool IsShowCreateScript()
        //{
        //    Object[] select_objs = Selection.objects;
        //    if (select_objs == null || select_objs.Length != 1)
        //        return false;
        //    Object[] all_objs = Selection.GetFiltered(typeof(AudioSubtitleData), SelectionMode.DeepAssets);
        //    if (all_objs == null || all_objs.Length == 0) return false;
        //    return true;
        //}
        [MenuItem("Assets/自动创建AudioSubtitleAsset脚本", false)]
        private static void CreateScript()
        {
            Object[] all_objs = Selection.GetFiltered(typeof(AudioSubtitleData), SelectionMode.DeepAssets);
            if (all_objs == null || all_objs.Length == 0) return;

            Object select_obj = null;
            for(int i = 0; i < all_objs.Length; ++i)
            {
                select_obj = all_objs[i];
                string path = AssetDatabase.GetAssetPath(select_obj);
                string file_name = Path.GetFileNameWithoutExtension(path);
                string folder = Path.GetDirectoryName(path);

                AudioSubtitleData data = select_obj as AudioSubtitleData;

                CreateScript(Application.dataPath + "/../" + folder, file_name, data);
            }
            AssetDatabase.Refresh();

            return;
        }
        private static void CreateScript(string folder, string name, AudioSubtitleData data)
        {
            StringBuilder sb = new StringBuilder();

            //生成头部
            sb.AppendLine(string.Format(@"
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace fs
{{
    public class {0}
    {{", name+"Name"));

            //生成字段
            for (int i = 0; i < data.audioLists.Count; i++)
            {
                var obj = data.audioLists[i];
                sb.AppendLine(string.Format(@"
        public static string {0} = ""{1}"";", obj.clipName, obj.clipName));
            }
            sb.AppendLine(@"
    }
}");
            File.WriteAllText(string.Format("{0}/{1}.cs", folder, name + "Name"), sb.ToString());
        }
    }
}
