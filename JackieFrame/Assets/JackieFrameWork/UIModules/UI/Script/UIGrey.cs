using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// UI变灰
    /// @author hannibal
    /// @date 2016.12.9
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UIGrey : MonoBehaviour
    {
        [SerializeField, Tooltip("是否变灰")]
        protected bool m_IsGrey = false;
        [SerializeField, Tooltip("变灰材质")]
        protected Material m_Material;

        private Image m_MaterialOwner;

        void Awake()
        {
            m_MaterialOwner = GetComponent<Image>();
            m_MaterialOwner.material = null;
            m_IsGrey = !m_IsGrey;
            SetGrey(!m_IsGrey);
        }

        public void SetGrey(bool grey)
        {
            if (m_IsGrey == grey) return;
            m_IsGrey = grey;

            Material newMaterial = null;
            if (m_IsGrey)
            {
                newMaterial = m_Material;
            }
            if (m_MaterialOwner != null)
            {
                m_MaterialOwner.material = newMaterial;
                SetChildrenMarterial(transform, newMaterial);
            }
        }

        private void SetChildrenMarterial(Transform target, Material newMaterial)
        {
            if (target == null) return;
            for (int i = 0; i < target.childCount; ++i)
            {
                Transform childTarget = target.GetChild(i);
                var targetImg = childTarget.GetComponent<Image>();
                if (targetImg != null) targetImg.material = newMaterial;
                SetChildrenMarterial(childTarget, newMaterial);
            }
        }

        public bool IsGrey
        {
            get { return m_IsGrey; }
        }
    }
}