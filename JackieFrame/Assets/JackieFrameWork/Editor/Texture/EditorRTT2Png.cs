
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：rtt贴图转png
// 创建时间：2019-08-14 09:22:09
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace fs
{
    public class EditorRTT2Png
    {
        [MenuItem("Assets/Rtt转png", true, 1050)]
        static private bool Change_IsShow()
        {
            Object[] select_objs = Selection.objects;
            if (select_objs == null || select_objs.Length != 1)
                return false;
            
            Object obj = select_objs[0];
            return obj is RenderTexture;
        }
        [MenuItem("Assets/Rtt转png", false, 1050)]
        static private void Change()
        {
            Object[] select_objs = Selection.objects;
            if (select_objs == null || select_objs.Length != 1)
                return;
     
            Object obj = select_objs[0];
            RenderTexture rt = obj as RenderTexture;
            if (rt == null)
                return;

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            byte[] bytes = png.EncodeToPNG();

            string path = AssetDatabase.GetAssetPath(obj);
            string folder = Path.GetDirectoryName(path);
            string file_name = Path.GetFileNameWithoutExtension(path);
            string full_path = folder + "/" + file_name + ".png";
            using (FileStream file = File.Open(full_path, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(file))
                    writer.Write(bytes);
                file.Close();
            }
            Texture2D.DestroyImmediate(png);
            png = null;
            RenderTexture.active = prev;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
