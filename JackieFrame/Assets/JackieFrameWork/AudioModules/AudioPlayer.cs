
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：挂到脚本播放声音
// 创建时间：2019-10-16 08:46:45
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fs
{
	public class AudioPlayer : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// 类型
        /// </summary>
        public enum AudioType
        {
            /// <summary>
            /// 音效
            /// </summary>
            Effect,
            /// <summary>
            /// 旁边
            /// </summary>
            Subtitle,
            /// <summary>
            /// 背景音乐
            /// </summary>
            BG,
        }
        /// <summary>
        /// 触发方式
        /// </summary>
        public enum Trigger
        {
            /// <summary>
            /// 点击
            /// </summary>
            Click,
            /// <summary>
            /// 激活
            /// </summary>
            Enable,
            /// <summary>
            /// 消失
            /// </summary>
            Disable,
        }
        public AudioType type = AudioType.Effect;
        public Trigger trigger = Trigger.Enable;
        public int loopCount = 1;
        public float delayTime = 0;
        public AudioClip audioClip;

        void Awake()
        {
        }

        void OnEnable()
        {
            if (trigger == Trigger.Enable)
            {
                PlaySound();
            }
        }

        void OnDisable()
        {
            if (trigger == Trigger.Disable)
            {
                PlaySound();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (trigger == Trigger.Click)
            {
                PlaySound();
            }
        }
        /// <summary>
        /// 外部调用
        /// </summary>
        public void Play()
        {
            PlaySound();
        }

        public void PlaySound()
        {
            if (audioClip == null) return;

            if (delayTime > 0)
            {
                Timer.instance.AddOnce(delayTime, PlayImpl);
            }
            else
            {
                PlayImpl();
            }
        }
        private void PlayImpl()
        {
            if (audioClip == null) return;
            //Debug.LogWarning(audioClip.name);
            switch (type)
            {
                case AudioType.Effect:
                    AudioTool.Instance.Play(audioClip.name);
                    break;
                case AudioType.BG:
                    AudioTool.Instance.PlayBgMusic(audioClip.name);
                    break;
                case AudioType.Subtitle:
                    AudioTool.Instance.PlaySubtitle(audioClip.name);
                    break;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AudioPlayer))]
    public class UIAudioPlayerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("Type：音乐类型(Effect:音效，Subtitle:旁边，BG:背景音乐)", MessageType.Info);
            EditorGUILayout.HelpBox("Trigger：触发方式(Click:点击，Enable:激活，Disable:消失)", MessageType.Info);
            EditorGUILayout.HelpBox("LoopCount：循环次数", MessageType.Info);
            EditorGUILayout.HelpBox("DelayTime：延迟时间(s)", MessageType.Info);
            EditorGUILayout.HelpBox("AudioClip：音乐文件", MessageType.Info);
        }
    }
#endif
}
