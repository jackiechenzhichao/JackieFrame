#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 注册表
    /// </summary>
    public class PCPlayerPrefs
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

        static PCPlayerPrefs()
        {
            //SetPlayerId("222");
            GameGlobalData game_data = GameGlobalData.Get();
            if (game_data != null)
            {
                m_player_id = game_data.PackageName;
            }

            Debuger.LogError("PCPlayerPrefs() ggggggg");
        }
        
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
        /// <summary>
        /// 
        /// </summary>
        public static void SetPlayerId(string id)
        {
            GameGlobalData game_data = GameGlobalData.Get();
            m_player_id = game_data.PackageName+ "_"+id;

            Debuger.LogError("SetPlayerId(string id) ggggggg");
        }
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
                Debuger.LogError("获得GetRootKey失败");
                return null;
            }
        }
        public static bool SetInt(string key, int value)
        {
            if (string.IsNullOrEmpty(key)) return false;

            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return false;
            try
            {
                reg_key.SetValue(key, value);
            }
            catch(System.Exception)
            {
                Debuger.LogError("设置失败:" + key);
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
        }
        public static bool SetFloat(string key, float value)
        {
            if (string.IsNullOrEmpty(key)) return false;

            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return false;
            try
            {
                reg_key.SetValue(key, value);
            }
            catch (System.Exception)
            {
                Debuger.LogError("设置失败:" + key);
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
        }
        public static bool SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return false;

            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return false;
            try
            {
                reg_key.SetValue(key, value);
            }
            catch (System.Exception)
            {
                Debuger.LogError("设置失败:" + key);
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
        }

        public static int GetInt(string key, int default_value = 0)
        {
            if (string.IsNullOrEmpty(key)) return default_value;

            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return default_value;
            try
            {
                return (int)reg_key.GetValue(key);
            }
            catch (System.Exception)
            {
                Debuger.LogError("获取失败:" + key);
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
        }
        public static float GetFloat(string key, float default_value = 0.0f)
        {
            if (string.IsNullOrEmpty(key)) return default_value;

            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return default_value;
            try
            {
                return (float)reg_key.GetValue(key);
            }
            catch (System.Exception)
            {
                Debuger.LogError("获取失败:" + key);
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
        }
        public static string GetString(string key, string default_value = "")
        {
            if (string.IsNullOrEmpty(key)) return default_value;

            RegistryKey reg_key = GetRootKey();
            if (reg_key == null) return default_value;
            try
            {
                return (string)reg_key.GetValue(key, default_value);
            }
            catch (System.Exception)
            {
                Debuger.LogError("获取失败:" + key);
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
        }
    }
}
#endif