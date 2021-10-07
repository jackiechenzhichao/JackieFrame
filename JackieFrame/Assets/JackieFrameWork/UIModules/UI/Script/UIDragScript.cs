using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace fs
{
    /// <summary>
    /// 可拖动窗口
    /// @author hannibal
    /// @time 2016-2-5
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class UIDragScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        RectTransform m_transform = null;
        private bool _isDragging = false;
        public static bool ResetCoords = false;
        private Vector3 m_originalCoods = Vector3.zero;
        private Canvas m_canvas;
        private RectTransform m_canvasRectTransform;
        public int KeepWindowInCanvas = 100;

        void Start()
        {
            m_transform = GetComponent<RectTransform>();
            m_originalCoods = m_transform.position;
            m_canvas = GetComponentInParent<Canvas>();
            m_canvasRectTransform = m_canvas.GetComponent<RectTransform>();
        }

        void Update()
        {
            if (ResetCoords)
                resetCoordinatePosition();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                var delta = ScreenToCanvas(eventData.position) - ScreenToCanvas(eventData.position - eventData.delta);
                m_transform.localPosition += delta;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {

            if (eventData.pointerCurrentRaycast.gameObject == null)
                return;

            //if (eventData.pointerCurrentRaycast.gameObject.name == name)
            {
                _isDragging = true;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
        }

        void resetCoordinatePosition()
        {
            m_transform.position = m_originalCoods;
            ResetCoords = false;
        }

        private Vector3 ScreenToCanvas(Vector3 screenPosition)
        {
            Vector3 localPosition;
            Vector2 min;
            Vector2 max;
            var canvasSize = m_canvasRectTransform.sizeDelta;

            if (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || (m_canvas.renderMode == RenderMode.ScreenSpaceCamera && m_canvas.worldCamera == null))
            {
                localPosition = screenPosition;

                min = Vector2.zero;
                max = canvasSize;
            }
            else
            {
                var ray = m_canvas.worldCamera.ScreenPointToRay(screenPosition);
                var plane = new Plane(m_canvasRectTransform.forward, m_canvasRectTransform.position);

                float distance;
                if (plane.Raycast(ray, out distance) == false)
                {
                    throw new System.Exception("Is it practically possible?");
                };
                var worldPosition = ray.origin + ray.direction * distance;
                localPosition = m_canvasRectTransform.InverseTransformPoint(worldPosition);

                min = -Vector2.Scale(canvasSize, m_canvasRectTransform.pivot);
                max = Vector2.Scale(canvasSize, Vector2.one - m_canvasRectTransform.pivot);
            }
            localPosition.x = Mathf.Clamp(localPosition.x, min.x + KeepWindowInCanvas, max.x - KeepWindowInCanvas);
            localPosition.y = Mathf.Clamp(localPosition.y, min.y + KeepWindowInCanvas, max.y - KeepWindowInCanvas);

            return localPosition;
        }
    }
}