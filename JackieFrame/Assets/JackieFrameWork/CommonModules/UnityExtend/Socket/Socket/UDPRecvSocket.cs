﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace fs
{
    public class UDPRecvSocket
    {
        private UdpClient m_socket = null;
        private object m_sync_lock = new object();
                
        /// <summary>
        /// 接收到客户端的数据
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="count"></param>
        public delegate void OnReceiveData(byte[] buf, int len);
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="conn_idx"></param>
        public delegate void OnConnectClose();
        public event OnReceiveData OnMessage;
        public event OnConnectClose OnClose;

        public bool Init(ushort port)
        {
            if (m_socket != null)
            {
                Debuger.LogError("UDPRecvSocket - 已经初始化过socket");
                return false;
            }
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);
                m_socket = new UdpClient(RemoteIpEndPoint);
                m_socket.Client.ReceiveBufferSize = 1024 * 1024;
                m_socket.Client.SendBufferSize = 1024 * 1024;
                m_socket.Client.SendTimeout = 5000;
                m_socket.Client.ReceiveTimeout = 5000;
                Debuger.Log("UDPRecvSocket - 创建UDPRecvSocket");

                UdpState s = new UdpState(m_socket, RemoteIpEndPoint);
                m_socket.BeginReceive(OnReceive, s);
            }
            catch(Exception e)
            {
                Debuger.LogException(e);
                return false;
            }
            return true;
        }
        public void Close()
        {
            OnClose = null;
            if (m_socket != null)
            {
                try
                {
                    m_socket.Close();
                }
                catch (Exception) { }
                m_socket = null;
            }
        }
        private void OnReceive(IAsyncResult ar)
        {
            if (m_socket == null) return;
            try
            {
                lock (m_sync_lock)
                {
                    UdpState s = ar.AsyncState as UdpState;
                    if(s != null)
                    {
                        UdpClient socket = s.UdpClient;

                        IPEndPoint ip = s.IP;
                        Byte[] buf = socket.EndReceive(ar, ref ip);
                        if (OnMessage != null) OnMessage.Invoke(buf, buf.Length);

                        //在这里重新开始一个异步接收，用于处理下一个网络请求
                        socket.BeginReceive(OnReceive, s);
                    }
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode != 10054) Debuger.LogException(e);
                if (OnClose != null)
                {
                    OnClose.Invoke();
                }
                this.Close();
            }
            catch (Exception e)
            {
                Debuger.LogException(e);
                if (OnClose != null)
                {
                    OnClose.Invoke();
                }
                this.Close();
                return;
            }
        }
        class UdpState
        {
            private UdpClient udpclient = null;
            public UdpClient UdpClient
            {
                get { return udpclient; }
            }

            private IPEndPoint ip;
            public IPEndPoint IP
            {
                get { return ip; }
            }

            public UdpState(UdpClient udpclient, IPEndPoint ip)
            {
                this.udpclient = udpclient;
                this.ip = ip;
            }
        }
    }
}
