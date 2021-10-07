
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：动画工具
// 创建时间：2019-10-31 15:44:09
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
	public static class AnimatorUtils 
	{
        #region 播放
        public static void Play(this Animator anim, string stateName, System.Action callback)
        {
            anim.Play(stateName);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, ()=> { if (callback != null) callback(); });
            }
        }
        public static void Play(this Animator anim, string stateName, int layer, System.Action callback)
        {
            anim.Play(stateName, layer);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, () => { if (callback != null) callback(); });
            }
        }
        public static void Play(this Animator anim, string stateName, int layer, float normalizedTime, System.Action callback)
        {
            anim.Play(stateName, layer, normalizedTime);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, () => { if (callback != null) callback(); });
            }
        }
        public static void PlayInFixedTime(this Animator anim, string stateName, System.Action callback)
        {
            anim.PlayInFixedTime(stateName);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, () => { if (callback != null) callback(); });
            }
        }
        public static void PlayInFixedTime(this Animator anim, string stateName, int layer, System.Action callback)
        {
            anim.PlayInFixedTime(stateName, layer);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, () => { if (callback != null) callback(); });
            }
        }
        public static void PlayInFixedTime(this Animator anim, string stateName, int layer, float normalizedTime, System.Action callback)
        {
            anim.PlayInFixedTime(stateName, layer, normalizedTime);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, () => { if (callback != null) callback(); });
            }
        }
        #endregion

        #region 停止
        public static void StopToFirst(this Animator anim)
        {
            if (anim.runtimeAnimatorController == null)
            {
                Debuger.LogWarning("没有找到动画控制器");
                return;
            }

            AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
            for (int j = 0; j < clips.Length; ++j)
            {
                AnimationClip clip = clips[j];
                clip.SampleAnimation(anim.gameObject, 0);
            }
            anim.enabled = false;
        }
        #endregion

        #region 动画时间
        /// <summary>
        /// 动画时长
        /// </summary>
        public static float GetAnimTime(this Animator anim, string pose_name)
        {
            if (anim.runtimeAnimatorController == null)
            {
                Debuger.LogWarning("没有找到动画控制器");
                return 0;
            }

            float total_time = 0;
            AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
            for (int j = 0; j < clips.Length; ++j)
            {
                AnimationClip clip = clips[j];
                if (clip.name.Equals(pose_name, System.StringComparison.OrdinalIgnoreCase))
                {
                    total_time = clip.length;
                    break;
                }
            }
            return total_time;
        }
        public static float GetAnimTime(this Animator anim)
        {
            if (anim.runtimeAnimatorController == null)
            {
                Debuger.LogWarning("没有找到动画控制器");
                return 0;
            }

            float total_time = anim.GetCurrentAnimatorStateInfo(0).length;
            return total_time;
        }
        public static float GetAnimTime(this GameObject obj, string pose_name)
        {
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null)
            {
                Debuger.LogWarning("没有找到动画脚本");
                return 0;
            }
            return anim.GetAnimTime(pose_name);
        }
        public static float GetAnimTime(this GameObject obj)
        {
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null)
            {
                Debuger.LogWarning("没有找到动画脚本");
                return 0;
            }
            return anim.GetAnimTime();
        }
        public static float GetAnimTime(this Transform obj, string pose_name)
        {
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null)
            {
                Debuger.LogWarning("没有找到动画脚本");
                return 0;
            }
            return anim.GetAnimTime(pose_name);
        }
        public static float GetAnimTime(this Transform obj)
        {
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null)
            {
                Debuger.LogWarning("没有找到动画脚本");
                return 0;
            }
            return anim.GetAnimTime();
        }
        #endregion
    }
}
