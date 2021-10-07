using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace fs
{
    /// <summary>
    /// Sprite对象池
    /// @author hannibal
    /// @time 2016-1-20
    /// </summary>
    public class SpritePools
    {
        private static Dictionary<string, List<Sprite>> m_DicFile2Pool = new Dictionary<string, List<Sprite>>();

        /// <summary>
        /// 产生对象
        /// </summary>
        /// <param name="file">查找键值，注意：内部会修改sprite的name，如果使用了这个接口，外部就不要再修改name</param>
        /// <returns></returns>
        public static Sprite Spawn(string file)
        {
            if (file.Length == 0) return null;

            Sprite spawnItem = null;
            List<Sprite> itemArray = null;
            ///1.查找pools
            if (m_DicFile2Pool.TryGetValue(file, out itemArray))
            {
                if (itemArray.Count > 0)
                {
                    spawnItem = itemArray[itemArray.Count - 1];
                    itemArray.RemoveAt(itemArray.Count - 1);
                }
            }
            ///2.创建新的
            if (spawnItem == null)
            {
                spawnItem = ResourceManager.instance.LoadResource<Sprite>(file);
            }
            if (spawnItem != null) spawnItem.name = file;
            return spawnItem;
        }
        public static void Spawn(string file, System.Action<Sprite> callback)
        {
            if (file.Length == 0)
            {
                if (callback != null) callback(null);
                return;
            }

            Sprite spawnItem = null;
            List<Sprite> itemArray = null;
            ///1.查找pools
            if (m_DicFile2Pool.TryGetValue(file, out itemArray))
            {
                if (itemArray.Count > 0)
                {
                    spawnItem = itemArray[itemArray.Count - 1];
                    itemArray.RemoveAt(itemArray.Count - 1);
                }
            }
            ///2.创建新的
            if (spawnItem == null)
            {
                ResourceManager.instance.LoadResource<Sprite>(file, delegate(Sprite sprite)
                {
                    if (sprite != null) sprite.name = file;
                    if (callback != null) callback(sprite);
                });
            }
            else
            {
                spawnItem.name = file;
                if (callback != null) callback(spawnItem);
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        public static void Despawn(Sprite obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.name)) return;

            List<Sprite> itemArray = null;
            if (!m_DicFile2Pool.TryGetValue(obj.name, out itemArray))
            {
                itemArray = new List<Sprite>();
                m_DicFile2Pool[obj.name] = itemArray;
            }
            if (!itemArray.Contains(obj)) itemArray.Add(obj);
        }

        public static void Release()
        {
            foreach (var item_list in m_DicFile2Pool)
            {
                foreach (var obj in item_list.Value)
                {
                    if (obj != null) Resources.UnloadAsset(obj);
                }
            }
            m_DicFile2Pool.Clear();
        }

        public static string ToString(bool is_print)
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("-------------------------");
            st.AppendLine("SpritePools使用情况:" + m_DicFile2Pool.Count);
            foreach (var obj in m_DicFile2Pool)
            {
                string one_line = obj.Key + " count:" + obj.Value.Count;
                st.AppendLine(one_line);
            }
            if (is_print) Debuger.Log(st.ToString());
            return st.ToString();
        }
    }
}