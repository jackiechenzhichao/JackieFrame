using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 协程；使用必须挂接到场景对象
    /// @author hannibal
    /// @time 2014-12-1
    /// </summary>
    public class CoroutineManager : DnotMonoSingleton<CoroutineManager>
    {
        public delegate void Fun(object info);

        private Dictionary<string,Coroutine> coroutines=new Dictionary<string, Coroutine>();
        
        /// <summary>
        /// 延迟回调
        /// </summary>
        public void Add(float time, Fun fun, object info)
        {
            StartCoroutine(HandleFun(time, fun, info));
        }
       
       /// <summary>
        /// 执行一个函数
       /// </summary>
       /// <param name="coroutineFunc"></param>
       /// <param name="isCancel">是否可以取消</param>
        
        public void Add(IEnumerator coroutineFunc,bool isCancel=false)
        {
           if (!isCancel)
           {
               StartCoroutine(coroutineFunc);
           }
           else
           {
               
               if (!coroutines.ContainsKey(coroutineFunc.ToString()))
               {
                   coroutines.Add(coroutineFunc.ToString(),StartCoroutine(coroutineFunc));
               }
           }
        }

        

        /// <summary>
        /// 取消一个协程
        /// </summary>
        /// <param name="coroutineFunc"></param>
        public void Cancel(IEnumerator coroutineFunc)
        {
            if (coroutines.ContainsKey(coroutineFunc.ToString()))
            {
                StopCoroutine(coroutines[coroutineFunc.ToString()]);
                coroutines.Remove(coroutineFunc.ToString());
            }
           
        }

        IEnumerator HandleFun(float time, Fun fun, object info)
        {
            yield return new WaitForSeconds(time);
            if (fun != null) fun(info);
        }
    }
}