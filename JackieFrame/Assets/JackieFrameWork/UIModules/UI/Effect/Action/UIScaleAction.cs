using UnityEngine;
using System.Collections;

namespace fs
{
    /// <summary>
    /// 缩放
    /// @author hannibal
    /// @time 2016-4-8
    /// </summary>
    [DisallowMultipleComponent]
    public class UIScaleAction : MonoBehaviour
    {
        /// <summary>
        /// 缩放到
        /// </summary>
        public float m_ToScale = 0.8f;
        /// <summary>
        /// 缩放速度
        /// </summary>
        public float m_Duration = 1;
        /// <summary>
        /// 是否循环
        /// </summary>
        public bool m_Repeat = true;

        private bool m_Active = false;
        private int m_LoopCount = 0;

        void OnEnable()
        {
            m_LoopCount = 0;
            Start();
        }

        void OnDisable()
        {
            Stop(1);
        }

        void OnScaleOut()
        {
            if (!m_Active) return;
            UIEffectTools.ScaleTo(gameObject, m_Duration, 1, OnScaleIn);
        }

        void OnScaleIn()
        {
            if (!m_Active) return;
            m_LoopCount++;
            //非循环播放，只执行一次
            if (m_Repeat || (!m_Repeat && m_LoopCount == 1))
            {
                UIEffectTools.ScaleTo(gameObject, m_Duration, m_ToScale, OnScaleOut);
            }
        }

        public void Start()
        {
            if (m_Active) return;
            m_Active = true;
            OnScaleIn();
        }

        public void Stop(float scale)
        {
            if (!m_Active) return;
            m_Active = false;
            UIEffectTools.Stop(gameObject);
            UIEffectTools.ScaleTo(gameObject, 0, scale, null);
        }
    }
}