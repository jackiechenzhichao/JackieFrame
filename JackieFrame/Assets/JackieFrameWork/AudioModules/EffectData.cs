using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace fs
{
    public class EffectData : ScriptableObject
    {
        public List<PrefabSort> prefabSorts = new List<PrefabSort>();

        [Button("保存")]
        private void EffectDataSave()
        {
            for(int i = 0; i < prefabSorts.Count; i++)
            {
                for(int j = 0; j < prefabSorts[i].prefabs.Count; j++)
                {
                    if (prefabSorts[i].prefabs[j].value != null)
                    {
                        prefabSorts[i].prefabs[j].Key = prefabSorts[i].prefabs[j].value.name;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class PrefabSort
    {
        public string name;
        public List<Prefab> prefabs = new List<Prefab>();
    }

    [System.Serializable]
    public class Prefab
    {
        public string Key;
        public GameObject value;

        [Space(20)]
        public bool isUI = false;
        public bool isPool = true;

    }
}
