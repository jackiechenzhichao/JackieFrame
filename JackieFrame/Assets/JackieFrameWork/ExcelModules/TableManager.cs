using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using ProtoBuf;

namespace fs
{
    /// <summary>
    /// 配置表管理器
    /// @author hannibal
    /// @time 2019-6-13
    /// </summary>
    public class TableManager :Singleton<TableManager>
    {
        /// <summary>
        /// Resources目录下的路径
        /// </summary>
        public string TablePath = "Table";

        ////[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        //static void StartOnUnity()
        //{
        //    TableManager.instance.Load();
        //}

        public void OnInit()
        {
            //Debuger.LogError("gggggggggg");
            Load();
        }

        private void Load()
        {
            this.LoadTable<string, StdGlobalValue>();
            this.LoadTable<string, StdGridReward>();
        }
        
        private void LoadTable<K, T>() where T : Row<K>
        {
            string name = typeof(T).Name;
            Debuger.Log("加载配置表:" + name);

            try
            {
                //加载
                string file_path = GetTablePath(name);
                byte[] ta = FileUtils.ReadFileByte(file_path);
                if (ta == null)
                {
                    Debuger.LogError("配置表加载失败:" + file_path);
                    return;
                }

                PropertyInfo pi = typeof(TableLib).GetProperty(name);
                pi.SetValue(new TableLib(), LoadTable<K, T>(ta), null);
            }
            catch(System.Exception e)
            {
                Debuger.LogError(string.Format("配置表{0}读取失败:{1}",name, e.ToString()));
            }
        }
        private Table<K, T> LoadTable<K, T>(byte[] ta) where T : Row<K>
        {
            DataSrc<K, T> dataSrc = Load<K, T>(ta);
            if (dataSrc != null)
            {
                return new Table<K, T>(dataSrc);
            }
            return null;
        }
        private DataSrc<K, T> Load<K, T>(byte[] ta) where T : Row<K>
        {
            if (ta != null)
            {
                using (MemoryStream ms = new MemoryStream(ta))
                {
                    return Serializer.Deserialize<DataSrc<K, T>>(ms);
                }
            }
            return null;
        }
        
        private string GetTablePath(string tableName)
        {
            return string.Format("{0}/{1}/{2}.bytes", Application.streamingAssetsPath, TablePath, tableName);
        }
    }

    public partial class TableLib { }
}
