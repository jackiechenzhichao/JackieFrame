using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fs
{
    /// <summary>
    /// 数字排列组件：需要配合GridLayoutGroup一块使用
    /// @author hannibal
    /// @time 2015-1-6
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class UIImageNumber : UIComponentBase
    {
        public Sprite[] m_TemplateSprite;                               //存放图片的数组
        public string m_TemplateText;                                   //模板字符串
        public bool m_SetNativeSize = true;                             //图片是否为原始比例
        public string m_NumValue = "";                                  //数字字符串
        private List<GameObject> m_NumImage = new List<GameObject>();   //存放产生的物体列表

        void Start()
        {
            SetData(m_NumValue);
        }

        /// <summary>
        /// 设置物体的名称、父节点、图片等信息
        /// </summary>
        /// <param name="num"></param>
        public void SetData(string num)
        {
            this.Despawn();
            m_NumValue = num;
            string arr = m_NumValue.ToString();
            for (int i = 0; i < arr.Length; ++i)
            {
                GameObject obj = this.Spawn();
                obj.name = arr[i].ToString();

                Image image = obj.GetComponent<Image>();
                image.transform.SetParent(transform, false);
                image.sprite = this.GetSpriteByNumber(arr[i]);
                image.raycastTarget = false;
                if (m_SetNativeSize) image.SetNativeSize();
            }
        }

        /// <summary>
        /// 销毁所有的子节点
        /// </summary>
        public void Destroy()
        {
            while (transform.childCount > 0)
            {
                GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
            }
            m_NumImage.Clear();
        }

        /// <summary>
        /// 通过索引值返回图片数组内的图片
        /// </summary>
        /// <param name="num">字符数字</param>
        /// <returns></returns>
        private Sprite GetSpriteByNumber(char num)
        {
            int index = m_TemplateText.IndexOf(num);
            if (index >= 0 && index < m_TemplateSprite.Length)
            {
                return m_TemplateSprite[index];
            }
            else
            {
                Debuger.LogError("设置了不存在的字符:" + num);
                return null;
            }
        }
        
        /// <summary>
        /// 产生新的图片
        /// </summary>
        /// <returns></returns>
        private GameObject Spawn()
        {
            GameObject obj = null;
            while (m_NumImage.Count > 0)
            {
                if(m_NumImage[0] != null)
                {
                    obj = m_NumImage[0];
                    m_NumImage.RemoveAt(0);
                    break;
                }
                else
                {
                    m_NumImage.RemoveAt(0);
                }
            }
            if(obj == null)
            {
                obj = new GameObject();
                obj.AddComponent<Image>();
            }
            obj.transform.localScale = Vector3.one;
            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 如果物体状态为激活状态，则产生新的游戏物体
        /// </summary>
        private void Despawn()
        {
            foreach(Transform t in transform)
            {
                GameObject obj = t.gameObject;
                if(obj.activeSelf)
                {
                    m_NumImage.Add(obj);
                    obj.SetActive(false);
                }
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UIImageNumber))]
    public class UIImageNumberInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UIImageNumber owner_script = (UIImageNumber)target;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("预览"))
            {
                owner_script.SetData(owner_script.m_NumValue);
            }
            if (GUILayout.Button("清除"))
            {
                owner_script.Destroy();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}