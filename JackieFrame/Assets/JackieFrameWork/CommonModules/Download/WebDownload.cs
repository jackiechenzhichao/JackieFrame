using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace fs
{
    /// <summary>
    /// web下载
    /// 1.针对http服务器，ftp服务器不要用
    /// 2.如果设置了身份认证，需要填充WebClient.Credentials
    /// 3.IIS下载apk如果报(404-找不到文件或目录)。需要在iis中配置网站的MIME类型，添加上MIME类型：扩展名APK，MIME类型application/vnd.android.package-archive
    /// </summary>
    public class WebDownload
    {
        private static WebTimeout m_timeout = new WebTimeout();
        /// <summary>
        /// 添加新下载项：异步方式
        /// </summary>
        public static void DownloadFileAsync(WebDownloadEvent evt)
        {
            try
            {
                //删除旧的
                FileUtils.DeleteFile(evt.path);

                using (WebClient client = new WebClient())
                {
                    if (!string.IsNullOrEmpty(evt.username)) client.Credentials = new System.Net.NetworkCredential(evt.username, evt.password);
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadCompleted);
                    client.DownloadFileAsync(new System.Uri(evt.url), evt.path, evt);

                    //超时处理
                    m_timeout.Start(evt.timeout, () =>
                    {
                        if (evt != null && evt.error != null)
                        {//防止多次调用
                            System.Action<string> fun = evt.error;
                            evt.error = null;
                            fun(evt.url);
                        }
                        if(client != null)
                        {
                            Debuger.LogError("等待超时，取消下载");
                            client.CancelAsync();
                        }
                    });
                }
            }
            catch(System.Exception e)
            {
                m_timeout.Stop();
                Debuger.LogError("创建下载链接失败:" + e.ToString());
                if (evt != null && evt.error != null)
                {//防止多次调用
                    System.Action<string> fun = evt.error;
                    evt.error = null;
                    fun(evt.url);
                }
            }
        }
        /// <summary>
        /// 添加新下载项：同步方式
        /// </summary>
        public static bool DownloadFileSync(WebDownloadEvent evt)
        {
            try
            {
                //删除旧的
                FileUtils.DeleteFile(evt.path);

                using (WebClient client = new WebClient())
                {
                    if(!string.IsNullOrEmpty(evt.username)) client.Credentials = new System.Net.NetworkCredential(evt.username, evt.password);
                    client.DownloadFile(new System.Uri(evt.url), evt.path);
                    if (evt.complete != null) evt.complete(evt.url, evt.path);
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debuger.LogError("下载失败:" + e.ToString());
                if (evt.error != null)
                {//防止多次调用
                    System.Action<string> fun = evt.error;
                    evt.error = null;
                    fun(evt.url);
                }
                return false;
            }
        }

        /// <summary>
        /// 下载一项
        /// </summary>
        private static void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            m_timeout.Stop();

            WebDownloadEvent evt = e.UserState as WebDownloadEvent;
            if(evt == null)
            {
                Debuger.LogError("内部错误发生在OnDownloadCompleted");
                return;
            }
            if (e.Error == null && e.Cancelled == false)
            {
                if (evt.complete != null)
                {//防止多次调用
                    System.Action<string, string> fun = evt.complete;
                    evt.complete = null;
                    fun(evt.url, evt.path);
                }
            }
            else
            {
                if (e.Error != null) Debuger.LogError("下载失败，原因:" + e.Error.ToString());
                if (evt.error != null)
                {//防止多次调用
                    System.Action<string> fun = evt.error;
                    evt.error = null;
                    fun(evt.url);
                }
            }
        }

        /// <summary>
        /// 下载进度改变
        /// </summary>
        private static void OnProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            m_timeout.Reset();

            WebDownloadEvent evt = e.UserState as WebDownloadEvent;
            if (evt == null)
            {
                Debuger.LogError("内部错误发生在OnProgressChanged");
                return;
            }
            if (evt.progress != null) evt.progress(evt.url, e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
        }
    }

    /// <summary>
    /// 下载项
    /// </summary>
    public class WebDownloadEvent
    {
        /// <summary>
        /// 下载的路径
        /// </summary>
        public string url;
        /// <summary>
        /// 存放的路径
        /// </summary>
        public string path;

        /// <summary>
        /// 超时等待时长(s)
        /// </summary>
        public int timeout = 60;

        /// <summary>
        /// 身份信息
        /// </summary>
        public string username = string.Empty;
        public string password = string.Empty;

        /// <summary>
        /// 下载进度，参数1:进度[0,100]；参数2:已下载字节；参数3:总共需要下载字节
        /// </summary>
        public System.Action<string, int, long, long> progress;
        /// <summary>
        /// 下载错误(参数1：需要下载的ur)
        /// </summary>
        public System.Action<string> error;
        /// <summary>
        /// 下载完成(参数1：需要下载的ur)
        /// </summary>
        public System.Action<string, string> complete;
    }

}
