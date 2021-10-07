#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using Microsoft.Win32;
#else
using UnityEngine;
#endif
using System;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 注册表
    /// </summary>
    public class MyPlayerPrefs
    {
        /// <summary>
        /// 写入路径
        /// </summary>
        private const string KEY_PATH = @"SOFTWARE\funshare";
        private static string m_cur_path = KEY_PATH;

        /// <summary>
        /// 玩家id，默认使用游戏名称
        /// </summary>
        private static string m_player_id = string.Empty;
        
        /// <summary>
        /// 修改保存路径
        /// </summary>
        /// <param name="path"></param>
        public static void SetDefaultPath(string path)
        {
            m_cur_path = path;
        }
        public static void RestoreDefaultPath()
        {
            m_cur_path = KEY_PATH;
        }
        static MyPlayerPrefs()
        {
            GameGlobalData game_data = GameGlobalData.Get();
            if (game_data != null)
            {
                m_player_id = game_data.PackageName;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void SetPlayerId(string id)
        {
            m_player_id = id;
        }
        public static string GetPlayerId()
        {
            return m_player_id;
        }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        private static RegistryKey GetRootKey()
        {
            try
            {
                string path = m_cur_path;
                if(!string.IsNullOrEmpty(m_player_id)) path = m_cur_path + "\\" + m_player_id;
                RegistryKey user_machine = Registry.CurrentUser;
                RegistryKey reg_key = user_machine.OpenSubKey(path, true);
                if(reg_key == null)
                {
                    reg_key = user_machine.CreateSubKey(path);
                }
                return reg_key;
            }
            catch(System.Exception)
            {
                return null;
            }

        }

        private static string GetRootKey(string key)
        {
            if (!string.IsNullOrEmpty(m_player_id)) key = m_player_id + "_" + key;
            return key;
        }
#endif
        public static bool SetInt(string key, int value)
        {
            if (string.IsNullOrEmpty(key)) return false;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return false;
            try
            {
                reg_key.SetValue(key, value);
            }
            catch(System.Exception)
            {
                return false;
            }
            finally
            {
                if (reg_key != null)
                {
                    reg_key.Flush();
                    reg_key.Close();
                }
            }
            return true;
#else
            PlayerPrefs.SetInt(GetRootKey(key), value);
            return true;
#endif
        }
        public static bool SetFloat(string key, float value)
        {
            if (string.IsNullOrEmpty(key)) return false;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return false;
            try
            {
                reg_key.SetValue(key, value);
            }
            catch (System.Exception)
            {
                return false;
            }
            finally
            {
                if (reg_key != null)
                {
                    reg_key.Flush();
                    reg_key.Close();
                }
            }
            return true;
#else
            PlayerPrefs.SetFloat(GetRootKey(key), value);
            return true;
#endif
        }
        public static bool SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return false;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return false;
            try
            {
                reg_key.SetValue(key, value);
            }
            catch (System.Exception)
            {
                return false;
            }
            finally
            {
                if (reg_key != null)
                {
                    reg_key.Flush();
                    reg_key.Close();
                }
            }
            return true;
#else
            PlayerPrefs.SetString(GetRootKey(key), value);
            return true;
#endif
        }

        public static int GetInt(string key, int default_value = 0)
        {
            if (string.IsNullOrEmpty(key)) return default_value;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return default_value;
            try
            {
                return (int)reg_key.GetValue(key);
            }
            catch (System.Exception)
            {
                return default_value;
            }
            finally
            {
                if (reg_key != null)
                {
                    reg_key.Flush();
                    reg_key.Close();
                }
            }
#else
            return PlayerPrefs.GetInt(GetRootKey(key), default_value);
#endif
        }
        public static float GetFloat(string key, float default_value = 0.0f)
        {
            if (string.IsNullOrEmpty(key)) return default_value;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return default_value;
            try
            {
                return (float)reg_key.GetValue(key);
            }
            catch (System.Exception)
            {
                return default_value;
            }
            finally
            {
                if (reg_key != null)
                {
                    reg_key.Flush();
                    reg_key.Close();
                }
            }
#else
            return PlayerPrefs.GetFloat(GetRootKey(key), default_value);
#endif
        }
        public static string GetString(string key, string default_value = "")
        {
            if (string.IsNullOrEmpty(key)) return default_value;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return default_value;
            try
            {
                return (string)reg_key.GetValue(key, default_value);
            }
            catch (System.Exception)
            {
                return default_value;
            }
            finally
            {
                if (reg_key != null)
                {
                    reg_key.Flush();
                    reg_key.Close();
                }
            }
#else
            return PlayerPrefs.GetString(GetRootKey(key), default_value);
#endif
        }
    }
}