using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

namespace fs
{
    /// <summary>
    /// UI动画效果工具类
    /// @author hannibal
    /// @time 2016-12-2
    /// </summary>
    public class UIEffectTools
    {
        //～～～～～～～～～～～～～～～～～～～～～～～缩放动画~～～～～～～～～～～～～～～～～～～～～～～～//
        /// <summary>
        /// UI按下缩放动画
        /// </summary>
        /// <param name="receive_obj">接收事件对象</param>
        /// <param name="influence_obj">动画作用对象</param>
        /// <param name="time">缩放过程时间</param>
        /// <param name="scale">按下时的缩放比例</param>
        static public void AddPressScaleAnim(GameObject receive_obj, GameObject influence_obj, float time, float scale)
        {
            if (receive_obj == null || influence_obj == null) return;

            UIEventListener.Get(receive_obj).AddEventListener(eUIEventType.Down, delegate(UIEventArgs evt) { influence_obj.transform.DOScale(Vector3.one * scale, time); });
            UIEventListener.Get(receive_obj).AddEventListener(eUIEventType.Up, delegate(UIEventArgs evt) { influence_obj.transform.DOScale(Vector3.one, time); });
            UIEventListener.Get(receive_obj).AddEventListener(eUIEventType.Exit, delegate(UIEventArgs evt) { influence_obj.transform.DOScale(Vector3.one, time); });
        }
        static public void RemovePressScaleAnim(GameObject receive_obj)
        {
            if (receive_obj == null) return;

            UIEventListener.Get(receive_obj).ClearEventListener(eUIEventType.Down);
            UIEventListener.Get(receive_obj).ClearEventListener(eUIEventType.Up);
            UIEventListener.Get(receive_obj).ClearEventListener(eUIEventType.Exit);
        }
        //～～～～～～～～～～～～～～～～～～～～～～～渐隐动画~～～～～～～～～～～～～～～～～～～～～～～～//
        public static void FadeIn(GameObject go, float time, float alpha = 1, System.Action fun = null)
        {
            if (go == null)
            {
                fun();
                return;
            }
            bool is_trigger = false;
            Component[] comps = go.GetComponentsInChildren<Component>();
            for (int index = 0; index < comps.Length; index++)
            {
                Graphic c = comps[index] as Graphic;
                if (c != null)
                {
                    c.DOFade(alpha, time)
                        .OnComplete(() =>
                        {
                            if (fun != null && is_trigger == false)
                            {
                                fun();
                            }
                            is_trigger = true;
                        });
                }
            }
        }
        public static void FadeOut(GameObject go, float time, float alpha = 0, System.Action fun = null)
        {
            if (go == null)
            {
                fun();
                return;
            }
            bool is_trigger = false;
            Component[] comps = go.GetComponentsInChildren<Component>();
            for (int index = 0; index < comps.Length; index++)
            {
                Graphic c = comps[index] as Graphic;
                if (c != null)
                {
                    c.DOFade(alpha, time)
                        .OnComplete(() =>
                        {
                            if (fun != null && is_trigger == false)
                            {
                                fun();
                            }
                            is_trigger = true;
                        });
                }
            }
        }
        public static void FadeStop(GameObject go)
        {
            if (go == null) return;
            Component[] comps = go.GetComponentsInChildren<Component>();
            for (int index = 0; index < comps.Length; index++)
            {
                Graphic c = comps[index] as Graphic;
                if (c != null)
                {
                    c.DOKill();
                }
            }
        }
        /// <summary>
        /// 透明
        /// </summary>
        public static void SetAlpha(GameObject go, float alpha)
        {
            if (go == null) return;
            CanvasGroup canvas = GameObjectUtils.GetOrCreateComponent<CanvasGroup>(go);
            if (canvas != null)
            {
                canvas.alpha = alpha;
            }
        }
        //～～～～～～～～～～～～～～～～～～～～～～～缩放动画~～～～～～～～～～～～～～～～～～～～～～～～//
        public static void ScaleTo(GameObject go, float time, float scale = 1, System.Action fun = null)
        {
            if (go == null)
            {
                fun();
                return;
            }
            go.transform.DOScale(scale * Vector3.one, time).OnComplete(() =>
            {
                if (fun != null)
                {
                    fun();
                }
            });
        }
        //～～～～～～～～～～～～～～～～～～～～～～～移动动画~～～～～～～～～～～～～～～～～～～～～～～～//
        public static void MoveTo(GameObject go, float time, Vector3 target_pos, System.Action fun = null)
        {
            if (go == null)
            {
                fun();
                return;
            }
            go.transform.DOLocalMove(target_pos, time).OnComplete(() =>
            {
                if (fun != null)
                {
                    fun();
                }
            });
        }
        public static void Stop(GameObject go)
        {
            if (go == null)
            {
                return;
            }
            go.transform.DOKill();
        }
    }
}