using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace fs
{
    /// <summary>
    /// 界面动画
    /// @author hannibal
    /// @time 2016-12-11
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UISpriteAnimation : UIComponentBase
    {
        public List<Sprite> SpriteFrames;       //存放图片的列表
        public float FPS = 12;                  //帧数
        public bool AutoPlay = true;            //是否自动播放
        public bool Loop = true;                //是否循环播放
        public bool Foward = true;              //是否是正常顺序播放
        public bool SetNativeSize = true;       //是否为原始比例
        public bool SelfUpdate = true;          //      
        private int mCurFrame = 0;              //当前帧数
        private float mDelta = 0;
        private bool mIsPlaying = false;        //是否正在播放
        /// <summary>
        /// 完成回调
        /// </summary>
        private Action mCompleteFun = null;

        private Image m_ImgComponent = null;
        private Image ImgComponent
        {
            get
            {
                if (m_ImgComponent == null) m_ImgComponent = GetComponent<Image>();
                return m_ImgComponent;
            }
        }

        void Start()
        {
            if (AutoPlay)
            {
                this.Play();
            }
            else
            {
                mIsPlaying = false;
            }
        }

        protected override void OnEnable()
        {
            if (AutoPlay)
            {
                this.Rewind();
            }
            else
            {
                mIsPlaying = false;
            }
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            this.Stop();
            base.OnDisable();
        }

        void Update()
        {
            if(SelfUpdate)
            {
                Simulate(Time.deltaTime);
            }
        }

        public void Simulate(float detal_time)
        {
            if (!mIsPlaying || 0 == FrameCount)
            {
                return;
            }
            mDelta += detal_time;
            if (mDelta > 1 / FPS)
            {
                mDelta = 0;
                if (Foward)
                {
                    mCurFrame++;
                }
                else
                {
                    mCurFrame--;
                }
                if (mCurFrame >= FrameCount)
                {
                    if (Loop)
                    {
                        mCurFrame = 0;
                    }
                    else
                    {
                        this.OnStop();
                        return;
                    }
                }
                else if (mCurFrame < 0)
                {
                    if (Loop)
                    {
                        mCurFrame = FrameCount - 1;
                    }
                    else
                    {
                        this.OnStop();
                        return;
                    }
                }
                SetSprite(mCurFrame);
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            mIsPlaying = true;
        }

        /// <summary>
        /// 重新播放
        /// </summary>
        public void Rewind()
        {
            if (Foward)
                mCurFrame = 0;
            else
                mCurFrame = FrameCount - 1;
            SetSprite(mCurFrame);
            Play();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            mCurFrame = 0;
            SetSprite(mCurFrame);
            mIsPlaying = false;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            mIsPlaying = false;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void Resume()
        {
            if (!mIsPlaying)
            {
                mIsPlaying = true;
            }
        }

        /// <summary>
        /// 完成回调，针对非循环
        /// </summary>
        /// <param name="fun"></param>
        public void OnComplete(Action fun)
        {
            mCompleteFun = fun;
        }

        /// <summary>
        /// 停止播放时
        /// </summary>
        private void OnStop()
        {
            mIsPlaying = false;
            if (mCompleteFun != null)
            {
                mCompleteFun();
            }
        }

        /// <summary>
        /// 设置帧动画的图片
        /// </summary>
        /// <param name="idx">帧动画数组中的索引值</param>
        private void SetSprite(int idx)
        {
            if (SpriteFrames == null || idx < 0 || idx >= SpriteFrames.Count) return;
            // Debuger.Log("当前播放帧:" + idx);
            ImgComponent.sprite = SpriteFrames[idx];
            if (SetNativeSize) ImgComponent.SetNativeSize();
        }

        /// <summary>
        /// 返回帧动画的帧数量
        /// </summary>
        public int FrameCount
        {
            get
            {
                return SpriteFrames.Count;
            }
        }

        /// <summary>
        /// 判断是否正在播放帧动画
        /// </summary>
        public bool IsPlaying
        {
            get { return mIsPlaying; }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UISpriteAnimation))]
    public class UISpriteAnimationInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UISpriteAnimation owner_script = (UISpriteAnimation)target;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("预览"))
            {
                EditorApplication.update += Update;
                owner_script.Play();
            }
            if (GUILayout.Button("停止"))
            {
                EditorApplication.update -= Update;
                owner_script.Stop();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Update()
        {
            UISpriteAnimation owner_script = (UISpriteAnimation)target;
            owner_script.Simulate(Time.deltaTime);
        }
    }
#endif
}