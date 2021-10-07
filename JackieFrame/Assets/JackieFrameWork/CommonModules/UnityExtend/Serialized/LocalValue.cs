using UnityEngine;
using System.Collections;
using AntiCheat.ObscuredTypes;

namespace fs
{
    /// <summary>
    /// 本地数据
    /// @author hannibal
    /// @time 2014-12-19
    /// </summary>
    public class LocalValue
    {
        private static string m_Account = string.Empty;
        /// <summary>
        /// 设置账户，防止不同账号数据覆盖
        /// </summary>
        public static void SetAccount(string account)
        {
            m_Account = account;
        }
        private static string GetKey(string key)
        {
            return m_Account + key;
        }
        /// <summary>
        /// 泛型存取
        /// </summary>
        /// <param name="key">Key.键</param>
        /// <param name="value">Value.值</param>
        public static void SetValue<T>(string key, T value)
        {
            key = GetKey(key);

            if (typeof(T) == typeof(int))
            {
                ObscuredPrefs.SetInt(key, (int)(object)value);
            }
            else if (typeof(T) == typeof(long))
            {
                ObscuredPrefs.SetLong(key, (long)(object)value);
            }
            else if (typeof(T) == typeof(string))
            {
                ObscuredPrefs.SetString(key, (string)(object)value);
            }
            else if (typeof(T) == typeof(float))
            {
                ObscuredPrefs.SetFloat(key, (float)(object)value);
            }
            else
            {
                Debuger.LogError("SetValue : type error:" + typeof(T));
            }
        }
        public static T GetValue<T>(string key, T defaultVaule)
        {
            key = GetKey(key);

            if (typeof(T) == typeof(int))
            {
                return (T)(object)ObscuredPrefs.GetInt(key, (int)(object)defaultVaule);
            }
            else if (typeof(T) == typeof(long))
            {
                return (T)(object)ObscuredPrefs.GetLong(key, (long)(object)defaultVaule);
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)ObscuredPrefs.GetString(key, (string)(object)defaultVaule);
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)(object)ObscuredPrefs.GetFloat(key, (float)(object)defaultVaule);
            }
            else
            {
                Debuger.LogError("GetValue : type error:" + typeof(T));
                return (T)(object)0;
            }
        }

        public static void DeleteKey(string key)
        {
            key = GetKey(key);
            ObscuredPrefs.DeleteKey(key);
        }

        public static void DeleteAll()
        {
            ObscuredPrefs.DeleteAll();
        }

        public static bool HasKey(string key)
        {
            key = GetKey(key);
            return ObscuredPrefs.HasKey(key);
        }
    }
}