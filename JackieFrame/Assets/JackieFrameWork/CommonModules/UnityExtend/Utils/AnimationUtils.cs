
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
	public static class AnimationUtils
    {
        public static void Play(this Animation anim, string stateName, System.Action callback)
        {
            anim.Play(stateName);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, ()=> { if (callback != null) callback(); });
            }
        }
        public static void Play(this Animation anim, string stateName, PlayMode mode, System.Action callback)
        {
            anim.Play(stateName, mode);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, () => { if (callback != null) callback(); });
            }
        }
        public static void CrossFade(this Animation anim, string stateName, System.Action callback)
        {
            anim.CrossFade(stateName);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, () => { if (callback != null) callback(); });
            }
        }
        public static void CrossFade(this Animation anim, string stateName, float fadeLength, System.Action callback)
        {
            anim.CrossFade(stateName, fadeLength);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, () => { if (callback != null) callback(); });
            }
        }
        public static void CrossFade(this Animation anim, string stateName, float fadeLength, PlayMode mode, System.Action callback)
        {
            anim.CrossFade(stateName, fadeLength, mode);
            if (callback != null)
            {
                float time = anim.GetAnimTime(stateName);
                Timer.instance.AddOnce(time, () => { if (callback != null) callback(); });
            }
        }

        /// <summary>
        /// 动画时长
        /// </summary>
        public static float GetAnimTime(this Animation anim, string pose_name)
        {
            float total_time = 0;
            foreach (AnimationState state in anim)
            {
                if (anim.name.Equals(pose_name, System.StringComparison.OrdinalIgnoreCase))
                {
                    total_time = state.length;
                    break;
                }
            }
            return total_time;
        }
        public static float GetAnimTime(this Animation anim)
        {
            float total_time = 0;
            foreach (AnimationState state in anim)
            {
                if (anim.IsPlaying(state.name) && state.length > total_time)
                    total_time = state.length;
            }
            return total_time;
        }
    }
}
