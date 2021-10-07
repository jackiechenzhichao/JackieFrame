
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：ui字体
// 创建时间：2019-09-18 11:08:14
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class UIFontList : UIComponentBase
    {
        public Font[] m_FontList;
        private int m_FontIndex = 0;

        private Text m_TextComponent = null;
        private Text TextComponent
        {
            get
            {
                if (m_TextComponent == null) m_TextComponent = GetComponent<Text>();
                return m_TextComponent;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (m_FontList == null || m_FontList.Length == 0) Debuger.LogError("没有设置基础数据");
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="index">索引值</param>
        public void SetFont(int index)
        {
            if (index < 0 || index >= m_FontList.Length)
            {
                Debuger.LogWarning("字体索引超出范围:" + index);
            }
            else
            {
                TextComponent.font = m_FontList[index];
                m_FontIndex = index;
            }
        }
        
        /// <summary>
        /// 返回索引
        /// </summary>
        public int FontIndex
        {
            get { return m_FontIndex; }
        }
    }
}
