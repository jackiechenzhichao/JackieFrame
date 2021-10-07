using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    public static class UIHelper
    {
        /// <summary>
        /// 判断屏幕坐标是否在UGUI控件矩形区域
        /// </summary>
        /// <param name="rt">ugui容器</param>
        /// <param name="screen_pt">屏幕坐标</param>
        /// <returns></returns>
        public static bool IsPointInRect(RectTransform rt, Vector2 screen_pt, Camera ui_camera)
        {
            Vector2 local_pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screen_pt, ui_camera, out local_pos);
            if (rt.rect.Contains(local_pos))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 屏幕坐标转世界坐标
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="ui_camera"></param>
        /// <param name="screen_pt"></param>
        /// <returns></returns>
        public static Vector3 Screen2WorldPos(Canvas canvas, Camera ui_camera, Vector3 screen_pt)
        {
            Vector3 w_pt;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, screen_pt, ui_camera, out w_pt);
            return w_pt;
        }

        #region 添加组件
        public static T GetComponent<T>(GameObject go, bool auto_add = true) where T : Component
        {
            T ret = go.GetComponent<T>();
            if (ret == null && auto_add) ret = go.AddComponent<T>();
            return ret;
        }
        public static T GetComponent<T>(GameObject go, Type classType, bool auto_add = true) where T : Component
        {
            T ret = go.GetComponent(classType) as T;
            if (ret == null && auto_add) ret = go.AddComponent(classType) as T;
            return ret;
        }
        public static void RemoveAllChild(Transform Target)
        {
            while (Target.childCount > 0)
            {
                Transform obj = Target.GetChild(0);
                obj.SetParent(null);
                GameObject.Destroy(obj.gameObject);
            }
        }
        #endregion

        #region 事件
        public static void AddEventListener(Transform parent, string obj_name, eUIEventType type, UIEventListener.EventDelegate callBack, eUIEventPriority priority = eUIEventPriority.Normal)
        {
            if (parent == null)
            {
                Debuger.LogError("参数错误");
                return;
            }
            AddEventListener(parent.GetChild(obj_name).gameObject, type, callBack, priority);
        }
        public static void AddEventListener(GameObject obj, eUIEventType type, UIEventListener.EventDelegate callBack, eUIEventPriority priority = eUIEventPriority.Normal)
        {
            if (obj == null)
            {
                Debuger.LogError("参数错误");
                return;
            }
            UIEventListener.Get(obj).AddEventListener(type, callBack, priority);
        }
        public static void RemoveEventListener(Transform parent, string obj_name, eUIEventType type, UIEventListener.EventDelegate callBack, eUIEventPriority priority = eUIEventPriority.Normal)
        {
            if (parent == null)
            {
                Debuger.LogError("参数错误");
                return;
            }
            RemoveEventListener(parent.GetChild(obj_name).gameObject, type, callBack, priority);
        }
        public static void RemoveEventListener(GameObject obj, eUIEventType type, UIEventListener.EventDelegate callBack, eUIEventPriority priority = eUIEventPriority.Normal)
        {
            if (obj == null)
            {
                Debuger.LogError("参数错误");
                return;
            }
            UIEventListener.Get(obj).RemoveEventListener(type, callBack, priority);
        }
        #endregion
    }
}