using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fs
{
    /// <summary>
    /// UI进度条
    /// @author hannibal
    /// @time 2016-2-25
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UIProgressBar : UIComponentBase
    {
        public bool m_SelfUpdate = true;    //是否自我更新

        private float m_FillCount;          //进度条的填充值
        private float m_FillSpeed;          //进度条填充的速度
        private float m_StartFill = 0;
        private float m_fStartTime = 0;      //变换开始时间
        private float m_fEndTime = 0;        //变换结束时间
        private bool m_Active = false;       //是否激活
        private System.Action m_OnComplete;  //播放完成回调

        //进度条中的图片组件
        private Image m_ImgComponent = null;
        private Image ImgComponent
        {
            get
            {
                if (m_ImgComponent == null) m_ImgComponent = GetComponent<Image>();
                return m_ImgComponent;
            }
        }

        /// <summary>
        /// 初始化文字滚动组件
        /// </summary>
        public void Play(float value, float time, System.Action complete = null)
        {
            Debug.Assert(time > 0);

            m_Active = true;

            m_StartFill = ImgComponent.fillAmount;
            m_fStartTime = Time.realtimeSinceStartup;
            m_fEndTime = m_fStartTime + time;

            m_FillCount = value;
            m_FillSpeed = (m_FillCount - m_StartFill) / time;
            m_OnComplete = complete;
        }

        void Update()
        {
            if (m_SelfUpdate)
            {
                Simulate(Time.deltaTime);
            }
        }

        /// <summary>
        /// 转换激活状态
        /// </summary>
        /// <param name="detal_time">时间</param>
        public void Simulate(float detal_time)
        {
            if (!m_Active) return;

            float curr_time = Time.realtimeSinceStartup;
            if (curr_time >= m_fEndTime)
            {
                ImgComponent.fillAmount = m_FillCount;
                m_Active = false;
                if(m_OnComplete != null)
                {
                    m_OnComplete();
                    m_OnComplete = null;
                }
            }
            else
            {
                float time_elapased = curr_time - m_fStartTime;
                ImgComponent.fillAmount = (m_StartFill + m_FillSpeed * time_elapased);
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UIProgressBar))]
    public class UIProgressBarInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UIProgressBar owner_script = (UIProgressBar)target;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("测试"))
            {
                EditorApplication.update += Update;
                owner_script.Play(1, 1,  ()=>{ EditorApplication.update -= Update; });
            }
            if (GUILayout.Button("反向测试"))
            {
                EditorApplication.update += Update;
                owner_script.Play(0, 1, () => { EditorApplication.update -= Update; });
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Update()
        {
            UIProgressBar owner_script = (UIProgressBar)target;
            owner_script.Simulate(Time.deltaTime);
        }
    }
#endif
}