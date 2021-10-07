
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：运行模式下保存脚本修改
// 创建时间：2019-07-23 13:55:38
// 使用方式如下：
//#if UNITY_EDITOR
//    [CustomEditor(typeof(GlobalConfig), true)]
//    public class GlobalConfigEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();

//            GUILayout.Space(10);
//            if (GUILayout.Button("保存"))
//            {
//                SaveParam();
//                //ApplyParam();
//            }
//        }

//        void SaveParam()
//        {
//            //将target（即Editor脚本的目标组件TestEditParam）注册到修改队列
//            Component mTarget = target as Component;
//            PlayModeSaver.Instance.SaveValues(mTarget);
//            //将Transform 注册到修改队列
//            Transform rectCom = mTarget.transform.GetComponent<Transform>();
//            PlayModeSaver.Instance.SaveValues(rectCom);
//        }

//        //提交预设体修改  直接保存prefab的暴力方法 只需更改prefab时直接用此接口即可
//        void ApplyParam()
//        {
//            Object prefabParent = PrefabUtility.GetPrefabParent(target);
//            GlobalConfig mScript = target as GlobalConfig;
//            PrefabUtility.ReplacePrefab(mScript.gameObject, prefabParent);
//        }
//    }
//#endif
//=======================================================
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEditor;

namespace fs
{
    public class PlayModeSaver
    {
        private static PlayModeSaver _instance;
        public static PlayModeSaver Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PlayModeSaver();
                return _instance;
            }
        }

        //在构造函数中 注册修改事件
        public PlayModeSaver()
        {
            EditorApplication.playmodeStateChanged = Application_PlaymodeStateChanged;
            SavingDatas = new Dictionary<int, SettingData>();
        }

        List<Component> SavingGroups = new List<Component>(); //需要保存的组件列表
        List<int> SavingIDs = new List<int>();                //需要保存组件的InstanceID列表
        Dictionary<int, SettingData> SavingDatas;             //缓存数据字典

        void Application_PlaymodeStateChanged()
        {
            if (EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                //window repaint or do sth...
            }
            else
            {
                RestoreAllSavingSetting();
                EditorApplication.playmodeStateChanged = null;
            }
        }

        //还原所有修改数据
        void RestoreAllSavingSetting()
        {
            for (int i = 0; i < SavingIDs.Count; i++)
            {
                RestoreSetting(SavingIDs[i]);
            }
        }

        //通过实例ID修改组件参数
        void RestoreSetting(int id)
        {
            Component ComponentObject = EditorUtility.InstanceIDToObject(id) as Component;

            Dictionary<string, object> values = SavingDatas[id].values;

            foreach (string name in values.Keys)
            {
                object newValue = values[name];

                PropertyInfo property = ComponentObject.GetType().GetProperty(name);

                if (null != property)
                {
                    object currentValue = property.GetValue(ComponentObject, null);
                    property.SetValue(ComponentObject, newValue, null);
                }
                else
                {
                    FieldInfo field = ComponentObject.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    object currentValue = field.GetValue(ComponentObject);
                    field.SetValue(ComponentObject, newValue);
                }
            }

            if (ComponentObject != null)
            {
                SetPrefabDirty(ComponentObject);
            }
        }

        //如果是预设体，通知引擎修改 不能省略，修正了修改目标是预设体时，每次运行参数重置的bug
        void SetPrefabDirty(Component ComponentObject)
        {
            PrefabType prefabType = PrefabUtility.GetPrefabType(ComponentObject.gameObject);
            if (prefabType == PrefabType.DisconnectedPrefabInstance || prefabType == PrefabType.PrefabInstance)
            {
                EditorUtility.SetDirty(ComponentObject);
            }
        }

        //缓存修改信息  包括属性、字段
        public void SaveValues(Component ComponentObject)
        {
            if (SavingGroups.Contains(ComponentObject))
            {
                RefreshValues(ComponentObject);
                return;
            }

            Dictionary<string, object> values = new Dictionary<string, object>();

            List<PropertyInfo> properties = GetProperties(ComponentObject);
            List<FieldInfo> fields = GetFields(ComponentObject);

            foreach (PropertyInfo property in properties)
            {
                values.Add(property.Name, property.GetValue(ComponentObject, null));
            }

            foreach (FieldInfo field in fields)
            {
                values.Add(field.Name, field.GetValue(ComponentObject));
            }

            //添加需要保存的组件
            SavingGroups.Add(ComponentObject);
            SavingIDs.Add(ComponentObject.GetInstanceID());
            SavingDatas.Add(ComponentObject.GetInstanceID(), new SettingData(values));
        }

        //修正多次点击修改不能更新数据的bug
        private void RefreshValues(Component ComponentObject)
        {
            Dictionary<string, object> values = SavingDatas[ComponentObject.GetInstanceID()].values;

            List<PropertyInfo> properties = GetProperties(ComponentObject);
            List<FieldInfo> fields = GetFields(ComponentObject);

            foreach (PropertyInfo property in properties)
            {
                values[property.Name] = property.GetValue(ComponentObject, null);
            }

            foreach (FieldInfo field in fields)
            {
                values[field.Name] = field.GetValue(ComponentObject);
            }
        }

        //获取私有字段+公有字段列表
        private List<FieldInfo> GetFields(Component ComponentObject)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            //获取字段包括私有字段、共有字段  修正了私有字段不能正常修改的bug
            FieldInfo[] infos = ComponentObject.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            //foreach (FieldInfo fieldInfo in ComponentObject.GetType().GetFields())
            foreach (FieldInfo fieldInfo in infos)
            {
                if (!Attribute.IsDefined(fieldInfo, typeof(HideInInspector)))
                {
                    fields.Add(fieldInfo);
                }
            }
            return fields;
        }

        //获取属性列表
        private List<PropertyInfo> GetProperties(Component ComponentObject)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            foreach (PropertyInfo propertyInfo in ComponentObject.GetType().GetProperties())
            {
                if (!Attribute.IsDefined(propertyInfo, typeof(HideInInspector)))
                {
                    MethodInfo setMethod = propertyInfo.GetSetMethod();
                    if (null != setMethod && setMethod.IsPublic)
                    {
                        properties.Add(propertyInfo);
                    }
                }
            }
            return properties;
        }
    }

    //简易的设置数据 自行根据需求扩充
    class SettingData
    {
        public Dictionary<string, object> values;

        public SettingData(Dictionary<string, object> datas)
        {
            values = datas;
        }

        public void AddData(string name, object value)
        {
            if (!values.ContainsKey(name))
            {
                values.Add(name, value);
            }
        }
    }
}
#endif