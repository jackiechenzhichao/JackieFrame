using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
    /// <summary>
    /// UI文本加下划线
    /// @author hannibal
    /// @time 2016-2-25
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class UITextUnderline : MonoBehaviour
    {
        private Text m_LinkText;

        public int m_OffsetY = 0;   //上下偏移
        public int m_LineBold = 0;  //线条粗细
        public Color m_LineColor;   //颜色

        void Awake()
        {
            m_LinkText = this.GetComponent<Text>();
        }
        void Start()
        {
            CreateLine();
        }

        private void CreateLine()
        {
            //克隆Text，获得相同的属性  
            Text underline = GameObject.Instantiate(m_LinkText) as Text;
            underline.name = "Underline_" + MathUtils.RandRange_Int(0, ushort.MaxValue);
            underline.color = m_LineColor;
            if (m_LineBold > 0) underline.fontSize = m_LineBold;
            underline.transform.SetParent(m_LinkText.transform, false);

            //移除UITextUnderline，防止新创建的text带有脚本
            UITextUnderline underline_script = underline.GetComponent<UITextUnderline>();
            if (underline_script != null)
                GameObject.DestroyImmediate(underline_script);

            //设置下划线坐标和位置  
            RectTransform rt = underline.rectTransform;
            rt.anchoredPosition3D = Vector3.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;
            (underline.transform as RectTransform).anchoredPosition += new Vector2(0, m_OffsetY);

            //构建下划线
            underline.text = "_";
            string str_line = "";
            float perlineWidth = underline.preferredWidth;  //单个下划线宽度  
            float width = m_LinkText.preferredWidth;        //总长度
            int lineCount = (int)Mathf.Round(width / perlineWidth);
            for (int i = 1; i < lineCount; i++)
            {
                str_line += "_";
            }
            underline.text += str_line;
        }
    }
}