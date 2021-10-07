using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// ui制作快捷键，如
    /// alt shift l 自动生成text
    /// alt shift s 自动生成Image
    /// alt shift i 自动生成input field
    /// @author matt
    /// @time 2019-4-10
    /// </summary>
    public static class EditorUITool
    {
        #region 创建
        public static int UIlayer = LayerMask.NameToLayer("UI");
        static int FontSize = 26;
        //static Font fnt;
        static void CacheFont(Text text)
        {
            //if (fnt == null)
            //    fnt = AssetDatabase.LoadAssetAtPath<Font>("Assets/Art/Font/ui/HKYT.ttf");
            //if (fnt != null)
            //{
            //    text.font = fnt;
            //}
        }

        [MenuItem("UGUI/CreateText #&L", false, 1)]
        public static void CreateText()
        {
            if (Selection.gameObjects.Length == 1)
            {
                GameObject obj = Selection.gameObjects[0];
                GameObject text = new GameObject();
                RectTransform textRect = text.AddComponent<RectTransform>();
                Text textTx = text.AddComponent<Text>();
                text.transform.SetParent(obj.transform);
                text.name = "Txt";
                text.layer = UIlayer;
                //设置字体
                CacheFont(textTx);
                textTx.fontSize = FontSize;
                textTx.text = "";
                textTx.alignment = TextAnchor.MiddleCenter;
                textTx.supportRichText = false;
                textTx.raycastTarget = false;
                textRect.localScale = Vector3.one;
                textRect.sizeDelta = new Vector2(100, 50);//考虑到后期修改字体大小或多语言，高度尽量不要少于40
                RectTransformZero(textRect);
                Selection.activeGameObject = text;
            }
        }

        [MenuItem("UGUI/CreateButton #&B", false, 2)]
        public static void CreateButton()
        {
            if (Selection.gameObjects.Length == 1)
            {
                GameObject obj = Selection.gameObjects[0];
                if (obj == null) return;

                GameObject button = new GameObject();
                GameObject buttonTx = new GameObject();

                RectTransform buttonRect = button.AddComponent<RectTransform>();
                RectTransform buttonTxRect = buttonTx.AddComponent<RectTransform>();

                button.AddComponent<Image>();
                Text texBtn = buttonTx.AddComponent<Text>();
                CacheFont(texBtn);
                texBtn.fontSize = FontSize;
                texBtn.text = "Button";
                texBtn.raycastTarget = false;
                buttonTxRect.sizeDelta = new Vector2(texBtn.preferredWidth, 50);
                button.transform.SetParent(obj.transform);
                buttonTx.transform.SetParent(button.transform);
                button.name = "Btn";
                buttonTx.name = "Txt";

                button.layer = UIlayer;
                buttonTx.layer = UIlayer;

                RectTransformZero(buttonRect);
                RectTransformZero(buttonTxRect);
                Selection.activeGameObject = button;
            }
        }

        [MenuItem("UGUI/CreateImage #&S", false, 3)]
        public static void CreateImage()
        {
            if (Selection.gameObjects.Length == 1)
            {
                GameObject obj = Selection.gameObjects[0];

                GameObject image = new GameObject();
                RectTransform imageRect = image.AddComponent<RectTransform>();
                Image img = image.AddComponent<Image>();
                image.transform.SetParent(obj.transform);
                image.name = "Img";
                image.layer = 5;
                img.raycastTarget = false;

                RectTransformZero(imageRect);
                Selection.activeGameObject = image;
            }

        }
        [MenuItem("UGUI/CreatImageButton #&S", false, 4)]
        public static void CreatImageButton()
        {
            if (Selection.gameObjects.Length == 1)
            {
                GameObject obj = Selection.gameObjects[0];

                GameObject image = new GameObject();
                RectTransform imageRect = image.AddComponent<RectTransform>();
                Image img = image.AddComponent<Image>();
                image.transform.SetParent(obj.transform);
                image.name = "Btn";
                image.layer = 5;
                img.raycastTarget = true;

                RectTransformZero(imageRect);
                Selection.activeGameObject = image;
            }

        }
        [MenuItem("UGUI/CreateRawImage #&T", false, 5)]
        public static void CreateRawImage()
        {
            if (Selection.gameObjects.Length == 1)
            {
                GameObject obj = Selection.gameObjects[0];

                GameObject image = new GameObject();
                RectTransform imageRect = image.AddComponent<RectTransform>();
                RawImage img = image.AddComponent<RawImage>();
                image.transform.SetParent(obj.transform);
                image.name = "Raw Image";
                image.layer = 5;
                img.raycastTarget = false;

                RectTransformZero(imageRect);
                Selection.activeGameObject = image;
            }

        }

        [MenuItem("UGUI/CreateInputField #&I", false, 6)]
        public static void CreateInputField()
        {
            if (Selection.gameObjects.Length == 1)
            {
                GameObject obj = Selection.gameObjects[0];

                GameObject inputField = new GameObject();
                RectTransform rectTransform = inputField.AddComponent<RectTransform>();
                inputField.AddComponent<Image>();
                //image.sprite = Resources.Load<Sprite>("UnityPlugins/UGUIShortcutKeyTexture/background1");
                inputField.AddComponent<InputField>();
                rectTransform.localScale = new Vector3(1, 1, 1);
                inputField.layer = UIlayer;

                inputField.transform.SetParent(obj.transform);
                inputField.name = "InputField";

                GameObject placeholder = new GameObject();
                Text placeholderTx = placeholder.AddComponent<Text>();
                CacheFont(placeholderTx);
                placeholderTx.fontSize = FontSize;
                placeholderTx.text = "Enter text...";
                placeholder.transform.SetParent(inputField.transform);
                placeholder.name = "Placeholder";
                placeholder.layer = UIlayer;
                placeholderTx.color = Color.black;

                GameObject text = new GameObject();
                Text textTx = text.AddComponent<Text>();
                text.transform.SetParent(inputField.transform);
                text.name = "Text";
                text.layer = UIlayer;

                textTx.color = Color.black;

                RectTransformZero(rectTransform);
                Selection.activeGameObject = inputField;
            }
        }

        public static void RectTransformZero(RectTransform rectTransform)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.anchoredPosition3D = Vector3.zero;
        }
        #endregion

        #region 获取控件路径 
        public enum ExportUINode
        {
            Lb,
            Img,
            Transform,
        }
        public static string[] WidgetClass = {
            "Text",
            "Image",
            "Transform",
        };

        [MenuItem("GameObject/获取控件代码段", priority = -1)]
        public static void GetWgtScript()
        {
            var sels = Selection.gameObjects;
            if (sels.Length <= 0) return;
            var fields = new System.Text.StringBuilder();
            var codes = new System.Text.StringBuilder();
            //字段模版
            var fieldTemplate = "private {0} m_{1};\n";
            //代码模版
            var baseCodeTemplate = "m_{1} = transform.Find(\"{0}\")";

            foreach (var one in sels)
            {
                var trans = one.transform;

                if (trans == null) return;
                var path = FindPath(trans);


                string template = baseCodeTemplate;
                var nodeType = GetNodeType(trans);
                var nodeClassType = WidgetClass[(int)nodeType];
                var nodeName = trans.name;
                //generate field
                //nodeName = nodeName.Substring(0, 1).ToLower() + nodeName.Substring(1);
                fields.AppendFormat(fieldTemplate, nodeClassType, nodeName);
                //generate code
                string code;
                if (nodeType == ExportUINode.Transform)
                    template += " as RectTransform;";
                else
                    template = baseCodeTemplate + ".GetComponent <{2}>();";
                code = string.Format(template, path, nodeName, nodeClassType);

                codes.AppendLine(code);
                //Debug.Log(code);
            }
            var copy = fields.ToString() + "\n" + codes.ToString();
            EditorGUIUtility.systemCopyBuffer = copy;
        }

        private static ExportUINode GetNodeType(Transform trans)
        {
            //var btn = trans.GetComponent<Button>();
            var lb = trans.GetComponent<Text>();
            var img = trans.GetComponent<Image>();
            var type = ExportUINode.Transform;
            //if (btn != null) type = ExportUINode.Btn;
            //else
            //{
            if (img != null) type = ExportUINode.Img;
            else if (lb != null) type = ExportUINode.Lb;
            //}
            return type;
        }

        static HashSet<string> validRoots;
        public static string FindPath(Transform trans, bool includeUI = false)
        {
            if (validRoots == null)
            {
                validRoots = new HashSet<string>();
                int layerCount = 4;
                for (int i = 0; i < layerCount; i++)
                    validRoots.Add(string.Format("Layer{0}", i));
                validRoots.Add("UICanvas");
            }
            var sb = new System.Text.StringBuilder();
            var nodes = new Stack<string>();

            while (trans != null &&
                !validRoots.Contains(trans.name))
            {
                nodes.Push(trans.name);
                trans = trans.parent;
            }
            //如果不包含根节点，则先去掉
            if (!includeUI && nodes.Count > 0) nodes.Pop();
            while (nodes.Count > 0)
            {
                sb.Append(nodes.Pop());
                if (nodes.Count > 0)
                    sb.Append("/");
            }
            var path = sb.ToString();
            return path;
        }

        [MenuItem("Assets/拷贝路径", false, 1020)]
        static void CopyAssetPath()
        {
            var guids = Selection.assetGUIDs;
            if (guids != null)
            {
                var sb = new System.Text.StringBuilder();
                var isSelOneAsset = guids.Length <= 1;
                foreach (var one in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(one);
                    if (isSelOneAsset)
                        sb.Append(path);
                    else sb.AppendLine(path);
                }
                EditorGUIUtility.systemCopyBuffer = sb.ToString();
            }
        }
        #endregion
    }
}