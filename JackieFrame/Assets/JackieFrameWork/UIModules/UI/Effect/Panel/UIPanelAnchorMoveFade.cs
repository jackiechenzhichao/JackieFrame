using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace fs
{
    /// <summary>
    /// panel 锚点位移动画 分上下左右四个方向
    /// </summary>
    public class UIPanelAnchorMoveFade:UIPanelAnimation
    {
        public bool m_OpenFadeAnim = true;
        public float m_OffestAlpha;     // 淡出值
        public float m_FixAlpha;    // 标准值

        public List<RectTransform> m_TopObjs;
        public Vector3 m_TopOffest;
        public float m_TopDuration;
        public List<RectTransform> m_BottomObjs;
        public Vector3 m_BottomOffest;
        public float m_BottomDuration;
        public List<RectTransform> m_LeftObjs;
        public Vector3 m_LeftOffest;
        public float m_LeftDuration;
        public List<RectTransform> m_RightObjs;
        public Vector3 m_RightOffest;
        public float m_RightDuration;

        //  缓存标准坐标
        private List<Vector3> m_TopFixPos;
        private List<Vector3> m_BottomFixPos;
        private List<Vector3> m_LeftFixPos;
        private List<Vector3> m_RightFixPos;

        // 缓存CanvasGroups 用于淡入淡出效果
        private List<CanvasGroup> m_TopCanvasGroups;
        private List<CanvasGroup> m_BottomCanvasGroups;
        private List<CanvasGroup> m_LeftCanvasGroups;
        private List<CanvasGroup> m_RightCanvasGroups;
        
        private float defaultToAlpha = 0f;
        private float defaultFromAlpha = 1f;
        private float defaultDuration = 0.3f;

        private Vector3 defaultTopOffest = new Vector3(0f, 50f, 0f);
        private Vector3 defaultBottomOffest = new Vector3(0f, -50f, 0f);
        private Vector3 defaultLeftOffest = new Vector3(-50f, 0f, 0f);
        private Vector3 defaultRightOffest = new Vector3(50f, 0f, 0f);

        public override void Awake()
        {
            InitComponent(m_TopObjs, ref m_TopCanvasGroups, ref m_TopFixPos);
            InitComponent(m_BottomObjs, ref m_BottomCanvasGroups, ref m_BottomFixPos);
            InitComponent(m_LeftObjs, ref m_LeftCanvasGroups, ref m_LeftFixPos);
            InitComponent(m_RightObjs, ref m_RightCanvasGroups, ref m_RightFixPos);
            InitParam();
        }

        // 初始化组件
        private void InitComponent(List<RectTransform> objs, ref List<CanvasGroup> canvasList, ref List<Vector3> posList)
        {
            if (objs.Count > 0)
            {
                canvasList = new List<CanvasGroup>();
                posList = new List<Vector3>();
                for (int i = 0; i < objs.Count; i++)
                {
                    RectTransform rect = objs[i];
                    canvasList.Add(GameObjectUtils.GetOrCreateComponent<CanvasGroup>(rect.gameObject));
                    posList.Add(rect.localPosition);
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Reset();
        }

        public override void Update()
        {
        }

        private void Reset()
        {
            InitParam();
        }

        private void InitParam()
        {
            if (m_OffestAlpha == 0) m_OffestAlpha = defaultToAlpha;
            if (m_FixAlpha == 0) m_FixAlpha = defaultFromAlpha; 

            if (m_TopDuration == 0) m_TopDuration = defaultDuration;
            if (m_BottomDuration == 0) m_BottomDuration = defaultDuration;
            if (m_LeftDuration == 0) m_LeftDuration = defaultDuration;
            if (m_RightDuration == 0) m_RightDuration = defaultDuration;

            if (m_TopOffest == Vector3.zero) m_TopOffest = defaultTopOffest;
            if (m_BottomOffest == Vector3.zero) m_BottomOffest = defaultBottomOffest;
            if (m_LeftOffest == Vector3.zero) m_LeftOffest = defaultLeftOffest;
            if (m_RightOffest == Vector3.zero) m_RightOffest = defaultRightOffest;
        }

        /// <summary>
        ///  正向执行, ui从屏幕外进入+淡入
        /// </summary>
        /// <param name="callback"></param>
        public void PlayForward(float delay = 0f, Action callback = null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetEase(m_easeType);
            for (int i = 0; i < m_TopObjs.Count; i++)
            {
                if (!m_TopObjs[i].gameObject.activeInHierarchy) continue;
                RectTransform rect = m_TopObjs[i];
                rect.localPosition = m_TopFixPos[i] + m_TopOffest;
                Tweener tween = rect.DOLocalMoveY(m_TopFixPos[i].y, m_TopDuration).SetEase(m_easeType);
                sequence.Insert(delay, tween);
                if (m_OpenFadeAnim)
                {
                    m_TopCanvasGroups[i].alpha = m_OffestAlpha;
                    Tweener fade = m_TopCanvasGroups[i].DOFade(m_FixAlpha, m_TopDuration).SetEase(m_easeType);
                    sequence.Insert(delay, fade);
                }
                
            }
            
            for (int i = 0; i < m_BottomObjs.Count; i++)
            {
                if (!m_BottomObjs[i].gameObject.activeInHierarchy) continue;
                RectTransform rect = m_BottomObjs[i];
                rect.localPosition = m_BottomFixPos[i] + m_BottomOffest;
                Tweener tween = rect.DOLocalMoveY(m_BottomFixPos[i].y, m_BottomDuration).SetEase(m_easeType);
                sequence.Insert(delay, tween);
                if (m_OpenFadeAnim)
                {
                    m_BottomCanvasGroups[i].alpha = m_OffestAlpha;
                    Tweener fade = m_BottomCanvasGroups[i].DOFade(m_FixAlpha, m_BottomDuration).SetEase(m_easeType);
                    sequence.Insert(delay, fade);
                }
            }
            for (int i = 0; i < m_LeftObjs.Count; i++)
            {
                if (!m_LeftObjs[i].gameObject.activeInHierarchy) continue;
                RectTransform rect = m_LeftObjs[i];
                rect.localPosition = m_LeftFixPos[i] + m_LeftOffest;
                Tweener tween = rect.DOLocalMoveX(m_LeftFixPos[i].x, m_LeftDuration).SetEase(m_easeType);
                sequence.Insert(delay, tween);
                if (m_OpenFadeAnim)
                {
                    m_LeftCanvasGroups[i].alpha = m_OffestAlpha;
                    Tweener fade = m_LeftCanvasGroups[i].DOFade(m_FixAlpha, m_LeftDuration).SetEase(m_easeType);
                    sequence.Insert(delay, fade);
                }
                
            }
            for (int i = 0; i < m_RightObjs.Count; i++)
            {
                if (!m_RightObjs[i].gameObject.activeInHierarchy) continue;
                RectTransform rect = m_RightObjs[i];
                rect.localPosition = m_RightFixPos[i] + m_RightOffest;
                Tweener tween = rect.DOLocalMoveX(m_RightFixPos[i].x, m_RightDuration).SetEase(m_easeType);
                sequence.Insert(delay, tween);
                if (m_OpenFadeAnim)
                {
                    m_RightCanvasGroups[i].alpha = m_OffestAlpha;
                    Tweener fade = m_RightCanvasGroups[i].DOFade(m_FixAlpha, m_RightDuration).SetEase(m_easeType);
                    sequence.Insert(delay, fade);
                }
                
            } 

            if (callback != null)
                sequence.OnComplete(() => { callback(); });
            //sequence.PlayForward();
        }

        /// <summary>
        ///  反向执行
        /// </summary>
        public void PlayBack(float delay = 0f, Action callback = null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetEase(m_easeType);
            for (int i = 0; i < m_TopObjs.Count; i++)
            {
                if (!m_TopObjs[i].gameObject.activeInHierarchy) continue;
                RectTransform rect = m_TopObjs[i];
                rect.localPosition = m_TopFixPos[i];
                Tweener tween = rect.DOLocalMoveY(m_TopFixPos[i].y + m_TopOffest.y, m_TopDuration).SetEase(m_easeType);
                sequence.Insert(delay, tween);
                
                if (m_OpenFadeAnim)
                {
                    m_TopCanvasGroups[i].alpha = m_FixAlpha;
                    Tweener fade = m_TopCanvasGroups[i].DOFade(m_OffestAlpha, m_TopDuration).SetEase(m_easeType);
                    sequence.Insert(delay, fade);
                }
                //Debuger.LogWarning(string.Format("{0} from:{1} to:{2}", rect.name, m_TopFixPos[i].y, m_TopFixPos[i].y + m_TopOffest.y));
            }

            for (int i = 0; i < m_BottomObjs.Count; i++)
            {
                if (!m_BottomObjs[i].gameObject.activeInHierarchy) continue;
                RectTransform rect = m_BottomObjs[i];
                rect.localPosition = m_BottomFixPos[i];
                Tweener tween = rect.DOLocalMoveY(m_BottomFixPos[i].y + m_BottomOffest.y, m_BottomDuration).SetEase(m_easeType);
                sequence.Insert(delay, tween);

                if (m_OpenFadeAnim)
                {
                    m_BottomCanvasGroups[i].alpha = m_FixAlpha;
                    Tweener fade = m_BottomCanvasGroups[i].DOFade(m_OffestAlpha, m_BottomDuration).SetEase(m_easeType);
                    sequence.Insert(delay, fade);
                }
                //Debuger.LogWarning(string.Format("{0} from:{1} to:{2}", rect.name, m_BottomFixPos[i].y, m_BottomFixPos[i].y + m_BottomOffest.y));
            }

            for (int i = 0; i < m_LeftObjs.Count; i++)
            {
                if (!m_LeftObjs[i].gameObject.activeInHierarchy) continue;
                RectTransform rect = m_LeftObjs[i];
                rect.localPosition = m_LeftFixPos[i] ;
                Tweener tween = rect.DOLocalMoveX(m_LeftFixPos[i].x + m_LeftOffest.x, m_LeftDuration).SetEase(m_easeType);
                sequence.Insert(delay, tween);
                
                if (m_OpenFadeAnim)
                {
                    m_LeftCanvasGroups[i].alpha = m_FixAlpha;
                    Tweener fade = m_LeftCanvasGroups[i].DOFade(m_OffestAlpha, m_LeftDuration).SetEase(m_easeType);
                    sequence.Insert(delay, fade);
                }
                //Debuger.LogWarning(string.Format("{0} from:{1} to:{2}", rect.name, m_LeftFixPos[i].x, m_LeftFixPos[i].x + m_LeftOffest.x));
            }

            for (int i = 0; i < m_RightObjs.Count; i++)
            {
                if (!m_RightObjs[i].gameObject.activeInHierarchy) continue;
                RectTransform rect = m_RightObjs[i];
                rect.localPosition = m_RightFixPos[i];
                Tweener tween = rect.DOLocalMoveX(m_RightFixPos[i].x + m_RightOffest.x, m_RightDuration).SetEase(m_easeType);
                sequence.Insert(delay, tween);
                
                if (m_OpenFadeAnim)
                {
                    m_RightCanvasGroups[i].alpha = m_FixAlpha;
                    Tweener fade = m_RightCanvasGroups[i].DOFade(m_OffestAlpha, m_RightDuration).SetEase(m_easeType);
                    sequence.Insert(delay, fade);
                }
                //Debuger.LogWarning(string.Format("{0} from:{1} to:{2}", rect.name, m_RightFixPos[i].x, m_RightFixPos[i].x + m_RightOffest.x));
            }
             
            if (callback != null)
                sequence.OnComplete(() => { callback(); });
            //sequence.PlayForward();
        }
    }
}
