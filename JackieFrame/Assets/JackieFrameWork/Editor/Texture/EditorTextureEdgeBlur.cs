
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：边缘模糊
// 创建时间：2019-07-24 18:21:51
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace fs
{
	public class EditorTextureEdgeBlur : EditorWindow
    {
        private int BlurRange = 2;
        private float AlphaThreshold = 1;

        [MenuItem("Tools/Texture/边缘模糊")]
        static void AlphaBlur()
        {
            EditorWindow.GetWindow(typeof(EditorTextureEdgeBlur));
        }
        void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("模糊范围", GUILayout.Width(80));
            BlurRange = (int)GUILayout.HorizontalSlider((float)BlurRange, 1, 5);
            GUILayout.Label(BlurRange.ToString());
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Alpha阀值", GUILayout.Width(80));
            AlphaThreshold = EditorGUILayout.FloatField(AlphaThreshold);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            if (GUILayout.Button("执行"))
            {
                this.Process();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void Process()
        {
            Object[] select_objs = Selection.objects;
            if (select_objs == null || select_objs.Length == 0)
            {
                Debug.Log("没有选择");
                return;
            }
            foreach(var obj in select_objs)
            {
                string select_path = AssetDatabase.GetAssetPath(obj);
                BlurOneTexture(select_path);
            }
        }

        void BlurOneTexture(string select_path)
        {
            //设置为可读写模式
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(select_path);
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            bool is_readable = ti.isReadable;
            if(!is_readable)
            {
                ti.isReadable = true;
                AssetDatabase.ImportAsset(select_path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(select_path);
            Texture2D new_tex = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
            for (int x = 0; x < tex.width; ++x)
            {
                for (int y = 0; y < tex.height; ++y)
                {
                    Color color = tex.GetPixel(x, y);
                    if(color.a >= AlphaThreshold)
                    {
                        new_tex.SetPixel(x, y, color);
                    }
                    else
                    {
                        int count = 0;
                        Color sum_color = new Color(0,0,0,0);
                        for (int i = x - BlurRange; i <= x + BlurRange; ++i)
                        {
                            for (int j = y - BlurRange; j <= y + BlurRange; ++j)
                            {
                                if (i < 0 || j < 0 || i >= tex.width || j >= tex.height) continue;

                                int offset_i = Mathf.Abs(i - x);
                                int offset_j = Mathf.Abs(j - y);

                                int max_offset = Mathf.Max(offset_i, offset_j);
                                int weight = BlurRange + 1 - max_offset;

                                count += weight;
                                sum_color += tex.GetPixel(i, j) * weight;
                            }
                        }

                        color = sum_color / count;
                        new_tex.SetPixel(x, y, color);
                    }
                }
            }
            new_tex.Apply();
            File.WriteAllBytes(select_path, new_tex.EncodeToPNG());

            //还原读写模式
            ti.isReadable = is_readable;
            AssetDatabase.ImportAsset(select_path);
        }
    }
}
