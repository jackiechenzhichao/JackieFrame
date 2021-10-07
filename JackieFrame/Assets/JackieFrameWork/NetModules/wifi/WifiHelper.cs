
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：wifi
// 创建时间：2020-12-24 17:08:10
//=======================================================
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
    public enum eWifiState
    {
        Connected,
        Connecting,
        Disconnect,
    }
	public class WifiHelper : Singleton<WifiHelper>
    {
        private NativeWifi.WlanClient.WlanInterface m_WifiInstance = null;
        private List<NativeWifi.Wlan.WlanAvailableNetwork> m_ListWifi = new List<NativeWifi.Wlan.WlanAvailableNetwork>();
        System.Action<eWifiState> m_Callback = null;
        
        public WifiHelper()
        {
            try
            {
                NativeWifi.WlanClient client = new NativeWifi.WlanClient();
                if (client.Interfaces != null && client.Interfaces.Length > 0)
                {
                    m_WifiInstance = client.Interfaces[0];//取第一个网卡
                    m_WifiInstance.WlanConnectionNotification += OnWlanConnectionNotification;
                }
                else
                {
                    Debuger.Log("没有找到可用wifi模块");
                }
            }
            catch (System.Exception e)
            {
                Debuger.Log("初始化wifi模块失败:" + e.ToString());
            }
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="callback"></param>
        public void Connect(string name, string password, System.Action<eWifiState> callback)
        {
            Debuger.Log("请求连接wifi:" + name);
            NativeWifi.Wlan.WlanAvailableNetwork network;
            if(!TryGetNetwork(name, out network))
            {
                if (callback != null) callback(eWifiState.Disconnect);
                return;
            }
            m_Callback = callback;
            try
            {
                if (network.securityEnabled && !NativeWifi.WlanHelper.HasProfile(m_WifiInstance, NativeWifi.WlanHelper.GetStringForSSID(network.dot11Ssid)))
                {
                    NativeWifi.WlanHelper.ConnetWifi(m_WifiInstance, network, password);
                }
                else
                {
                    NativeWifi.WlanHelper.ConnetWifi(m_WifiInstance, network);
                }
            }
            catch (System.Exception ex)
            {
                Debuger.LogException("wifi连接失败，确保密码输入正确:" + ex.ToString());
            }
        }
        /// <summary>
        /// wifi列表
        /// </summary>
        public List<string> Refresh()
        {
            if (m_WifiInstance == null)
            {
                return new List<string>();
            }
            m_ListWifi.Clear();
            List<string> list = new List<string>();
            NativeWifi.Wlan.WlanAvailableNetwork[] networks = m_WifiInstance.GetAvailableNetworkList(0);
            foreach (NativeWifi.Wlan.WlanAvailableNetwork network in networks)
            {
                string SSID = NativeWifi.WlanHelper.GetStringForSSID(network.dot11Ssid);
                if ((network.flags & NativeWifi.Wlan.WlanAvailableNetworkFlags.Connected) != 0)
                {
                    Debuger.Log("wifi已连接到:" + SSID);
                }
                //如果有配置文件的SSID会重复出现。过滤掉
                if (!string.IsNullOrEmpty(SSID) && !list.Contains(SSID))
                {
                    list.Add(SSID);
                    m_ListWifi.Add(network);
                }
            }

            //信号强度排序
            m_ListWifi.Sort(delegate (NativeWifi.Wlan.WlanAvailableNetwork a, NativeWifi.Wlan.WlanAvailableNetwork b)
            {
                return b.wlanSignalQuality.CompareTo(a.wlanSignalQuality);
            });

            //返回信息
            List<string> wifi_states = new List<string>();
            m_ListWifi.ForEach((NativeWifi.Wlan.WlanAvailableNetwork network) => 
            {
                string name = NativeWifi.WlanHelper.GetStringForSSID(network.dot11Ssid);
                Debuger.Log(string.Format("识别到wifi:{0},状态:{1}", name, network.flags));
                wifi_states.Add(name);
            });
            return wifi_states;
        }
        public eWifiState GetState(string name)
        {
            eWifiState state = eWifiState.Disconnect;
            NativeWifi.Wlan.WlanAvailableNetwork network;
            if(TryGetNetwork(name, out network))
            {
                state = ((network.flags & NativeWifi.Wlan.WlanAvailableNetworkFlags.Connected) != 0) ? eWifiState.Connected : eWifiState.Disconnect;
            }
            return state;
        }
        private bool TryGetNetwork(string name, out NativeWifi.Wlan.WlanAvailableNetwork network)
        {
            foreach(var net in m_ListWifi)
            {
                string SSID = NativeWifi.WlanHelper.GetStringForSSID(net.dot11Ssid);
                if (SSID == name)
                {
                    network = net;
                    return true;
                }
            }
            network = default(NativeWifi.Wlan.WlanAvailableNetwork);
            return false;
        }
        private void OnWlanConnectionNotification(NativeWifi.Wlan.WlanNotificationData notifyData, NativeWifi.Wlan.WlanConnectionNotificationData connNotifyData)
        {
            if (notifyData.notificationSource == NativeWifi.Wlan.WlanNotificationSource.MSM)
            {
                //这里是完成连接
                if ((NativeWifi.Wlan.WlanNotificationCodeMsm)notifyData.NotificationCode == NativeWifi.Wlan.WlanNotificationCodeMsm.Connected)
                {
                    Debuger.Log("wifi连接到:" + connNotifyData.profileName);
                    if (m_Callback != null) m_Callback(eWifiState.Connected);
                }
            }
            else if (notifyData.notificationSource == NativeWifi.Wlan.WlanNotificationSource.ACM)
            {
                //连接失败
                if ((NativeWifi.Wlan.WlanNotificationCodeAcm)notifyData.NotificationCode == NativeWifi.Wlan.WlanNotificationCodeAcm.ConnectionAttemptFail)
                {
                    Debuger.Log("wifi连接失败，请检查密码是否正确");
                    m_WifiInstance.DeleteProfile(connNotifyData.profileName);
                    if (m_Callback != null) m_Callback(eWifiState.Disconnect);
                }
                if ((NativeWifi.Wlan.WlanNotificationCodeAcm)notifyData.NotificationCode == NativeWifi.Wlan.WlanNotificationCodeAcm.Disconnected)
                {
                    Debuger.Log("wifi未连接1");
                    if (m_Callback != null) m_Callback(eWifiState.Disconnect);
                }
                if ((NativeWifi.Wlan.WlanNotificationCodeAcm)notifyData.NotificationCode == NativeWifi.Wlan.WlanNotificationCodeAcm.Disconnecting)
                {
                    Debuger.Log("wifi未连接2");
                }
                if ((NativeWifi.Wlan.WlanNotificationCodeAcm)notifyData.NotificationCode == NativeWifi.Wlan.WlanNotificationCodeAcm.ConnectionStart)
                {
                    Debuger.Log("wifi连接中…");
                    if (m_Callback != null) m_Callback(eWifiState.Connecting);
                }
            }
        }
    }
}
#endif