
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：合并两张贴图
// 创建时间：2019-07-24 18:21:51
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace fs
{
	public class EditorCombineTexture : EditorWindow
    {
        private Texture2D BaseTexture;
        private Texture2D AlphaTexture;
        private float AlphaClip;

        [MenuItem("Tools/Texture/合并两张图")]
        static void CombineTexture()
        {
            EditorWindow.GetWindow(typeof(EditorCombineTexture));
        }
        void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("基础贴图", GUILayout.Width(80));
            BaseTexture = (Texture2D)EditorGUILayout.ObjectField(BaseTexture, typeof(Texture2D), true, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alpha贴图", GUILayout.Width(80));
            AlphaTexture = (Texture2D)EditorGUILayout.ObjectField(AlphaTexture, typeof(Texture2D), true, GUILayout.MinWidth(100f));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alpha Clip", GUILayout.Width(80));
            AlphaClip = EditorGUILayout.FloatField(AlphaClip);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            if (GUILayout.Button("修改") && BaseTexture != null && AlphaTexture != null)
            {
                this.Process();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void Process()
        {
            if(BaseTexture.width % AlphaTexture.width != 0 || BaseTexture.height % AlphaTexture.height != 0)
            {
                Debug.LogError("两张图的尺寸需要一样");
                return;
            }

            int w_scale = BaseTexture.width / AlphaTexture.width;
            int h_scale = BaseTexture.height / AlphaTexture.height;

            // 创建单独的纹理
            Texture2D tex = new Texture2D(BaseTexture.width, BaseTexture.height, TextureFormat.ARGB32, false);
            for(int x = 0; x < BaseTexture.width; ++x)
            {
                for(int y = 0; y < BaseTexture.height; ++y)
                {
                    Color base_color = BaseTexture.GetPixel(x, y);
                    Color alpha_color = AlphaTexture.GetPixel((int)(x / w_scale), (int)(y / h_scale));
                    base_color.a = alpha_color.r;
                    if (base_color.a < AlphaClip) base_color.a = 0;
                    tex.SetPixel(x, y, base_color);
                }
            }
            tex.Apply();

            // 写入成PNG文件
            string path = AssetDatabase.GetAssetPath(BaseTexture);
            string folder = Path.GetDirectoryName(path);
            string file_name = Path.GetFileNameWithoutExtension(path);
            string full_path = folder + "/" + file_name + "_combine.png";
            if (File.Exists(full_path)) File.Delete(full_path);
            File.WriteAllBytes(full_path, tex.EncodeToPNG());
        }
    }
}
