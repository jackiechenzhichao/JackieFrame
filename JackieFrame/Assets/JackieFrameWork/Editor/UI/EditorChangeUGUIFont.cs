using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 更换字体
    /// 操作方式：
    /// 1.在弹出的设置面板，设置目标字体
    /// 2.在Project视图，选择ui预制体，点面板的"修改"按钮
    /// @author hannibal
    /// @time 2016-11-5
    /// </summary>
    public class EditorChangeUGUIFont : EditorWindow
    {
        Font toChange;
        Font toChangeFont;

        [MenuItem("Tools/换字体")]
        static void ChangeFont()
        {
            EditorWindow.GetWindow(typeof(EditorChangeUGUIFont));
        }

        //[MenuItem("Assets/换字体样式")]
        //static void ChangeFontStyle()
        //{
        //    Object[] labels = Selection.GetFiltered(typeof(Text), SelectionMode.Deep);
        //    foreach (Object item in labels)
        //    {
        //        Text label = (Text)item;
        //        label.resizeTextForBestFit = true;
        //        label.resizeTextMaxSize = label.fontSize;
        //    }

        //    Save();
        //    Debug.Log("执行完毕");
        //}

        void OnGUI()
        {
            toChange = (Font)EditorGUILayout.ObjectField(toChange, typeof(Font), true, GUILayout.MinWidth(100f));
            if (GUILayout.Button("修改"))
            {
                toChangeFont = toChange;
                if (toChangeFont == null)
                {
                    Debug.LogError("需要先设置字体");
                    return;
                }
                ProcessChangeFont();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void ProcessChangeFont()
        {
            Object[] labels = Selection.GetFiltered(typeof(Text), SelectionMode.Deep);
            foreach (Object item in labels)
            {
                Text label = (Text)item;
                label.font = toChangeFont;
            }

            Save();
            Debug.Log("执行完毕");
        }

        static void Save()
        {
            var gameObjects = Selection.gameObjects;
            if (gameObjects == null || gameObjects.Length == 0)
                return;
            foreach (var go in gameObjects)
            {
                EditorUtility.SetDirty(go);
            }
            AssetDatabase.SaveAssets();
        }
    }
}