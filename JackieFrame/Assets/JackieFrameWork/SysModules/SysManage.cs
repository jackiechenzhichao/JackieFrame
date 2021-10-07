/****************************************************
  *  Copyright © DefaultCompany All rights reserved.
 *------------------------------------------------------------------------
 *  作者：JackieChen
 *  邮箱: Jackiechenzhichao@163.com
 *  日期：2021/10/4 16:15:0
 *  项目：JackieFrame
 *  功能：系统总管理类
*****************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Jackie
{
    public static class SysManage
    {
        /// <summary>
        /// 所有系统集合
        /// </summary>
        private static Dictionary<Type, ISys> SysDic = new Dictionary<Type, ISys>();

        /// <summary>
        /// 创建系统
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static void CreateSys<T>() where T : ISys, new()
        {
            if (!SysDic.ContainsKey(typeof(T)))
            {
                T t = new T();
                SysDic.Add(typeof(T), t);
                t.OnInit();
            }
        }

        /// <summary>
        /// 获取系统
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetSys<T>() where T : ISys,new()
        {
            if (!SysDic.ContainsKey(typeof(T)))
            {
                CreateSys<T>();
            }
            
            return SysDic[typeof(T)] as T;
        }

        /// <summary>
        /// 更新系统
        /// </summary>
        public static  void UpdateSys()
        {
            foreach(ISys value in SysDic.Values)
            {
                value.OnUpdate();
            }
        }

        /// <summary>
        /// 结束系统
        /// </summary>
        public static void EndSys()
        {
            foreach (ISys value in SysDic.Values)
            {
                value.OnEnd();
            }
        }
      
    }
}
