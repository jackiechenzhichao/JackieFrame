using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace fs
{
    /// <summary>
    /// UI界面alpha渐变
    /// @author hannibal
    /// @time 2016-3-26
    /// </summary>
    public class UIPanelFade : UIPanelAnimation
    {
        public float m_ToAlpha;
        public float m_FromAlpha;

        private CanvasGroup m_CanvasGroup;

        private float defaultDuration = 0.4f;
        private float defaultFromAlpha = 0;
        private float defaultToAlpha = 1;

        public override void Awake()
        {
#if !EXTEND_EDITOR
            if (m_CanvasGroup == null)
            {
                m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
            }
            if (m_CanvasGroup == null)
            {
                m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
#endif
        }

        public override void Start()
        {
        }

        private float m_CurTick;
        public override void Update()
        {
#if !EXTEND_EDITOR
            if (m_CurTick <= Time.time && m_CurTick != -1.0f)
            {
                PlayForward();
                m_CurTick = -1.0f;
            }
#endif
        }

        public override void OnEnable()
        {
#if !EXTEND_EDITOR
            base.OnEnable();

            if (m_FromAlpha == 0) m_FromAlpha = defaultFromAlpha;
            if (m_ToAlpha == 0) m_ToAlpha = defaultToAlpha;
            if (m_Duration == 0) m_Duration = defaultDuration;

            m_CurTick = Time.time + m_Delay;
            m_CanvasGroup.alpha = m_FromAlpha;
#endif
        }

        public override void OnDisable()
        {
#if !EXTEND_EDITOR
            Reset();
            base.OnDisable();
#endif
        }

        public void Reset()
        {
            m_CurTick = Time.time + m_Delay;
            m_CanvasGroup.DOFade(m_FromAlpha, 0);
        }

        public void PlayForward()
        {
            m_CanvasGroup.DOFade(m_ToAlpha, m_Duration).SetEase(m_easeType);
        }
    }

}