using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace fs
{
    public class Source2D
    {
        public AudioSource aSource;
        public string clipName;
        public bool is3D = false;

        public virtual void SetSource(Transform transform)
        {
            aSource.clip = null;
            aSource.playOnAwake = false;
            aSource.loop = false;
            aSource.volume = 1;
        }

        public virtual void SetData(bool isLoop, float volume)
        {
            aSource.loop = isLoop;
            aSource.volume = volume;
        }

        public virtual void SetVolume(float volume)
        {
            aSource.volume = volume;
        }

        public virtual void ClearClip()
        {
            isPause = false;
            aSource.Stop();
            aSource.clip = null;
        }

        public virtual void DestroySelf()
        {
            isPause = false;
            aSource.Stop();
            aSource.clip = null;
        }


        bool isPause = false;

        public void Play()
        {
            isPause = false;
            aSource.time = 0;
            aSource.Play();
        }
        public void Play(float time)
        {
            isPause = false;
            aSource.time = time;
            aSource.Play();
        }

        public void Play(AudioClip audioClip)
        {
            isPause = false;
            aSource.time = 0;
            aSource.clip = audioClip;
            aSource.Play();
        }
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="time">开始播放位置</param>
        public void Play(AudioClip audioClip, float time)
        {
            isPause = false;
            aSource.clip = audioClip;
            aSource.time = time;
            aSource.Play();
        }

        public void Pause()
        {
            isPause = true;
            aSource.Pause();
        }

        public void Stop()
        {
            ClearClip();
        }


        public bool PlayDone()
        {
            if (aSource.isPlaying == false && isPause == false)
                return true;
            return false;
        }
        /// <summary>
        /// 播放位置(s)
        /// </summary>
        /// <returns></returns>
        public float GetPlayTime()
        {
            return aSource.time;
        }

        public virtual void SetListanceData(float radius, float maxRadius) { }
        public virtual void Init() { }

    }

    public class Sources2D : Source2D
    {
        public override void SetSource(Transform transform)
        {
            aSource = transform.gameObject.AddComponent<AudioSource>();
            base.SetSource(transform);
        }

        public override void DestroySelf()
        {
            base.DestroySelf();
            MonoBehaviour.Destroy(aSource);
        }
    }

    public class Sources3D : Source2D
    {

        public GameObject game3D;

        public Sources3D(System.Type type)
        {
            game3D = new GameObject("Game3D", type);
        }

        public override void SetSource(Transform transform)
        {
            game3D.transform.SetParent(transform);
            game3D.transform.localPosition = new Vector3(0, 0, 0);
            if (aSource == null) aSource = game3D.AddComponent<AudioSource>();
            is3D = true;
            base.SetSource(transform);
        }

        public override void SetListanceData(float radius, float maxRadius)
        {
            SourceExtend3D sourceExtend3D = game3D.GetComponent<SourceExtend3D>();
            sourceExtend3D.SetRadius(radius, maxRadius);
        }

        public override void DestroySelf()
        {
            base.DestroySelf();
            MonoBehaviour.Destroy(game3D);
        }

    }
}
