using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace fs
{
    /// <summary>
    /// UI图片列表，同一时刻，只显示一张
    /// @author hannibal
    /// @time 2016-2-25
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UIImageList : UIComponentBase
    {
        public Sprite[] m_ImageList;            //用于存放图片的数组
        public bool m_SetNativeSize = true;     //是否显示原始比例，默认是原始比例
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
                ImgComponent.sprite = m_ImageList[index];
                m_ImgIndex = index;
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