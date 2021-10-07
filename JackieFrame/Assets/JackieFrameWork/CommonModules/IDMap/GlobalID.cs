
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 全局配置
    /// @author hannibal
    /// @time 2014-11-11
    /// </summary>
    public class GlobalID
    {
        private static eLanguageType m_LanguageType = eLanguageType.zh_cn;
        public static eLanguageType LanguageType
        {
            get
            {
                return m_LanguageType;
            }
            set
            {
                m_LanguageType = value;
                AudioTool.Instance.GameLanguage = (int)value;
                EventDispatcher.TriggerEvent(GlobalEvent.SET_LANGUAGE);
            }
        }

        /// <summary>
        /// 应用是否已经退出
        /// </summary>
        [DefaultValue(false)]
        public static bool IsApplicationQuit { get; set; }
    }

    public class GlobalEvent
    {
        /// <summary>
        /// 更改语言
        /// </summary>
        public const string SET_LANGUAGE = "SET_LANGUAGE";
        /// <summary>
        /// 更改音量
        /// </summary>
        public const string AUDIO_VOLUME = "AUDIO_VOLUME";
    }
}