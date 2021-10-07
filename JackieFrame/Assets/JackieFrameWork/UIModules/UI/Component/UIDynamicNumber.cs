using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fs
{
    /// <summary>
    /// 动态显示数值文本
    /// @author hannibal
    /// @time 2015-2-1
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class UIDynamicNumber : UIComponentBase
    {
        public bool m_SelfUpdate = true;

        /**时间：控制改变速度*/
        public float m_TotalTime = 1f;
        public bool m_FixedTime = true;
        public float m_Speed = 0.03f;        //m_FixedTime为false时，起效
        private float m_StartTime = 0f;

        /**颜色*/
        public bool m_EnableColor = false;
        public Color m_Color = Color.white;

        /**数值前后字符串*/
        public string m_PreData = "";
        public string m_EndData = "";
        
        private bool m_Active = false;

        /**值*/
        private int m_Value = 0;
        private int m_InitValue = 0;
        private int m_EndValue = 0;

        private System.Action m_OnComplete = null;

        private Text m_TextComponent;//Text组件
        private Text TextComponent
        {
            get
            {
                if (m_TextComponent == null) m_TextComponent = GetComponent<Text>();
                return m_TextComponent;
            }
        }

        void Update()
        {
            if (m_SelfUpdate)
            {
                Simulate(Time.deltaTime);
            }
        }
        public void Simulate(float detal_time)
        {
            if (!m_Active) return;

            //线性插值
            int to_value = m_InitValue + (int)((m_EndValue - m_InitValue) * ((Time.realtimeSinceStartup - m_StartTime) / m_TotalTime));
            if (Time.realtimeSinceStartup - m_StartTime >= m_TotalTime)
            {
                to_value = m_EndValue;
                m_Active = false;
            }
            Value = to_value;

            if (!m_Active && m_OnComplete != null)
            {
                m_OnComplete();
                m_OnComplete = null;
            }
        }
        /// <summary>
        /// 显示动画
        /// </summary>
        /// <param name="to_value">结束值</param>
        /// <param name="on_end"></param>
        public void To(int to_value, System.Action on_end = null)
        {
            m_Active = true;

            if (to_value == m_EndValue)
            {
                if (on_end != null) on_end();
                return;
            }

            m_OnComplete = on_end;
            m_InitValue = m_Value;
            m_EndValue = to_value;

            m_StartTime = Time.realtimeSinceStartup;
            if (!m_FixedTime)
            {//计算动画时间
                float num = Mathf.Abs(to_value - m_Value);
                float time = num * m_Speed;
                m_TotalTime = time > m_TotalTime ? m_TotalTime : time;
            }
        }
        /// <summary>
        /// 停止动画
        /// </summary>
        public void Stop()
        {
            if (!m_Active) return;
            m_Active = false;
        }

        public int Value
        {
            get { return m_Value; }
            set
            {
                m_Value = value;
                string str_value = m_Value.ToString();
                if (m_EnableColor)str_value = StringUtils.SetFontColor(m_Value.ToString(), "#" + ColorUtils.ColorToHex(m_Color));
                TextComponent.text = m_PreData + str_value + m_EndData;
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UIDynamicNumber))]
    public class UIDynamicNumberInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UIDynamicNumber owner_script = (UIDynamicNumber)target;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("测试"))
            {
                EditorApplication.update += Update;
                owner_script.To(100, () => { EditorApplication.update -= Update; });
            }
            if (GUILayout.Button("反向测试"))
            {
                EditorApplication.update += Update;
                owner_script.To(0, () => { EditorApplication.update -= Update; });
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Update()
        {
            UIDynamicNumber owner_script = (UIDynamicNumber)target;
            owner_script.Simulate(Time.deltaTime);
        }
    }
#endif
}