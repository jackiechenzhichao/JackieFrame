using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace fs
{
    public class PoolTool : MonoBehaviour
    {
        private static PoolTool instance;
        private static PoolTool Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("PoolLogic").AddComponent<PoolTool>();
                    //game.hideFlags = HideFlags.HideAndDontSave;
                }
                return instance;
            }
        }

        private Dictionary<string, PoolSort> poolSorts = new Dictionary<string, PoolSort>();
        private Dictionary<int, string> idForKey = new Dictionary<int, string>();

        void Awake()
        {
            if (instance == null)
                instance = this;
            SetSortArray();
        }

        void SetSortArray()
        {
            PoolSort[] psts = FindObjectsOfType<PoolSort>();
            Debug.Log("对象池类型数量："+ psts.Length);
            for (int i = 0; i < psts.Length; i++)
            {
                PoolSort temp = psts[i];
                if (temp.GetType() == typeof(PoolSort))
                {
                    Debug.Log("对象池的key：" + temp.UseKey);
                    poolSorts.Add(temp.UseKey, temp);
                }
                else
                {
                    Debug.Log("对象池的key：" + temp.GetType().Name);
                    poolSorts.Add(temp.GetType().Name, temp);
                }
            }
        }

        /// <summary>
        /// 拿到T类型的新物体
        /// </summary>
        public static GameObject CreatObject<T>()
        {
            return Instance.CreatNewObject<T>();
        }
        private GameObject CreatNewObject<T>()
        {
            string nameKey = typeof(T).Name;
            PoolSort tempSort = poolSorts[nameKey];
            GameObject temp = tempSort.CreatNewObject();

            if (idForKey.ContainsKey(temp.GetInstanceID()) == false)
                idForKey.Add(temp.GetInstanceID(), typeof(T).ToString());
            return temp;
        }

        /// <summary>
        /// 拿到对应key值的新物体
        /// </summary>
        /// <param name="poolSortKey"></param>
        /// <returns></returns>
        public static GameObject CreatObject(string poolSortKey)
        {
            return Instance.CreatNewObject(poolSortKey);
        }
        private GameObject CreatNewObject(string useKey)
        {
            PoolSort tempSort = poolSorts[useKey];
            GameObject temp = tempSort.CreatNewObject();

            if (idForKey.ContainsKey(temp.GetInstanceID()) == false)
                idForKey.Add(temp.GetInstanceID(), useKey);

            return temp;
        }

        /// <summary>
        /// 删除T类型的对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="oldObject">需要被删除的物体</param>
        public static void DestroyObject<T>(GameObject oldObject)
        {
            Instance.DestroyOldObject<T>(oldObject);
        }


        private void DestroyOldObject<T>(GameObject oldObject)
        {
            string nameKey = typeof(T).Name;
            PoolSort tempSort = poolSorts[nameKey];
            tempSort.DestoryOldObject(oldObject);
        }



        /// <summary>
        /// 删除T类型的所有对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="oldObject">需要被删除的物体</param>
        public static void DestroyAllObject<T>()
        {
            Instance.DestroyAllOldObject<T>();
        }
        private void DestroyAllOldObject<T>()
        {
            string nameKey = typeof(T).Name;
            PoolSort tempSort = poolSorts[nameKey];
            tempSort.DestroyAllObject();
        }


        /// <summary>
        /// 删除key类型的对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="oldObject">需要被删除的物体</param>
        public static void DestroyObject(string poolSortKey,GameObject oldObj)
        {
            Instance.DestroyOldObject(poolSortKey, oldObj);
        }



        private void DestroyOldObject(string poolSortKey, GameObject oldObj)
        {
            PoolSort tempSort = poolSorts[poolSortKey];
            tempSort.DestoryOldObject(oldObj);
        }


        public static void DestroyObject(GameObject oldObj)
        {
            Instance.DestroyOldObject(oldObj);
        }
        private void DestroyOldObject(GameObject oldObj)
        {
            PoolSort tempSort = poolSorts[idForKey[oldObj.GetInstanceID()]];
            tempSort.DestoryOldObject(oldObj);
        }

        /// <summary>
        /// 删除key类型的所有对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="oldObject">需要被删除的物体</param>
        public static void DestroyAllObject(string poolSortKey)
        {
            Instance.DestroyAllOldObject(poolSortKey);
        }
        private void DestroyAllOldObject(string poolSortKey)
        {
            PoolSort tempSort = poolSorts[poolSortKey];
            tempSort.DestroyAllObject();
        }


        private void OnDestroy()
        {
            poolSorts.Clear();
            instance = null;
        }

    }

    public static class ExtendPool
    {
        public static void DestroyInPool(this GameObject game, string useKey)
        {
            PoolTool.DestroyObject(useKey, game);
        }

        public static void DestroyInPool<T>(this GameObject game)
        {
            PoolTool.DestroyObject<T>(game);
        }
    }


}
