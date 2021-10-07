using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace fs
{
    /// <summary>
    /// UI界面移动
    /// @author hannibal
    /// @time 2016-3-26
    /// </summary>
    public class UIPanelMove : UIPanelAnimation
    {
        public Vector3 m_FromPositon;
        public Vector3 m_ToPosition;
        private RectTransform rectTransform;
        public override void Awake()
        {
#if !EXTEND_EDITOR
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
            rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = m_FromPositon;
            m_CurTick = Time.time + m_Delay;
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
            rectTransform.anchoredPosition = m_FromPositon;
        }

        public void PlayForward()
        {
            m_Running = true;
            Tweener tweener = rectTransform.DOAnchorPos3D(m_ToPosition, m_Duration);
            tweener.SetEase(m_easeType);
            tweener.OnComplete(() =>
            {
                m_Running = false;
            });
        }

        public override void Rollback()
        {
            m_Running = true;
            Tweener tweener = rectTransform.DOAnchorPos3D(m_FromPositon, m_Duration);
            tweener.SetEase(m_easeType);
            tweener.OnComplete(() =>
            {
                m_Running = false;
            });
        }
    }
}