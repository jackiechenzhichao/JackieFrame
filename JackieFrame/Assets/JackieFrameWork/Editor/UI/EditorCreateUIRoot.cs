
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：创建ui root
// 创建时间：2019-07-26 17:38:27
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace fs
{
	public class EditorCreateUIRoot : EditorWindow
    {
        private const string TemplatePath = "Assets/Plugins/Res/Prefabs/UIRoot.prefab";
        private int ScreenRows = 1;
        private int ScreenCols = 1;

        private int ScreenWidth = 1920;
        private int ScreenHeight = 1080;

        [MenuItem("UGUI/创建UIRoot", false, 200)]
        static void CreateUIRoot()
        {
            EditorWindow.GetWindow(typeof(EditorCreateUIRoot));
        }
        void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("行数", GUILayout.Width(80));
            ScreenRows = EditorGUILayout.IntField(ScreenRows);
            GUILayout.Label("列数", GUILayout.Width(80));
            ScreenCols = EditorGUILayout.IntField(ScreenCols);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("屏幕像素宽", GUILayout.Width(80));
            ScreenWidth = EditorGUILayout.IntField(ScreenWidth);
            GUILayout.Label("屏幕像素高", GUILayout.Width(80));
            ScreenHeight = EditorGUILayout.IntField(ScreenHeight);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            if (GUILayout.Button("创建"))
            {
                Process();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        void Process()
        {
            if (ScreenCols < 0 || ScreenRows < 0)
            {
                Debug.LogError(string.Format("设置的数量错误row:{0},col:{1}", ScreenRows, ScreenCols));
                return;
            }
            for (int row = 1; row <= ScreenRows; ++row)
            {
                for (int col = 1; col <= ScreenCols; ++col)
                {
                    this.CreateOne(row, col);
                }
            }
        }
        void CreateOne(int row, int col)
        {
            //ui root
            GameObject template = AssetDatabase.LoadAssetAtPath<GameObject>(TemplatePath);
            if(template == null)
            {
                Debug.LogError("没有资源模板:" + TemplatePath);
                return;
            }
            GameObject ui_root = GameObject.Instantiate(template);
            ui_root.name = this.GetUIRootName(row, col);

            //分辨率
            GameObject ui_canvas = ui_root.transform.Find("UICanvas").gameObject;
            ui_canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(ScreenWidth, ScreenHeight);

            //相机
            Camera ui_camera = ui_root.GetComponentInChildren<Camera>();
            ui_camera.rect = this.GetViewportRect(row, col);
        }
        /// <summary>
        /// 获取名称
        /// </summary>
        string GetUIRootName(int row, int col)
        {
            if (ScreenRows == 1 && ScreenCols == 1)
            {
                return "UIRoot";
            }
            else if(ScreenRows == 1 && ScreenCols > 1)
            {
                return "UIRoot" + col;
            }
            else if (ScreenRows > 1 && ScreenCols == 1)
            {
                return "UIRoot" + row;
            }
            else
            {
                return "UIRoot" + row + "_" + col;
            }
        }
        /// <summary>
        /// 视口
        /// </summary>
        Rect GetViewportRect(int row, int col)
        {
            Rect rect = new Rect();
            if (ScreenRows == 1 && ScreenCols == 1)
            {
                rect.x = rect.y = 0;
                rect.width = rect.height = 1;
            }
            else if (ScreenRows == 1 && ScreenCols > 1)
            {
                rect.x = (float)(col - 1) / ScreenCols;
                rect.y = 0;
                rect.width = 1.0f/ ScreenCols;
                rect.height = 1;
            }
            else if (ScreenRows > 1 && ScreenCols == 1)
            {
                rect.x = 0;
                rect.y = (float)(row - 1) / ScreenRows;
                rect.width = 1;
                rect.height = 1.0f / ScreenRows;
            }
            else
            {
                rect.x = (float)(col - 1) / ScreenCols;
                rect.y = (float)(row - 1) / ScreenRows;
                rect.width = 1.0f / ScreenCols;
                rect.height = 1.0f / ScreenRows;
            }
            return rect;
        }
    }
}
