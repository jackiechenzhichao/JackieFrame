using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace fs
{
    public class AudioSoundData : ScriptableObject
    {
        public string urlPath;
        public List<SoundData> audioLists;
    }


    [System.Serializable]
    public class SoundData
    {
        public string clipName;
        public string clipSoundUrl;
        public AudioClip clipSoundData;

        public int clipCD = -1;
        public float clipVolume = 1;



        float time = 0;
        public bool IsCD()
        {
            if (Time.time - time >= clipCD || clipCD < 0)
            {
                time = Time.time;
                return true;
            }
            else
                return false;
        }
    }
}


