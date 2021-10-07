using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

namespace fs
{
    /// <summary>
    /// 重写ugui创建控件方式
    /// @author hannibal
    /// @time 2016-12-17
    /// </summary>
    public class EditorOverrideUGUI : Editor
    {
        #region 重写基础组件
        [MenuItem("GameObject/UI/Image")]
        static void CreatImage()
        {
            if (Selection.activeTransform)
            {
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    EditorUITool.CreateImage();
                }
            }
        }
        [MenuItem("GameObject/UI/ImageButton")]
        static void CreatImageButton()
        {
            if (Selection.activeTransform)
            {
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    EditorUITool.CreatImageButton();
                }
            }
        }
        [MenuItem("GameObject/UI/Text")]
        static void CreatText()
        {
            if (Selection.activeTransform)
            {
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    EditorUITool.CreateText();
                }
            }
        }
        [MenuItem("GameObject/UI/RawImage")]
        static void CreatRawImage()
        {
            if (Selection.activeTransform)
            {
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    EditorUITool.CreateRawImage();
                }
            }
        }
        [MenuItem("GameObject/UI/Button")]
        static void CreatButton()
        {
            if (Selection.activeTransform)
            {
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    EditorUITool.CreateButton();
                }
            }
        }
        [MenuItem("GameObject/UI/InputField")]
        static void CreatInputField()
        {
            if (Selection.activeTransform)
            {
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    EditorUITool.CreateInputField();
                }
            }
        }
        #endregion
        
        #region 组件
        static void CreateUGUIComponent<T>()
        {
            if (Selection.activeTransform)
            {
                if (Selection.activeTransform.GetComponentInParent<Canvas>())
                {
                    GameObject obj = Selection.gameObjects[0];
                    if (obj != null)
                    {
                        obj.AddComponent(typeof(T));
                    }
                }
            }
        }
        [MenuItem("UGUI/效果组件-ImageButton", false, 100)]
        static void CreateUGUIComponent_ImageButton()
        {
            CreateUGUIComponent<UIImageButton>();
        }
        [MenuItem("UGUI/效果组件-SwitchButton", false, 101)]
        static void CreateUGUIComponent_SwitchButton()
        {
            CreateUGUIComponent<UISwitchButton>();
        }
        [MenuItem("UGUI/效果组件-ImageList", false, 102)]
        static void CreateUGUIComponent_ImageList()
        {
            CreateUGUIComponent<UIImageList>();
        }
        [MenuItem("UGUI/效果组件-SpriteAnimation", false, 103)]
        static void CreateUGUIComponent_SpriteAnimation()
        {
            CreateUGUIComponent<UISpriteAnimation>();
        }
        [MenuItem("UGUI/效果组件-PressScale", false, 104)]
        static void CreateUGUIComponent_PressScale()
        {
            CreateUGUIComponent<UIPressScaleAction>();
        }
        #endregion
    }
}