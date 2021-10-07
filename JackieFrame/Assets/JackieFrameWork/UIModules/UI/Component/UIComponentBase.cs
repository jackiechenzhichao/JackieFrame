using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// ui组件基类
    /// @author hannibal
    /// @time 2016-2-5
    /// </summary>
    public abstract class UIComponentBase : MonoBehaviour
    {
        protected bool m_IsVisible = true;
        //～～～～～～～～～～～～～～～～～～～～～～～基本方法～～～～～～～～～～～～～～～～～～～～～～～//
        /// <summary>
        /// 用于取界面控件
        /// </summary>
        protected virtual void Awake() { }

        protected virtual void OnEnable()
        {
        }
        protected virtual void OnDisable()
        {
        }
        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// 设置物体的激活状态
        /// </summary>
        /// <param name="is_Show">是否激活</param>
        public virtual void SetVisible(bool is_Show)
        {
            m_IsVisible = is_Show;
            if (gameObject != null)
            {
                gameObject.SetActive(is_Show);
            }
        }
    }
}