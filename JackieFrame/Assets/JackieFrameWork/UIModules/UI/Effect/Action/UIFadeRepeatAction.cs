using UnityEngine;
using System.Collections;

namespace fs
{
    /// <summary>
    /// 闪烁
    /// @author hannibal
    /// @time 2016-3-26
    /// </summary>
    [DisallowMultipleComponent]
    public class UIFadeRepeatAction : MonoBehaviour
    {
        /// <summary>
        /// 渐变到
        /// </summary>
        public float m_ToAlpha = 0;
        /// <summary>
        /// 渐变速度
        /// </summary>
        public float m_Duration = 1;
        /// <summary>
        /// 是否循环
        /// </summary>
        public bool m_Repeat = true;

        private bool m_Active = false;

        void OnEnable()
        {
            Start();
        }

        void OnDisable()
        {
            Stop(1);
        }

        void OnFadeOut()
        {
            if (!m_Active) return;
            UIEffectTools.FadeIn(gameObject, m_Duration, 1, OnFadeIn);
        }

        void OnFadeIn()
        {
            if (!m_Active) return;
            UIEffectTools.FadeOut(gameObject, m_Duration, m_ToAlpha, OnFadeOut);
        }

        public void Start()
        {
            if (m_Active) return;
            m_Active = true;
            OnFadeIn();
        }

        public void Stop(float alpha)
        {
            if (!m_Active) return;
            m_Active = false;
            UIEffectTools.FadeStop(gameObject);
            UIEffectTools.FadeIn(gameObject, 0, alpha, null);
        }
    }
}