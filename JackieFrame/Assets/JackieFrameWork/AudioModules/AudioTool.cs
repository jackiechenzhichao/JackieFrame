using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace fs
{
    public class AudioTool : MonoBehaviour
    {
        private Dictionary<string, SoundData> soundClips = new Dictionary<string, SoundData>();
        private Dictionary<string, SubtitleData> subtitleClips = new Dictionary<string, SubtitleData>();

        private List<string> allClipNames = new List<string>();

        private List<Source2D> idleSources2D = new List<Source2D>();
        private List<Source2D> idleSources3D = new List<Source2D>();

        private List<Source2D> useSources = new List<Source2D>();

        private Source2D mainBGSource;
        
        private bool isLoadComplete = false;
        public bool IsLoadComplete
        {
            get { return isLoadComplete; }
        }
        public System.Action completeEvent;



        private int maxCount = 8;
        public bool isSubtitling = false;


        /// <summary>
        /// 游戏音量，影响音效和旁白
        /// </summary>
        private float gameVolume = 1;
        public float GameVolume
        {
            get { return gameVolume; }
            set{ gameVolume = value; }
        }
        /// <summary>
        /// 游戏音量，影响音效和旁白
        /// </summary>
        private float bgVolume = 1;
        public float BGVolume
        {
            get { return bgVolume; }
            set
            {
                bgVolume = value;
                SetBgMusicVolume(bgVolume);
            }
        }

        /// <summary>
        /// 语言
        /// NOTE:需要外部设置
        /// </summary>
        private int gameLanguage = 1;
        public int GameLanguage
        {
            get { return gameLanguage; }
            set { gameLanguage = value; }
        }
        
        private float volumeWeight = 1.0f;
        public float VolumeWeight
        {
            get { return volumeWeight; }
            set { volumeWeight = value; }
        }


        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void StartOnUnity()
        {
            instance = new GameObject("AudioToolGame").AddComponent<AudioTool>();
            //防止场景切换时被消耗，导致音量和语言等设置失效
            DontDestroyOnLoad(instance.gameObject);
        }


        private void Awake()
        {
            LoadAllClip();
        }



        private static AudioTool instance;
        public static AudioTool Instance
        {
            get
            {
                return instance;
            }
        }
        
        void InitSources2D()
        {
            for (int i = 0; i < 2; i++)
            {
                Sources2D temp = new Sources2D();
                temp.SetSource(this.transform);
                idleSources2D.Add(temp);
            }
        }

        #region 播放音效事件

        /// <summary>
        /// 播放指定key值的音频(2D)
        /// </summary>
        /// <param name="clipName">音频的key</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <param name="volume">音频的音量</param>
        public void Play(string clipName, float volume = 1.0f)
        {
            Play(clipName, false, volume);
        }

        /// <summary>
        /// 播放指定key值的音频(2D)
        /// </summary>
        /// <param name="clipName">音频的key</param>
        /// <param name="isLoop">是否循环播放</param>
        public void Play(string clipName, bool isLoop)
        {
            Play(clipName, isLoop, 1.0f);
        }

        /// <summary>
        /// 播放指定key值的音频(2D)
        /// </summary>
        /// <param name="clipName">音频的key</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <param name="volume">音频的音量</param>
        public void Play(string clipName, bool isLoop, float volume)
        {
            //Debuger.Log("音效："+clipName);

            //if (GameManage.Instance.isIdleMovie)
            //    return;
            try
            {
                if (soundClips[clipName].IsCD())
                {

                    if (idleSources2D.Count == 0)
                    {
                        Sources2D temp = new Sources2D();
                        temp.SetSource(transform);
                        idleSources2D.Add(temp);
                    }

                    Source2D audioSource = idleSources2D[0];
                    AudioClip audioClip = soundClips[clipName].clipSoundData;
                    audioSource.clipName = clipName;
                    audioSource.SetData(isLoop, GetVolumeForWeight(soundClips[clipName].clipVolume) * volume * gameVolume);
                    audioSource.Play(audioClip);

                    useSources.Add(audioSource);
                    idleSources2D.Remove(audioSource);
                }
            }
            catch(System.Exception e)
            {
                Debuger.Log(string.Format("音频报错,{0}, 原因:{1}", clipName, e.ToString()));
            }
        }

        /// <summary>
        /// 播放指定key值的音频(3D)
        /// </summary>
        /// <param name="clipName">音频的key</param>
        /// <param name="game3D">播放3D音效需要的物体</param>
        /// <param name="type">在音频播放组件上需要加载的脚本类型</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <param name="volume">音频的音量</param>
        public void Play(string clipName, GameObject game3D, System.Type type, bool isLoop = false, float volume = 1.0f)
        {
            try
            {
                if (idleSources3D.Count == 0)
                {
                    Sources3D temp = new Sources3D(type);
                    temp.SetSource(game3D.transform);
                    idleSources3D.Add(temp);
                }

                idleSources3D[0].SetSource(game3D.transform);

                Source2D audioSource = idleSources3D[0];
                AudioClip audioClip = soundClips[clipName].clipSoundData;
                audioSource.clipName = clipName;
                audioSource.SetData(isLoop, GetVolumeForWeight(soundClips[clipName].clipVolume) * volume * gameVolume);
                audioSource.Play(audioClip);

                useSources.Add(audioSource);
                idleSources3D.Remove(audioSource);
            }
            catch (System.Exception e)
            {
                Debuger.Log(string.Format("音频报错,{0}, 原因:{1}", clipName, e.ToString()));
            }
        }

        /// <summary>
        /// 播放指定key值的音频(3D)
        /// </summary>
        /// <param name="clipName">音频的key</param>
        /// <param name="game3D">播放3D音效需要的物体</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <param name="volume">音频的音量</param>
        public void Play(string clipName, GameObject game3D, bool isLoop = false, float volume = 1.0f)
        {
            Play(clipName, game3D, typeof(SourceExtend3D), isLoop, volume);
        }

        /// <summary>
        /// 播放指定key值的背景音频(2D)
        /// </summary>
        /// <param name="clipName">音频的key</param>
        /// <param name="loop">是否循环</param>
        /// <param name="volume">音量</param>
        /// <param name="playFinish">完成回调</param>
        /// <param name="time">指定位置开始播放</param>
        private uint tmpBGTimerId = 0;
        public void PlayBgMusic(string clipName, bool loop, float volume, System.Action playFinish, float time)
        {
            try
            {
                if (mainBGSource == null)
                {
                    mainBGSource = new Sources2D();
                    mainBGSource.SetSource(this.transform);
                }

                if (mainBGSource.aSource.isPlaying)
                {
                    mainBGSource.Stop();
                }
                if (tmpBGTimerId != 0)
                {
                    Timer.instance.RemoveTimer(tmpBGTimerId);
                    tmpBGTimerId = 0;
                }

                AudioClip audioClip = soundClips[clipName].clipSoundData;
                mainBGSource.clipName = clipName;
                mainBGSource.SetData(loop, GetVolumeForWeight(soundClips[clipName].clipVolume) * volume * bgVolume);
                if (time > 0)
                    mainBGSource.Play(audioClip, time);
                else
                    mainBGSource.Play(audioClip);
                //播放完成事件
                if (playFinish != null)
                {
                    tmpBGTimerId = Timer.instance.AddOnce(audioClip.length - time, () =>
                    {
                        tmpBGTimerId = 0;
                        if (playFinish != null) playFinish();
                    });
                }
            }
            catch (System.Exception e)
            {
                Debuger.Log(string.Format("音频报错,{0}, 原因:{1}", clipName, e.ToString()));
                if (playFinish != null) playFinish();
            }
        }
        public void PlayBgMusic(string clipName)
        {
            this.PlayBgMusic(clipName, true, gameVolume, null, 0);
        }
        public void PlayBgMusic(string clipName, bool loop, float volume)
        {
            this.PlayBgMusic(clipName, loop, volume, null, 0);
        }
        public void PlayBgMusic(string clipName, System.Action playFinish)
        {
            this.PlayBgMusic(clipName,true, gameVolume, playFinish, 0);
        }
        /// <summary>
        /// 设置背景声音音量
        /// </summary>
        /// <param name="volume">音量[0,1]</param>
        public void SetBgMusicVolume(float volume)
        {
            if(mainBGSource != null)
            {
                mainBGSource.SetVolume(GetVolumeForWeight(soundClips[mainBGSource.clipName].clipVolume) * volume);
            }
        }
        /// <summary>
        /// 背景音乐播放位置
        /// </summary>
        public float GetBgMusicPlayTime()
        {
            if (mainBGSource == null)
            {
                return -1;
            }
            return mainBGSource.GetPlayTime();
        }

        public void StopPlayBgSound()
        {
            if (mainBGSource == null)
                return;
            if (mainBGSource.aSource.isPlaying)
            {
                mainBGSource.Stop();
            }
        }

        public void AudioScale(float radio)
        {
            for (int i = 0; i < idleSources2D.Count; i++)
            {
                idleSources2D[i].aSource.pitch = radio;
            }

            for (int i = 0; i < idleSources3D.Count; i++)
            {
                idleSources3D[i].aSource.pitch = radio;
            }

            for (int i = 0; i < useSources.Count; i++)
            {
                useSources[i].aSource.pitch = radio;
            }
        }

        IEnumerator ListanceUseSource()
        {
            while (true)
            {
                if (useSources.Count > 0)
                {
                    for (int i = 0; i < useSources.Count; i++)
                    {
                        Source2D source = useSources[i];

                        if (source.aSource != null)
                        {
                            if (useSources[i].PlayDone())
                            {
                                source.Stop();
                                useSources.Remove(source);

                                if (source.is3D)
                                {
                                    if (idleSources3D.Count >= maxCount)
                                        Destroy(source.aSource.gameObject);
                                    else
                                        idleSources3D.Add(source);
                                }
                                else
                                {
                                    if (idleSources2D.Count >= maxCount)
                                        Destroy(source.aSource);
                                    else
                                    {
                                        idleSources2D.Add(source);
                                    }
                                }

                                source = null;
                            }
                        }
                        else
                        {
                            useSources.Remove(source);
                            source = null;
                        }
                    }
                }
                
                yield return null;
            }
        }

        #endregion


        #region 全部音效暂停和恢复播放事件
        /// <summary>
        /// 暂停音效
        /// </summary>
        public void Pause()
        {
            foreach (var item in useSources)
            {
                item.Pause();
            }
            subtitleSource.Pause();
        }

        /// <summary>
        /// 恢复暂停
        /// </summary>
        public void Replay()
        {
            foreach (var item in useSources)
            {
                item.Play();
            }
            subtitleSource.Play();
        }
        #endregion


        #region 停止播放音效和删除正在播放的音效组件
        /// <summary>
        /// 删除正在播放的音效组件
        /// </summary>
        public void DestroySelf()
        {
            foreach (var item in useSources)
            {
                item.DestroySelf();
            }
            useSources.Clear();
        }

        /// <summary>
        /// 停止播放音效
        /// </summary>
        public void Stop()
        {
            foreach (var item in useSources)
            {
                if (item != null)
                    item.Stop();
            }
        }

        public void Stop(string clipName)
        {
            foreach (var item in useSources)
            {
                if (item.aSource != null)
                {
                    if (item.aSource.clip != null)
                    {
                        if (item.aSource.clip.name == clipName)
                        {
                            item.Stop();
                        }
                    }
                }
            }
        }

        #endregion


        #region 播放字幕音频

        int currentDepth = int.MaxValue;
        Source2D subtitleSource;
        Coroutine subCor = null;
        System.Action audioDone;

        public void PlaySubtitle(string clipName)
        {
            PlaySubtitle(clipName,null);
        }

        public void PlaySubtitle(string clipName,System.Action action)
        {

            //if (GameManage.Instance.isIdleMovie)
            //    return;

            //Debuger.Log("字幕："+ clipName+"_Depth:"+(subtitleClips[clipName].clipDepth < currentDepth));

            if (subtitleClips[clipName].isBreak)
            {
                if (subtitleClips[clipName].clipDepth < currentDepth)
                {
                    Clip(clipName, action);
                }
            }
            else
            {
                Clip(clipName, action);
            }
        }

        void Clip(string clipName, Action action)
        {
            if (subtitleClips[clipName].IsCD())
            {
                if (subtitleSource == null)
                {
                    if (idleSources2D.Count == 0)
                    {
                        Sources2D temp = new Sources2D();
                        temp.SetSource(transform);
                        idleSources2D.Add(temp);
                    }

                    subtitleSource = idleSources2D[0];
                    idleSources2D.Remove(subtitleSource);
                }


                if (subtitleSource.aSource.isPlaying)
                {
                    subtitleSource.Stop();
                    StopCoroutine(subCor);
                }

                if (mainBGSource != null)
                {
                    if (mainBGSource.aSource.isPlaying)
                    {
                        mainBGSource.SetData(mainBGSource.aSource.loop, this.GetVolumeForWeight(0.6f) * bgVolume);
                    }
                }

                isSubtitling = true;
                int lang_index = this.GameLanguage;
                if(lang_index >= subtitleClips[clipName].clips.Count)
                    lang_index = 0;
                subtitleSource.aSource.clip = subtitleClips[clipName].clips[lang_index];
                subtitleSource.aSource.volume = subtitleClips[clipName].clipVolume;
                subtitleSource.Play();

                audioDone = action;

                //GameManage.Instance.AudioTimeRefresh();

                currentDepth = subtitleClips[clipName].clipDepth;

                subCor = StartCoroutine(ListanseSubtitle());
            }
        }

        float GetVolumeForWeight(float startVolume)
        {
            if (subtitleSource == null)
                return startVolume;
            return subtitleSource.aSource.isPlaying == false ? startVolume : startVolume * VolumeWeight;
        }


        IEnumerator ListanseSubtitle()
        {
            for (int i = 0; i < useSources.Count; i++)
            {
                if (useSources[i].aSource != null && useSources[i].aSource.clip != null)
                    useSources[i].aSource.volume = soundClips[useSources[i].aSource.clip.name].clipVolume * VolumeWeight;
            }

            while (subtitleSource.aSource.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }

            for (int i = 0; i < useSources.Count; i++)
            {
                useSources[i].aSource.volume /= VolumeWeight;
            }

         
            isSubtitling = false;
            currentDepth = int.MaxValue;
            if (audioDone != null) audioDone();
            if (mainBGSource != null)
            {
                if (mainBGSource.aSource.isPlaying)
                {
                    mainBGSource.SetData(mainBGSource.aSource.loop, this.GetVolumeForWeight(1.0f) * bgVolume);
                }
            }          
        }


        public void StopSubtitle()
        {
            Debuger.Log("停止掉音频");
            if (subtitleSource != null)
            {
                if (subtitleSource.aSource)
                {
                    if (subtitleSource.aSource.isPlaying)
                    {
                        isSubtitling = false;
                        currentDepth = int.MaxValue;
                        subtitleSource.Stop();
                        StopCoroutine(subCor);
                    }
                }
            }
        }
        #endregion
        
        #region 加载音频文件
        /// <summary>
        /// 加载音频文件（音效和字幕）
        /// </summary>
        /// <param name="isAsync">是否异步加载</param>
        public void LoadAllClip(bool isAsync = false)
        {
            StartCoroutine(WaitLoadComplete());

            AudioSubtitleData subData = Resources.Load<AudioSubtitleData>("AssetData/SubtitleAsset");
            int index = 0;
            try
            {     
                
                for (int i = 0; i < subData.audioLists.Count; i++)
                {
                    index = i;
                    subtitleClips.Add(subData.audioLists[i].clipName, subData.audioLists[i]);
                }
            }
            catch (System.Exception e)
            {
                Debuger.LogError("LLL: "+ subData.audioLists[index].clipName);
                Debuger.LogError("不存在字幕音效配置文件SubtitleAsset:" + e+ " : "+ subData.audioLists[index].clipName);
                return;
            }


            AudioSoundData soundData = Resources.Load<AudioSoundData>("AssetData/SoundAsset");
            try
            {
           
                for (int i = 0; i < soundData.audioLists.Count; i++)
                {
                    soundClips.Add(soundData.audioLists[i].clipName, soundData.audioLists[i]);
                }
            }
            catch (System.Exception e)
            {
                Debuger.LogError("不存在音效配置文件SoundAsset:" + e);
                return;
            }

            if (isAsync == false)
            {
                foreach (var item in subtitleClips.Values)
                {
                    for (int i = 0; i < item.clips.Count; i++)
                    {
                        try
                        {
                            item.clips[i].LoadAudioData();
                        }
                        catch (System.Exception e)
                        {
                            Debuger.LogError(string.Format("{0}_中英文资源不对等----{1}", item.clips[i], e));
                        }
                    }
                    item.Replace();
                }


                AudioClip[] sudClips = Resources.LoadAll<AudioClip>(soundData.urlPath);

                for (int i = 0; i < sudClips.Length; i++)
                {
                    if (soundClips.ContainsKey(sudClips[i].name))
                    {
                        soundClips[sudClips[i].name].clipSoundData = sudClips[i];
                    }
                }

                isLoadComplete = true;
            }
            else
            {
                StartCoroutine(LoadAsync());
            }
        }

        IEnumerator LoadAsync()
        {
            foreach (var item in soundClips.Values)
            {
                ResourceRequest clip = Resources.LoadAsync(item.clipSoundUrl);
                yield return clip;
                item.clipSoundData = (AudioClip)clip.asset;
            }

            foreach (var item in subtitleClips.Values)
            {
                for (int i = 0; i < item.clipPaths.Count; i++)
                {
                    ResourceRequest clip = Resources.LoadAsync(item.clipPaths[i]);
                    yield return clip;
                    item.clips.Add((AudioClip)clip.asset);
                    item.Replace();
                }          
            }

            isLoadComplete = true;
        }

        IEnumerator WaitLoadComplete()
        {
            while (true)
            {
                if (isLoadComplete == true)
                    break;
                yield return new WaitForEndOfFrame();
            }

            InitSources2D();
            StartCoroutine(ListanceUseSource());

            if (completeEvent != null) completeEvent();

            Debuger.Log("音频加载完毕！");
        }

        #endregion



        private void OnDestroy()
        {
            idleSources2D.Clear();
            idleSources3D.Clear();

            useSources.Clear();

            soundClips.Clear();
            subtitleClips.Clear();
            instance = null;
        }
    }

}
