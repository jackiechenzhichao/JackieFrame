using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JsonFx.Json;

namespace fs
{
    public class EditorCustomFont : ScriptableWizard
    {
        class sConfigInfo
        {
            public string content;//内容
            public int spacing;//间距
            public string adjust_width;
        }

        [MenuItem("字体/生成字体")]
        public static void GetAllFileNameInOneDir()
        {
            EditorWindow window = ScriptableWizard.DisplayWizard("自适应字体文件", typeof(EditorCustomFont), "关闭", "设置");
            window.position = EditorUtils.GetWindowRect(100, 100, 600, 550);
        }

        [Header("配置文件")]
        public TextAsset 配置文件;

        [Header("字体参数")]
        public Font 主体;
        public Material 材质;
        public Texture 图片;

        [Header("图片参数")]
        public string 内容;
        public int 间距;
        /// <summary>
        /// 在原有宽度基础上做调整，比如原来单个字符宽度是50，设置为-10的话，真实宽度就是40；如果设置10的话，真实宽度就是60
        /// 默认值或不填都是0
        /// </summary>
        public int[] 宽度调整;
        public string 宽度调整说明;

        private void OnEnable()
        {
            Object[] obj = Selection.objects;

            for (int i = 0; i < obj.Length; i++)
            {
                if (obj[i] is Font)
                {
                    主体 = (Font)obj[i];
                }
                if (obj[i] is Material)
                {
                    材质 = (Material)obj[i];
                }
                if (obj[i] is Texture)
                {
                    图片 = (Texture)obj[i];
                }
                if (obj[i] is TextAsset)
                {
                    配置文件 = (TextAsset)obj[i];
                    this.LoadConfig();
                }
            }

            string path = string.Empty;
            if (string.IsNullOrEmpty(path) && 主体 != null) path = AssetDatabase.GetAssetPath(主体);
            if (string.IsNullOrEmpty(path) && 材质 != null) path = AssetDatabase.GetAssetPath(材质);
            if (string.IsNullOrEmpty(path) && 图片 != null) path = AssetDatabase.GetAssetPath(图片);
            if (string.IsNullOrEmpty(path)) return;
            string folder = System.IO.Path.GetDirectoryName(path);
            string file_name = System.IO.Path.GetFileNameWithoutExtension(path);

            //如果没有选择配置表文件，加载同目录下的配置表文件；如果没有，则创建新的
            if (配置文件 == null)
            {
                string full_path = folder + "/" + file_name + ".json";
                配置文件 = AssetDatabase.LoadAssetAtPath<TextAsset>(full_path);
                if (配置文件 == null)
                {
                    string full_file_folder = Application.dataPath + "/../" + full_path;
                    System.IO.File.WriteAllText(full_file_folder, "", System.Text.Encoding.UTF8);
                    AssetDatabase.Refresh();
                    配置文件 = AssetDatabase.LoadAssetAtPath<TextAsset>(full_path);
                }
                this.LoadConfig();
            }
            if (主体 == null)
            {
                string full_path = folder + "/" + file_name + ".fontsettings";
                主体 = AssetDatabase.LoadAssetAtPath<Font>(full_path);
                if (主体 == null)
                {
                    AssetDatabase.CreateAsset(new Font(), full_path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    主体 = AssetDatabase.LoadAssetAtPath<Font>(full_path);
                }
            }
            if (材质 == null)
            {
                string full_path = folder + "/" + file_name + ".mat";
                材质 = AssetDatabase.LoadAssetAtPath<Material>(full_path);
                if (材质 == null)
                {
                    AssetDatabase.CreateAsset(new Material(Shader.Find("Standard")), full_path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    材质 = AssetDatabase.LoadAssetAtPath<Material>(full_path);
                }
            }
            if (图片 == null)
            {
                string full_path = folder + "/" + file_name + ".png";
                图片 = AssetDatabase.LoadAssetAtPath<Texture>(full_path);
            }

            宽度调整说明 = "内容是多少个字符,数组就设置多大;数组索引对应内容的第几个;数值是在原有宽度上做修正";
        }

        private void OnWizardOtherButton()
        {
            主体.material = 材质;
            材质.mainTexture = 图片;

            int num = 内容.Length;
            int fontWidth = 图片.width / num;
            int fontHeight = 图片.height;
            float uvWidth = 1f / num;
            List<CharacterInfo> charInfoList = new List<CharacterInfo>();
            for (int i = 0; i < num; i++)
            {
                int adjust_width = 0;//宽度修正值
                float adjust_uv = 0;//不调整uv会导致图片横向拉伸
                if (宽度调整.Length > i)
                {
                    adjust_width = 宽度调整[i];
                    adjust_uv = (adjust_width * 0.5f) / 图片.width;
                }

                CharacterInfo charInfo = new CharacterInfo();
                charInfo.index = (int)内容[i];

                charInfo.uvBottomLeft = new Vector2(uvWidth * i - adjust_uv, 0);
                charInfo.uvBottomRight = new Vector2(uvWidth * i + uvWidth + adjust_uv, 0);
                charInfo.uvTopLeft = new Vector2(uvWidth * i - adjust_uv, 1);
                charInfo.uvTopRight = new Vector2(uvWidth * i + uvWidth + adjust_uv, 1);

                charInfo.minX = 0;
                charInfo.maxX = fontWidth + adjust_width;
                charInfo.minY = 0;
                charInfo.maxY = fontHeight;

                charInfo.advance = fontWidth + 间距 + adjust_width;

                charInfoList.Add(charInfo);
            }

            主体.characterInfo = charInfoList.ToArray();
            this.SaveConfig();

            EditorUtility.SetDirty(主体);//设置变更过的资源  
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        #region 配置表
        /// <summary>
        /// 读取配置信息
        /// </summary>
        private void LoadConfig()
        {
            if (配置文件 == null) return;
            Debug.Log(配置文件.text);
            if (!string.IsNullOrEmpty(配置文件.text))
            {
                sConfigInfo config = JsonReader.Deserialize<sConfigInfo>(配置文件.text);
                内容 = config.content;
                间距 = config.spacing;
                if (!string.IsNullOrEmpty(config.adjust_width))
                    宽度调整 = StringUtils.Split<int>(config.adjust_width, ',').ToArray();
                else
                    宽度调整 = new int[内容.Length];
            }
        }
        /// <summary>
        /// 保存配置数据
        /// </summary>
        private void SaveConfig()
        {
            string full_path = full_path = Application.dataPath + "/../" + AssetDatabase.GetAssetPath(配置文件);
            if (System.IO.File.Exists(full_path))
                System.IO.File.Delete(full_path);

            string adjust_width = "";
            for (int i = 0; i < 宽度调整.Length; ++i)
            {
                if (i != 0) adjust_width += ",";
                adjust_width += 宽度调整[i];
            }
            sConfigInfo config = new sConfigInfo();
            config.content = 内容;
            config.spacing = 间距;
            config.adjust_width = adjust_width;
            string text = JsonWriter.Serialize(config);
            System.IO.File.WriteAllText(full_path, text, System.Text.Encoding.UTF8);
        }
        #endregion
    }
}