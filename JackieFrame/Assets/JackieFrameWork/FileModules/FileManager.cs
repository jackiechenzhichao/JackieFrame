using UnityEngine;
using System.IO;
using JsonFx.Json;
using System.Xml;
using System.Collections;

namespace fs
{
    /// <summary>
    /// 读写文件
    /// @author hannibal
    /// @time 2019-7-8
    /// </summary>
    public class FileManager : DnotMonoSingleton<FileManager>
    {
        #region json文件序列化
        /// <summary>
        /// 读文件，转化成object
        /// 1.读文件在主线程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">文件路径</param>
        public T ReadJsonObject<T>(string path, bool register = false) where T : new()
        {
            T obj = default(T);
            try
            {
                bool is_reg = false;
                string text = "";
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                if (register)
                {
                    is_reg = true;
                    string file_name = Path.GetFileNameWithoutExtension(path);
                    Debuger.Log("从注册表读取:" + file_name);
                    text = MyPlayerPrefs.GetString(file_name);
                    //Debuger.LogError("read: llllll"); 
                }
#endif
                if (!is_reg)
                {
                    Debuger.Log("从文件读取:" + path);
                    if (!File.Exists(path))
                    {
                        Debuger.LogWarning("目录不存在:" + path);
                        return default(T);
                    }
                    text = FileUtils.ReadFileText(path);
                }
                Debuger.Log("读取内容:" + text);
                obj = JsonReader.Deserialize<T>(text);
            }
            catch (System.Exception e)
            {
                Debuger.LogException(e);
                return default(T);
            }

            Debuger.Log("读取完毕:" + (obj == null ? "失败" : "成功"));
            return obj;
        }
        /// <summary>
        /// 写文件
        /// 1.异步方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">写入路径</param>
        /// <param name="data">写入数据</param>
        /// <param name="sync">是否同步</param>
        /// <param name="register">是否写入注册表</param>
        public void WriteJsonObject<T>(string path, T obj, bool sync = false, bool register = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debuger.LogError("存放目录为空");
                return;
            }
            try
            {
                Write2FileThread.instance.Push(path, obj, register);
                if (sync) Write2FileThread.instance.ProcessWrite();
            }
            catch(System.Exception ex)
            {
                Debuger.LogException(ex);
            }
        }
#endregion

#region txt文件
        /// <summary>
        /// 读文本文件：如果内容为空，会读取备份文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public string ReadText(string path)
        {
            try
            {
                Debuger.Log("请求读文件:" + path);
                if (!File.Exists(path))
                {
                    Debuger.LogWarning("目录不存在:" + path);
                    return null;
                }
                string text = FileUtils.ReadFileText(path);
                Debuger.Log("读取内容:" + text);
                return text;
            }
            catch (System.Exception e)
            {
                Debuger.LogException(e);
                return null;
            }
        }
        /// <summary>
        /// 写文件
        /// 1.异步方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">写入路径</param>
        /// <param name="data">写入数据</param>
        /// <param name="append">是否追加</param>
        /// <param name="sync">是否同步</param>
        public void WriteText(string path, string msg, bool append = false, bool sync = false)
        {
            //Debuger.Log(string.Format("请求写文件:{0},内容:{1}", path, msg));
            if (string.IsNullOrEmpty(path))
            {
                Debuger.LogError("存放目录为空");
                return;
            }
            try
            {
                Write2FileThread.instance.Push(path, msg, append);
                if (sync) Write2FileThread.instance.ProcessWrite();
            }
            catch (System.Exception ex)
            {
                Debuger.LogException(ex);
            }
        }
#endregion

#region 本地配置表文件,StreamingAssets目录下
        /// <summary>
        /// 读取文本文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public void ReadTxtFromStream(string path, System.Action<string> handler)
        {
            Debuger.Log("ReadTxt:" + path);
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
            string textAsset = FileUtils.ReadFileText(path);
            Debuger.Log(textAsset.ToString());
            try
            {
                if(handler != null) handler(textAsset);
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(string.Format("配置表:{0},解析错误:{1}", path, ex.ToString()));
            }
#elif UNITY_ANDROID
            this.StartCoroutine(ReadTextFileFromStream(path, (textAsset) =>
            {
                Debuger.Log(textAsset.ToString());
                try
                {
                    if(handler != null) handler(textAsset);
                }
                catch (System.Exception ex)
                {
                    Debuger.LogError(string.Format("配置表:{0},解析错误:{1}", path, ex.ToString()));
                }
            }));
#else
            if(handler != null) handler(null);
#endif
        }
        /// <summary>
        /// 读取Xml配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public void ReadXmlFromStream(string path, System.Action<XmlDocument> handler)
        {
            Debuger.Log("ReadXml:" + path);
#if UNITY_EDITOR || UNITY_STANDALONE
            string textAsset = FileUtils.ReadFileText(path);
            Debuger.Log(textAsset.ToString());
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(textAsset);
            try
            {
                if (handler != null) handler(doc);
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(string.Format("配置表:{0},解析错误:{1}", path, ex.ToString()));
            }
#elif UNITY_ANDROID
            this.StartCoroutine(ReadTextFileFromStream(path, (textAsset) =>
            {
                Debuger.Log(textAsset.ToString());
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(textAsset);
                try
                {
                    if(handler != null) handler(doc);
                }
                catch (System.Exception ex)
                {
                    Debuger.LogError(string.Format("配置表:{0},解析错误:{1}", path, ex.ToString()));
                }
            }));
#else
            if(handler != null) handler(null);
#endif
        }
        /// <summary>
        /// 读取CSV配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public void ReadCsvFromStream(string path, System.Action<CSVLoadData> handler)
        {
            Debuger.Log("ReadCsv:" + path);
#if UNITY_EDITOR || UNITY_STANDALONE
            string textAsset = FileUtils.ReadFileText(path);
            Debuger.Log(textAsset.ToString());
            CSVLoadData doc = new CSVLoadData();
            doc.Load(textAsset);
            try
            {
                handler(doc);
            }
            catch (System.Exception ex)
            {
                Debuger.LogError(string.Format("配置表:{0},解析错误:{1}", path, ex.ToString()));
            }
            doc.Clear();
#elif UNITY_ANDROID
            this.StartCoroutine(ReadTextFileFromStream(path, (textAsset) =>
            {
                Debuger.Log(textAsset.ToString());
                CSVLoadData doc = new CSVLoadData();
                doc.Load(textAsset);
                try
                {
                    handler(doc);
                }
                catch (System.Exception ex)
                {
                    Debuger.LogError(string.Format("配置表:{0},解析错误:{1}", path, ex.ToString()));
                }
                doc.Clear();
            }));
#else
            if(handler != null) handler(null);
#endif
        }

        /// <summary>
        /// 从StreamingAssets目录读取：安卓需要以www形式读取
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator ReadTextFileFromStream(string path, System.Action<string> callback)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debuger.LogWarning("路径错误");
                if (callback != null) callback(string.Empty);
                yield return null;
            }

            WWW www = new WWW(path);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debuger.LogWarning("读取失败:" + path);
                if (callback != null) callback(string.Empty);
                yield return null;
            }
            if (callback != null) callback(www.text);
            if(www != null)
            {
                www.Dispose();
                www = null;
            }
        }
        #endregion
    }
}