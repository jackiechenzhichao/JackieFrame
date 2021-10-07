using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    public class AudioSubtitleData : ScriptableObject
    {
        public string urlPath;
        public List<SubtitleData> audioLists;
    }


    [System.Serializable]
    public class SubtitleData
    {
        public string clipName;

        public Dictionary<string, int> records = new Dictionary<string, int>();

        public List<AudioClip> clips;
        public List<string> clipPaths;

        public bool isBreak = true;

        public int clipCD = -1;
        public int clipDepth = 1;
        public float clipVolume = 1;



        float time = 0;

        public void Replace()
        {
            time = 0;
        }

        public bool IsCD()
        {

            if (Time.time - time >= clipCD || time <= 0 || clipCD < 0)
            {
                time = Time.time;
                return true;
            }
            else
                return false;
        }
    }
}