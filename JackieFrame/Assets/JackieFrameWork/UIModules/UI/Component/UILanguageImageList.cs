using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace fs
{
    [System.Serializable]
    public class UILanguageImageListItem
    {
        public Sprite[] Image;                    //用于存放图片的数组
        public Vector2 OffsetPos;               //位置偏移
    }

    /// <summary>
    /// UI图片列表，同一时刻，只显示一张
    /// @author hannibal
    /// @time 2016-2-25
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UILanguageImageList : UIComponentBase
    {
        public UILanguageImageListItem[] m_ImageList;//用于存放图片的数组
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
            SetImage(m_ImgIndex);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="index">索引值</param>
        public void SetImage(int index)
        {
            if(m_ImageList.Length == 0)
            {
                ImgComponent.sprite = null;
                Debuger.LogWarning("需要设置图片语音信息:" + index);
                return;
            }
            int lang = (int)GlobalID.LanguageType;
            if (lang >= m_ImageList.Length)
                lang = 0;
            UILanguageImageListItem lang_item = m_ImageList[lang];

            if (index < 0 || index >= lang_item.Image.Length)
            {
                ImgComponent.sprite = null;
                Debuger.LogWarning("图片索引超出范围:" + index);
            }
            else
            {
                ImgComponent.sprite = lang_item.Image[index];
                m_ImgIndex = index;
                transform.localPosition = m_InitPos + m_ImageList[lang].OffsetPos;
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