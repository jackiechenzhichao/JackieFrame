using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace fs
{
    /// <summary>
    /// 缩放
    /// @author hannibal
    /// @time 2016-3-26
    /// </summary>
    public class UIPanelScale : UIPanelAnimation
    {
        public float m_ToMidDuration;

        public Vector2 m_FromScale;
        public Vector2 m_MidScale;
        public Vector2 m_ToScale;

        public List<GameObject> m_CompleteInfluenceList;

        private float   defaultDuration      = 0.7f;
        private float   defaultToMidDuration = 0.3f;
        private Vector2 defaultFromScale     = new Vector2(0.8f, 0.8f);
        private Vector2 defaultMidScale      = new Vector2(1.02f, 1.02f);
        private Vector2 defaultToScale       = new Vector2(1f, 1f);

        public override void Awake()
        {
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

            if (m_Duration == 0) m_Duration              = defaultDuration;
            if (m_ToMidDuration == 0) m_ToMidDuration = defaultToMidDuration;
            if (m_FromScale == Vector2.zero) m_FromScale = defaultFromScale;
            if (m_MidScale == Vector2.zero) m_MidScale   = defaultMidScale;
            if (m_ToScale == Vector2.zero) m_ToScale     = defaultToScale;

            gameObject.transform.localScale = new Vector3(m_FromScale.x, m_FromScale.y, 1);
            m_CurTick                       = Time.time + m_Delay;
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
            m_CurTick                       = Time.time + m_Delay;
            gameObject.transform.localScale = new Vector3(m_FromScale.x, m_FromScale.y, 1);
        }

        public void PlayForward()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetEase(m_easeType);
            sequence.Append(gameObject.transform.DOScale(new Vector3(m_MidScale.x, m_MidScale.y, 1),
                                                        m_ToMidDuration));
            sequence.Append(gameObject.transform.DOScale(new Vector3(m_ToScale.x, m_ToScale.y, 1),
                                                         m_Duration - m_ToMidDuration));
            sequence.OnComplete(() =>
            {
                if (m_CompleteInfluenceList == null)
                {
                    return;
                }

                for (int i = 0; i < m_CompleteInfluenceList.Count; i++)
                {
                    m_CompleteInfluenceList[i].SetActive(true);
                }
            });
        }
    }
}