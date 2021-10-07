using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace fs
{
    public class SourceExtend3D : MonoBehaviour
    {
        private AudioSource _source;
        /// <summary>
        /// 自身的音频播放组件
        /// </summary>
        public AudioSource source
        {
            get
            {
                if (_source == null)
                    _source = this.gameObject.GetComponent<AudioSource>();
                return _source;
            }
        }

        private AudioListener _listener;
        /// <summary>
        /// 游戏的音频侦听组件
        /// </summary>
        public AudioListener listener
        {
            get
            {
                if (_listener == null)
                    _listener = GameObject.FindObjectOfType<AudioListener>();
                return _listener;
            }
        }


        [SerializeField] private float radius = 6;
        [SerializeField] private float maxRadius = 60;

        public void SetRadius(float radius, float maxRadius)
        {
            this.radius = radius;
            this.maxRadius = maxRadius;
        }

        float currentDis = 0;
        float lastDis = 0;


        void Update()
        {
            ComputeSpatialBlend();
        }

        public virtual void ComputeSpatialBlend()
        {
            currentDis = Vector3.Distance(source.transform.position, listener.transform.position);

            if (lastDis != currentDis)
            {
                if (currentDis > radius)
                    source.spatialBlend = Mathf.Clamp01(currentDis / maxRadius);
                else
                    source.spatialBlend = 0;
                lastDis = currentDis;
            }
        }
    }

}
