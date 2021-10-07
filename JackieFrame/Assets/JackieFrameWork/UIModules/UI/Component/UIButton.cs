using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace fs
{
    /// <summary>
    /// UI按钮
    /// @author hannibal
    /// @time 2016-2-25
    /// </summary>
    public class UIButton : UIComponentBase
    {
        protected bool m_IsValid = true;    //激活状态，默认为激活

        /// <summary>
        /// 设置UI按钮的激活状态
        /// </summary>
        /// <param name="b">是否激活</param>
        public virtual void SetValid(bool b)
        {
            m_IsValid = b;
        }
    }
}