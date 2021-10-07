using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace fs
{
    /// <summary>
    /// 旋转动画：可以支持超过360度的旋转
    /// @author hannibal
    /// @time 2018-3-20
    /// </summary>
    public class UIPanelRotate : UIPanelAnimation
    {
        public float m_ToRotate;

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
        }

        public void PlayForward()
        {
            gameObject.transform.DORotate(new Vector3(0, 0, m_ToRotate), m_Duration, RotateMode.LocalAxisAdd).SetEase(m_easeType);
        }
    }
}