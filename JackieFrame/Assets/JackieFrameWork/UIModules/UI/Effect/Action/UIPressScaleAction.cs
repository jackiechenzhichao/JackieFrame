using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.EventSystems;

namespace fs
{
    /// <summary>
    /// 按钮按下的缩放动画
    /// @author hannibal
    /// @time 2016-2-25
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UIPressScaleAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public float Time = -1f;
        public float Scale = -1f;

        public GameObject InfluenceObject = null;

        private float defaultTime = 0.3f;
        private float defaultScale = 0.9f;

        private bool m_Enable = true;
        private Vector3 m_InitScale = Vector3.one;

        void Awake()
        {
            if (Time == -1) Time = defaultTime;
            if (Scale == -1) Scale = defaultScale;
            m_InitScale = transform.localScale;

            GetComponent<Image>().raycastTarget = true;
        }
        public void SetEnable(bool b)
        {
            m_Enable = b;
            InfObject.transform.DOKill();
            InfObject.transform.localScale = m_InitScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!m_Enable) return;
            InfObject.transform.DOScale(m_InitScale, Time);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!m_Enable) return;
            InfObject.transform.DOScale(m_InitScale, Time);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!m_Enable) return;
            InfObject.transform.DOScale(m_InitScale * Scale, Time);
        }

        private GameObject InfObject
        {
            get { return InfluenceObject != null ? InfluenceObject : gameObject; }
        }
    }
}