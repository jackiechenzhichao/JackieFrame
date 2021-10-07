
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：多语言
// 创建时间：2019-11-11 10:20:05
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
    [System.Serializable]
    public class UILanguageImageItem
    {
        public Sprite Image;                    //用于存放图片的数组
        public Vector2 OffsetPos;               //位置偏移
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UILanguageImage : UIComponentBase
    {
        public UILanguageImageItem[] m_ImageList;//用于存放图片的数组
        public bool m_SetNativeSize = true;     //是否显示原始比例，默认是原始比例

        private Vector2 m_InitPos = Vector2.zero;
        private int m_ImgIndex = 0;             //图片的索引值

        private Image m_ImgComponent = null;    //图片的组件
        private Image ImgComponent
        {
            get
            {
                if (m_ImgComponent == null) m_ImgComponent = GetComponent<Image>();
                return m_ImgComponent;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (m_ImageList == null || m_ImageList.Length == 0) Debuger.LogError("没有设置基础数据");
            m_InitPos = transform.localPosition;

            int lang = (int)GlobalID.LanguageType;
            this.SetImage(lang);
        }

        protected override void OnEnable()
        {
            int lang = (int)GlobalID.LanguageType;
            this.SetImage(lang);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="index">索引值</param>
        public void SetImage(int index)
        {
            if (index < 0 || index >= m_ImageList.Length)
            {
                ImgComponent.sprite = null;
                Debuger.LogWarning("图片索引超出范围:" + index);
            }
            else
            {
                ImgComponent.sprite = m_ImageList[index].Image;
                m_ImgIndex = index;
                transform.localPosition = m_InitPos + m_ImageList[index].OffsetPos;
                if (m_SetNativeSize) SetNativeSize();
            }
        }

        /// <summary>
        /// 设置图片为原始比例
        /// </summary>
        public void SetNativeSize()
        {
            ImgComponent.SetNativeSize();
        }

        /// <summary>
        /// 返回图片的索引
        /// </summary>
        public int ImgIndex
        {
            get { return m_ImgIndex; }
        }
    }
}
