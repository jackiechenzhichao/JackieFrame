using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// http上报数据
    /// @author hannibal
    /// @time 2016-4-5
    /// </summary>
    public class HttpClient : MonoBehaviour
    {
        public string m_URL;

        void Awake()
        {
            m_instance = this;
        }
        private static HttpClient m_instance;
        public static HttpClient Instance
        {
            get { return m_instance; }
        }

        public void PostData(Dictionary<string, string> postDatas, System.Action<string> onPostDoneAction)
        {
            if (string.IsNullOrEmpty(m_URL))
            {
                Debuger.LogError("HttpClient.PostData, url为空");
                return;
            }

            if (!m_URL.StartsWith("http") && m_URL.StartsWith("https"))
            {
                Debuger.LogError(string.Format("HttpClient.PostData, url地址格式不正确, {0}", m_URL));
                return;
            }

            WWWForm dataForm = new WWWForm();
            if (postDatas != null)
            {
                foreach (var postData in postDatas)
                {
                    dataForm.AddField(postData.Key, postData.Value);
                }
            }

            StartCoroutine(AsyncPostData(m_URL, dataForm, onPostDoneAction));
        }

        private IEnumerator AsyncPostData(string url, WWWForm dataForm, System.Action<string> onPostDoneAction)
        {
            Debuger.Log("start post data");
            using (WWW www = new WWW(url, dataForm))
            {
                yield return www;
                Debuger.Log("end post data");
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debuger.LogError("AsyncPostData error:" + www.error);
                }
                else
                {
                    Debuger.Log("recv data:" + www.text);
                    if (onPostDoneAction != null)
                        onPostDoneAction(www.text);
                }
            }
        }
    }
}