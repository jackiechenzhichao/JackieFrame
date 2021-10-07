using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace fs
{
    public class PoolSort : MonoBehaviour
    {
        [HideInInspector]public string _useKey;
        public string UseKey
        {
            get { return _useKey; }
        }

        private GameObject pre;
        public GameObject prefab
        {
            get
            {
                return pre;
            }
        }

        public virtual GameObject GetPrefab()
        {
            if (pre==null)
            {
                if (_objPrefab == null)
                {
                    pre = GameObject.Instantiate(Resources.Load<GameObject>(path)) as GameObject;
                    pre.gameObject.SetActive(false);
                    pre.transform.SetParent(this.transform);
                    pre.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    pre = _objPrefab;
                }
            }
            return pre;
        }


        public GameObject _objPrefab;
        public string path;
        public int preAmount = 8;
        public int maxCount = 15;

        public List<GameObject> useObjects = new List<GameObject>();
        public List<GameObject> idleObjects = new List<GameObject>();


        public void Start()
        {
            InitLogic();
            OnStart();
        }

        public virtual void OnStart()
        {
            StartCoroutine(WaitDestroy());
        }

        public virtual void InitLogic()
        {
            for (int i = 0; i < preAmount; i++)
            {
                CreatObject();
            }
        }

        public virtual void DestoryOldObject(GameObject oldObj)
        {
            if (this.gameObject != null)
            {
                oldObj.SetActive(false);
                oldObj.transform.SetParent(this.transform);
                SetToIdle(oldObj);
            }
        }

        public virtual void DestroyAllObject()
        {
            while (useObjects.Count > 0)
            {
                DestoryOldObject(useObjects[0]);
            }
        }

        public virtual GameObject CreatNewObject()
        {
            if (idleObjects.Count == 0)
            {
                CreatObject();
            }

            idleTime = 0;
            GameObject tempObject = idleObjects[0];
            idleObjects.RemoveAt(0);

            while (true)
            {
                if (tempObject != null)
                {
                    if (!useObjects.Contains(tempObject))
                        useObjects.Add(tempObject);

                    if (idleObjects.Contains(tempObject))
                        idleObjects.Remove(tempObject);

                    tempObject.gameObject.SetActive(true);
                    break;
                }
                else
                {          
                    if (idleObjects.Contains(tempObject))
                        idleObjects.Remove(tempObject);
                    if (idleObjects.Count <= 0)
                        CreatObject();
                    tempObject = idleObjects[0];
                    Debug.LogError("不知道为啥等于Null");
                }
            }
            return tempObject;
        }

        private void CreatObject()
        {
            pre = GetPrefab();
            GameObject obj = Instantiate(prefab) as GameObject;
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            idleObjects.Add(obj);
        }


        private void SetToIdle(GameObject oldObj)
        {
            //if (idleObjects.Count + useObjects.Count >= maxCount)
            //{
            //    idleObjects.Remove(oldObj);
            //    useObjects.Remove(oldObj);
            //    Destroy(oldObj);
            //}
            //else
            {
                if (!idleObjects.Contains(oldObj))
                    idleObjects.Add(oldObj);
                useObjects.Remove(oldObj);
            }
        }


        float idleTime = 0;
        float destroyTime = 5.0f;
        IEnumerator WaitDestroy()
        {
            Start:
            while (idleTime <= destroyTime)
            {
                idleTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            idleTime = 0;
            while (idleObjects.Count + useObjects.Count > maxCount)
            {
                if (idleObjects.Count > 0)
                {
                    Debuger.Log("删除多余的："+idleObjects.Count);
                    Destroy(idleObjects[idleObjects.Count-1]);
                    idleObjects.RemoveAt(idleObjects.Count-1);
                }
                else
                    break;
            }

            goto Start;
        }
    }

}
