using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace fs
{
    public class CreatAudioData : ScriptableWizard
    {
        public static string savePath = "Assets/Resources/AssetData";

        public static string SelfConfig = "Assets/Editor/AudioConfig";
        public static string configName = "AudioConfig.asset";


        public class Data
        {
            private bool isOpen = false;

            private string label = string.Empty;

            public string SoundUrl = string.Empty;

            private Rect rect = new Rect(20, 100, 100, 18);


            public Data(Rect rect, string tag)
            {
                label = tag;
                this.rect = rect;
            }

            public void OnShowGUI()
            {
                EditorGUI.LabelField(rect, label);

                isOpen = GUI.Button(new Rect(200 + rect.x, rect.y, rect.width, rect.height), new GUIContent("Select"));
                if (isOpen)
                {
                    SoundUrl = SplitResources(EditorUtility.OpenFolderPanel("选择文件", Application.dataPath + "Resources", ""));
                    isOpen = false;
                }

                SoundUrl = EditorGUI.TextField(new Rect(80 + rect.x, rect.y, rect.width, rect.height), SoundUrl);
            }

        }


        public static string SplitResources(string url, int count = 0)
        {
            char[] e = new char[] { '/', '\\' };

            string[] splits = url.Split(e);
            string resulf = string.Empty;

            bool isAdd = false;

            for (int i = 0; i < splits.Length - count; i++)
            {
                if (splits[i] == "Resources")
                {
                    isAdd = true;
                    continue;
                }
                if (isAdd)
                {
                    if (i < splits.Length - 1)
                        resulf += splits[i] + "/";
                    else
                        resulf += splits[i];
                }
            }

            isAdd = false;

            return resulf;
        }

        public static string SplitPath(string url)
        {
            char[] e = new char[] { '.' };
            return url.Split(e)[0];
        }

        public static string SplitFiles(string url)
        {
            char[] e = new char[] { '/', '\\' };
            return url.Split(e)[1];
        }

        public Data subData;
        public Data soundData;
        public Data effectData;

        Rect allRect = new Rect(20, 100, 100, 18);
        Rect SetRect(float x, float y, float w, float h)
        {
            return new Rect(allRect.x + x, allRect.y + y, allRect.width + w, allRect.height + h);
        }


        [MenuItem("音频/基本设置")]
        static void Creat()
        {
            ScriptableWizard.DisplayWizard("初始设置音频地址", typeof(CreatAudioData));
        }


        private void OnEnable()
        {


            AudioConfig audioConfig = AssetDatabase.LoadAssetAtPath<AudioConfig>(SelfConfig + "/" + configName);

            soundData = new Data(allRect, "配音资源地址");
            subData = new Data(new Rect(allRect.x, allRect.y + 30, allRect.width, allRect.height), "字幕资源地址");
            effectData = new Data(new Rect(allRect.x, allRect.y + 60, allRect.width, allRect.height), "特效资源地址");

            if (audioConfig != null)
            {
                soundData.SoundUrl = audioConfig.SoundPath;
                subData.SoundUrl = audioConfig.SubtitlePath;
                effectData.SoundUrl = audioConfig.effectPath;
            }
            else
            {

            }

        }


        private void OnGUI()
        {
            soundData.OnShowGUI();
            subData.OnShowGUI();
            effectData.OnShowGUI();

            if (GUI.Button(SetRect(200, 100, 0, 0), "创建"))
            {
                Create();
                this.Close();
            }
        }


        private void Create()
        {
            var creat = CreateInstance<AudioConfig>();

            if (Directory.Exists(SelfConfig) == false)
            {
                Directory.CreateDirectory(SelfConfig);
            }

            creat.SoundPath = soundData.SoundUrl;
            creat.SubtitlePath = subData.SoundUrl;

            AssetDatabase.CreateAsset(creat, SelfConfig + "/" + configName);
            AssetDatabase.Refresh();
        }



        #region 音效

        static string soundName = "SoundAsset.asset";
        static AudioClip[] audioClips;
        static Dictionary<string, SoundData> soundDec = new Dictionary<string, SoundData>();


        [MenuItem("音频/创建配置表/音效")]
        static void CreateExampleAsset()
        {
            AudioSoundData exampleAsset = AssetDatabase.LoadAssetAtPath<AudioSoundData>(CreatAudioData.savePath + "/" + soundName);
            if (exampleAsset != null)
            {
                soundDec.Clear();

                for (int i = 0; i < exampleAsset.audioLists.Count; i++)
                {
                    soundDec.Add(exampleAsset.audioLists[i].clipName, exampleAsset.audioLists[i]);
                }
            }
            else
            {
                if (Directory.Exists(CreatAudioData.savePath) == false)
                {
                    Directory.CreateDirectory(CreatAudioData.savePath);
                }
            }

            AudioConfig audioConfig = AssetDatabase.LoadAssetAtPath<AudioConfig>(CreatAudioData.SelfConfig + "/" + CreatAudioData.configName);

            RecordSoundData(audioConfig.SoundPath);
            CreatSound(audioConfig.SoundPath);
        }

        static void RecordSoundData(string clipPath)
        {
            List<string> names = new List<string>();
            List<string> clips = new List<string>();
            audioClips = Resources.LoadAll<AudioClip>(clipPath);


            for (int i = 0; i < audioClips.Length; i++)
            {
                if (soundDec.ContainsKey(audioClips[i].name) == false)
                {
                    SoundData temp = new SoundData();
                    temp.clipName = audioClips[i].name;
                    temp.clipSoundUrl = clipPath + "/" + temp.clipName;
                    soundDec.Add(temp.clipName, temp);
                }
                clips.Add(audioClips[i].name);
            }


            foreach (var item in soundDec.Keys)
            {
                if (clips.Contains(item) == false)
                    names.Add(item);
            }


            for (int i = 0; i < names.Count; i++)
            {
                soundDec.Remove(names[i]);
            }
        }

        static void CreatSound(string urlPath)
        {
            string[] names = new string[soundDec.Keys.Count];
            AudioSoundData exampleAsset = CreateInstance<AudioSoundData>();

            exampleAsset.audioLists = new List<SoundData>();
            soundDec.Keys.CopyTo(names, 0);

            for (int i = 0; i < soundDec.Keys.Count; i++)
            {
                exampleAsset.audioLists.Add(soundDec[names[i]]);
            }

            exampleAsset.urlPath = urlPath;

            AssetDatabase.CreateAsset(exampleAsset, CreatAudioData.savePath + "/" + soundName);
            AssetDatabase.Refresh();
        }

        #endregion

        #region 字幕
        static string subName = "SubtitleAsset.asset";
        static AudioClip[] subTitleClips;
        static Dictionary<string, SubtitleData> subTitleDec = new Dictionary<string, SubtitleData>();


        [MenuItem("音频/创建配置表/字幕")]
        static void CreateSubTitleAsset()
        {
            AudioSubtitleData exampleAsset = AssetDatabase.LoadAssetAtPath<AudioSubtitleData>(CreatAudioData.savePath + "/" + subName);

            if (exampleAsset != null)
            {
                subTitleDec.Clear();

                for (int i = 0; i < exampleAsset.audioLists.Count; i++)
                {
                    exampleAsset.audioLists[i].clips.Clear();
                    exampleAsset.audioLists[i].clipPaths.Clear();
                    subTitleDec.Add(exampleAsset.audioLists[i].clipName, exampleAsset.audioLists[i]);
                }

            }
            else
            {
                if (Directory.Exists(CreatAudioData.savePath) == false)
                {
                    Directory.CreateDirectory(CreatAudioData.savePath);
                }
            }

            AudioConfig audioConfig = AssetDatabase.LoadAssetAtPath<AudioConfig>(CreatAudioData.SelfConfig + "/" + CreatAudioData.configName);

            RecordSubtitleData(audioConfig.SubtitlePath);
            CreatSubtitle(audioConfig.SubtitlePath);
        }


        static void RecordSubtitleData(string clipPath)
        {
            List<string> names = new List<string>();
            List<string> clips = new List<string>();
            audioClips = Resources.LoadAll<AudioClip>(clipPath);

            for (int i = 0; i < audioClips.Length; i++)
            {
                if (subTitleDec.ContainsKey(audioClips[i].name) == false)
                {
                    SubtitleData temp = new SubtitleData();
                    temp.clipName = audioClips[i].name;
                    temp.clipPaths = new List<string>();
                    temp.clipPaths.Add(SplitPath(SplitResources(AssetDatabase.GetAssetPath(audioClips[i]))));

                    temp.clips = new List<AudioClip>();
                    temp.clips.Add(audioClips[i]);
                    subTitleDec.Add(audioClips[i].name, temp);
                }
                else
                {
                    string name = SplitPath(SplitResources(AssetDatabase.GetAssetPath(audioClips[i])));
                    SubtitleData temp = subTitleDec[audioClips[i].name];

                    if (temp.clipPaths.Contains(name) == false)
                    {
                        temp.clipName = audioClips[i].name;
                        temp.clipPaths.Add(name);
                        temp.clips.Add(audioClips[i]);
                        if (temp.records.ContainsKey(name))
                            temp.records[name] = temp.clips.Count - 1;
                        else
                            temp.records.Add(name, temp.clips.Count - 1);
                    }
                    else
                    {
                        Debug.Log(temp.records.Count);
                        temp.clips[temp.records[name]] = audioClips[i];
                    }

                }
                clips.Add(audioClips[i].name);
            }


            foreach (var item in subTitleDec.Keys)
            {
                if (clips.Contains(item) == false)
                    names.Add(item);
            }


            for (int i = 0; i < names.Count; i++)
            {
                subTitleDec.Remove(names[i]);
            }

        }

        static void CreatSubtitle(string urlPath)
        {
            string[] names = new string[subTitleDec.Keys.Count];
            AudioSubtitleData exampleAsset = CreateInstance<AudioSubtitleData>();

            exampleAsset.audioLists = new List<SubtitleData>();

            subTitleDec.Keys.CopyTo(names, 0);

            List<string> nameLists = new List<string>();
            for (int i = 0; i < names.Length; i++)
            {
                nameLists.Add(names[i]);
            }
            nameLists.Sort(EditorUtility.NaturalCompare);

            for (int i = 0; i < subTitleDec.Keys.Count; i++)
            {
                exampleAsset.audioLists.Add(subTitleDec[nameLists[i]]);
            }

            exampleAsset.urlPath = urlPath;

            AssetDatabase.CreateAsset(exampleAsset, CreatAudioData.savePath + "/" + subName);
            AssetDatabase.Refresh();
        }
        #endregion


        #region 特效
        static string effectName = "EffectAsset.asset";
        static AudioClip[] effectClips;
        static Dictionary<string, PrefabSort> effectDec = new Dictionary<string, PrefabSort>();

        [MenuItem("音频/创建配置表/特效")]
        static void CreateEffectAsset()
        {
            EffectData exampleAsset = AssetDatabase.LoadAssetAtPath<EffectData>(CreatAudioData.savePath + "/" + effectName);
            if (exampleAsset != null)
            {
                effectDec.Clear();

                for (int i = 0; i < exampleAsset.prefabSorts.Count; i++)
                {
                    effectDec.Add(exampleAsset.prefabSorts[i].name, exampleAsset.prefabSorts[i]);
                }
            }
            else
            {
                if (Directory.Exists(CreatAudioData.savePath) == false)
                {
                    Directory.CreateDirectory(CreatAudioData.savePath);
                }
            }

            AudioConfig audioConfig = AssetDatabase.LoadAssetAtPath<AudioConfig>(CreatAudioData.SelfConfig + "/" + CreatAudioData.configName);

            CreatEffect(audioConfig.effectPath);
        }

        static void CreatEffect(string urlPath)
        {
            string[] names = new string[soundDec.Keys.Count];
            EffectData exampleAsset = CreateInstance<EffectData>();

            exampleAsset.prefabSorts = new List<PrefabSort>();
            soundDec.Keys.CopyTo(names, 0);

            for (int i = 0; i < soundDec.Keys.Count; i++)
            {
                exampleAsset.prefabSorts.Add(effectDec[names[i]]);
            }

            AssetDatabase.CreateAsset(exampleAsset, CreatAudioData.savePath + "/" + effectName);
            AssetDatabase.Refresh();
        }

    } 
    #endregion
}