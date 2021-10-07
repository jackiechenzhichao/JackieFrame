
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：web下载，封装UnityWebRequest
// 返回码：
// 1xx 处理信息，服务器收到请求，需要请求者继续执行操作；
// 2xx 请求成功，操作被成功接收并处理；
// 3xx 重定向，需要进一步的操作以完成请求；
// 4xx 客户端错误，请求包含语法错误或无法完成请求；
// 5xx 服务器错误，服务器在处理请求的过程中发生了错误；
// 创建时间：2019-12-26 14:46:18
//=======================================================
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace fs
{
    public class WebRequest : DnotMonoSingleton<WebRequest>
    {
        #region  下载接口
        /// <summary>
        /// 下载文本
        /// </summary>
        /// <param name="url">请求的链接</param>
        /// <param name="action">发生的事件</param>
        /// <param name="method">请求的方式</param>
        /// <returns></returns>
        public static bool DownloadText(string url, WebRequestTextEvent action, string method = UnityWebRequest.kHttpVerbGET)
        {
            if (IsNull(url, action, action.complete)) return true;
            WebRequest.instance.StartCoroutine(WebRequest.instance.DownloadTextIEnumerator(url, method, action));
            return false;
        }

        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="url">请求的链接</param>
        /// <param name="action">发生的事件</param>
        /// <param name="method">请求的方式</param>
        /// <returns></returns>
        public static bool DownloadData(string url, WebRequestDataEvent action, string method = UnityWebRequest.kHttpVerbGET)
        {
            if (IsNull(url, action, action.complete)) return true;
            WebRequest.instance.StartCoroutine(WebRequest.instance.DownloadDataIEnumerator(url, method, action));
            return false;
        }

        /// <summary>
        /// 下载文件
        /// 1.文件存放位置不能重复
        /// 2.不能出现中文文件名
        /// </summary>
        /// <param name="url">请求的链接</param>
        /// <param name="savePath">保存的路径</param>
        /// <param name="action">发生的事件</param>
        /// <param name="method">请求的方式</param>
        /// <returns></returns>
        public static bool DownloadFile(string url, string savePath, WebRequestFileEvent action, string method = UnityWebRequest.kHttpVerbGET)
        {
            if (string.IsNullOrEmpty(url) || action == null) return true;
            WebRequest.instance.StartCoroutine(WebRequest.instance.DownloadFileIEnumerator(url, savePath, method, action));
            return false;
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url">请求的链接</param>
        /// <param name="action">发生的事件</param>
        /// <param name="method">请求的方式</param>
        /// <returns></returns>
        public static bool DownaloadSprite(string url, WebRequestSpriteEvent action, string method = UnityWebRequest.kHttpVerbGET)
        {
            if (IsNull(url, action, action.complete)) return true;
            WebRequest.instance.StartCoroutine(WebRequest.instance.DownloadSpriteIEnumerator(url, method, action));
            return false;
        }

        /// <summary>
        /// 下载AssetBundle
        /// </summary>
        /// <param name="url">请求的链接</param>
        /// <param name="action">发生的事件</param>
        /// <param name="method">请求的方式</param>
        /// <returns></returns>
        public static bool DownaloadAssetBundle(string url, WebRequestAssetBundleEvent action, string method = UnityWebRequest.kHttpVerbGET)
        {
            if (IsNull(url, action, action.complete)) return true;
            WebRequest.instance.StartCoroutine(WebRequest.instance.DownloadAssetBundleIEnumerator(url, method, action));
            return false;
        }

        /// <summary>
        /// 下载音频
        /// </summary>
        /// <param name="url">请求的链接</param>
        /// <param name="audioType">音频格式</param>
        /// <param name="action">发生的事件</param>
        /// <returns></returns>
        public static bool DownloadAudioClip(string url, WebRequestAudioClipEvent action, AudioType audioType = AudioType.AUDIOQUEUE)
        {
            if (IsNull(url, action, action.complete)) return true;
            WebRequest.instance.StartCoroutine(WebRequest.instance.DownloadAudioClipIEnumerator(url, audioType, action));
            return false;
        }
        #endregion

        IEnumerator DownloadTextIEnumerator(string url, string method, WebRequestTextEvent action)
        {
            var request = new UnityWebRequest(url, method);
            request.downloadHandler = new DownloadHandlerBuffer();
            if (action.progress != null) StartCoroutine(DownloadProgress(request, action.progress));
            yield return request.SendWebRequest();
            Dispose(request, () =>
            {
                action.complete(request.downloadHandler.text);
            }, action.error);
        }

        IEnumerator DownloadDataIEnumerator(string url, string method, WebRequestDataEvent action)
        {
            var request = new UnityWebRequest(url, method);
            request.downloadHandler = new DownloadHandlerBuffer();
            if (action.progress != null) StartCoroutine(DownloadProgress(request, action.progress));
            yield return request.SendWebRequest();
            Dispose(request, () =>
            {
                action.complete(request.downloadHandler.data);
            }, action.error);
        }

        IEnumerator DownloadFileIEnumerator(string url, string savePath, string method, WebRequestFileEvent action)
        {
            //需要先删除已有文件
            if (!File.Exists(savePath))
            {
                if(!FileUtils.DeleteFile(savePath))
                {
                    if (action.error != null) action.error(0, "删除目录文件失败:" + savePath);
                    yield return null;
                }
            }

            var request = new UnityWebRequest(url, method);
            request.downloadHandler = new DownloadHandlerFile(savePath);
            if (action.progress != null) StartCoroutine(DownloadProgress(request, action.progress));
            yield return request.SendWebRequest();
            Dispose(request, () =>
            {
                if (action != null) action.complete();
            }, action.error);
        }

        IEnumerator DownloadSpriteIEnumerator(string url, string method, WebRequestSpriteEvent action)
        {
            var request = new UnityWebRequest(url, method);
            var texDl = new DownloadHandlerTexture(true);
            request.downloadHandler = texDl;
            if (action.progress != null) StartCoroutine(DownloadProgress(request, action.progress));

            yield return request.SendWebRequest();
            Dispose(request, () =>
            {
                Texture2D t = texDl.texture;
                action.complete(Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero, 1f), texDl.data);
            }, action.error);
        }

        IEnumerator DownloadAssetBundleIEnumerator(string url, string method, WebRequestAssetBundleEvent action)
        {
            var request = new UnityWebRequest(url, method);
            var handler = new DownloadHandlerAssetBundle(request.url, uint.MaxValue);
            request.downloadHandler = handler;
            if (action.progress != null) StartCoroutine(DownloadProgress(request, action.progress));

            yield return request.SendWebRequest();
            Dispose(request, () =>
            {
                action.complete(handler.assetBundle);
            }, action.error);
        }

        IEnumerator DownloadAudioClipIEnumerator(string url, AudioType audioType, WebRequestAudioClipEvent action)
        {
            var request = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            if (action.progress != null) StartCoroutine(DownloadProgress(request, action.progress));

            yield return request.SendWebRequest();

            Dispose(request, () =>
            {
                action.complete(DownloadHandlerAudioClip.GetContent(request));
            }, action.error);
        }

        IEnumerator DownloadProgress(UnityWebRequest request, Action<float> action)
        {
            while (!request.isDone)
            {
                yield return null;
                action(request.downloadProgress);
            }
            request.Abort();
        }

        private void Dispose(UnityWebRequest request, Action ok, Action<long, string> error)
        {
            Debuger.Log(string.Format("url [ {0} ], code [ {1} ], error [ {2} ]", request.url, request.responseCode, request.error));
            if (request.isHttpError || request.isNetworkError)
            {
                if (error != null) error(request.responseCode, request.error);
            }
            else if(request.responseCode >= 400 && request.responseCode < 500)
            {
                if (error != null) error(request.responseCode, request.error);
            }
            else
            {
                if (ok != null) ok();
            }
        }

        private static bool IsNull(string url, BaseWebRequestEvent requestAction, object action)
        {
            if (string.IsNullOrEmpty(url) || requestAction == null || action == null)
            {
                Debuger.Log("Can't be empty");
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 下载事件基类
    /// </summary>
    public class BaseWebRequestEvent
    {
        /// <summary>
        /// 下载进度
        /// </summary>
        public Action<float> progress;
        /// <summary>
        /// 下载错误
        /// </summary>
        public Action<long, string> error;
    }

    /// <summary>
    /// 文本请求事件
    /// </summary>
    public class WebRequestTextEvent : BaseWebRequestEvent
    {
        public Action<string> complete;
    }

    /// <summary>
    /// 数据请求事件
    /// </summary>
    public class WebRequestDataEvent : BaseWebRequestEvent
    {
        public Action<byte[]> complete;
    }

    /// <summary>
    /// 文件请求事件
    /// </summary>
    public class WebRequestFileEvent : BaseWebRequestEvent
    {
        public Action complete;
    }

    /// <summary>
    /// 图片请求事件
    /// </summary>
    public class WebRequestSpriteEvent : BaseWebRequestEvent
    {
        public Action<Sprite, byte[]> complete;
    }

    /// <summary>
    /// 文本请求事件
    /// </summary>
    public class WebRequestAssetBundleEvent : BaseWebRequestEvent
    {
        public Action<AssetBundle> complete;
    }

    /// <summary>
    /// 音频请求事件
    /// </summary>
    public class WebRequestAudioClipEvent : BaseWebRequestEvent
    {
        public Action<AudioClip> complete;
    }
}