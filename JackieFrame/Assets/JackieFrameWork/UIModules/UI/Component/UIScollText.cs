using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fs
{
    /// <summary>
    /// 一个滚动播放文字的组件
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class UIScollText : UIComponentBase
    {
        public string m_Content;            //目标显示文字
        public float m_Speed;               //滚动速度（单位为 个/秒）
        public float m_SpeedScale = 1.0f;   //当前播放速度倍率
        private bool m_Active = false;      //激活状态
        private float m_CurrentTime = -1f;  //当前播放了的时间
        private int m_LastIndex = -1;       //上一次显示的Char index
        private Action m_OnComplete;        //播放完成回调
        private Text m_TextComponent;       //Text组件
        private Text TextComponent
        {
            get
            {
                if (m_TextComponent == null) m_TextComponent = GetComponent<Text>();
                return m_TextComponent;
            }
        }

        /// <summary>
        /// 初始化文字滚动组件
        /// </summary>
        public void Play(Action complete = null)
        {
            m_Active = true;
            m_OnComplete = complete;
            m_CurrentTime = 0;
            m_LastIndex = -1;
            TextComponent.text = "";
        }

        /// <summary>
        /// 停止滚动，激活状态为false
        /// </summary>
        public void Stop()
        {
            m_Active = false;
            if (m_OnComplete != null)
            {
                m_OnComplete();
                m_OnComplete = null;
            }
        }

        /// <summary>
        /// 设置速度缩放 SetScale(0) == Pause
        /// </summary>
        /// <param name="speedScale"></param>
        public void SetSpeedScale(float speedScale)
        {
            Debug.Assert(speedScale >= 0);
            m_SpeedScale = speedScale;
        }

        private void Update()
        {
            Simulate(Time.deltaTime);
        }

        /// <summary>
        /// 外部调用的Update接口，只有当更新模式是外部更新时调用该接口才有效
        /// </summary>
        public void Simulate(float detal_time)
        {
            if(m_Active)
            {
                UpdateTxt(detal_time);
            }
        }

        /// <summary>
        /// 更新Text的内部方法
        /// </summary>
        private void UpdateTxt(float deltaTime)
        {
            //速度缩放
            m_CurrentTime += m_SpeedScale * deltaTime;

            if (m_CurrentTime >= 0)
            {
                float value = m_CurrentTime * m_Speed;
                int index = (int)value;//向下取整

                if (index >= m_Content.Length - 1)//显示完毕
                {
                    index = m_Content.Length - 1;
                }

                if (index != m_LastIndex)//和上次的显示不一样，才要刷新
                {
                    m_LastIndex = index;
                    TextComponent.text = m_Content.Substring(0, index + 1);
#if UNITY_EDITOR
                    //编辑器下可能会出现没有自动刷新的情况
                    TextComponent.enabled = false;
                    TextComponent.enabled = true;
#endif
                    //播放完毕，执行回调
                    if (index >= m_Content.Length - 1)
                    {
                        m_Active = false;
                        if (m_OnComplete != null)
                        {
                            m_OnComplete();
                            m_OnComplete = null;
                        }
                    }
                }
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UIScollText))]
    public class UIScollTextInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UIScollText owner_script = (UIScollText)target;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("预览"))
            {
                EditorApplication.update += Update;
                owner_script.Play();
            }
            if (GUILayout.Button("停止"))
            {
                EditorApplication.update -= Update;
                owner_script.Stop();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Update()
        {
            UIScollText owner_script = (UIScollText)target;
            owner_script.Simulate(Time.deltaTime);
        }
    }
#endif
}