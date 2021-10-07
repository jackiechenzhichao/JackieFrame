using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace fs
{
    /// <summary>
    /// image进度
    /// @author hannibal
    /// @time 2016-3-26
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UIImageFillAction : MonoBehaviour
    {
        public float m_Duration = 1f;

        private Image m_ImageComponent = null;
        private bool m_Active = false;
        private float m_Speed = 0;
        public float m_StartValue = 0;
        private float m_ToValue = 0;
        private float m_StartTime;

        void Awake()
        {
            m_ImageComponent = GetComponent<Image>();
            m_ImageComponent.fillAmount = m_StartValue;
            m_Active = false;
        }

        void Update()
        {
            if (m_Active)
            {
                if (m_StartTime + m_Duration <= Time.realtimeSinceStartup)
                {
                    m_ImageComponent.fillAmount = m_ToValue;
                    m_Active = false;
                }
                else
                {
                    m_ImageComponent.fillAmount = m_StartValue + m_Speed * (Time.realtimeSinceStartup - m_StartTime);
                }
            }
        }

        public float Value
        {
            set
            {
                m_ImageComponent.fillAmount = value;
            }
        }

        public float FillAmount
        {
            set
            {
                //正在执行，且目标值和现在的一样
                if (m_Active && m_ToValue == value) return;
                //已经是目标值，忽视
                if (m_ImageComponent.fillAmount == value) return;

                m_StartValue = m_ImageComponent.fillAmount;
                m_ToValue = value;

                m_StartTime = Time.realtimeSinceStartup;
                m_Speed = (m_ToValue - m_StartValue) / m_Duration;

                m_Active = true;
            }
        }

        public float Duration
        {
            set { m_Duration = value; if (m_Duration == 0)m_Duration = 1; }
        }
    }
}