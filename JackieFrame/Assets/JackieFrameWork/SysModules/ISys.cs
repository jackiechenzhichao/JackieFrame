/****************************************************
  *  Copyright © DefaultCompany All rights reserved.
 *------------------------------------------------------------------------
 *  作者：JackieChen
 *  邮箱: Jackiechenzhichao@163.com
 *  日期：2021/10/4 16:16:14
 *  项目：JackieFrame
 *  功能：系统基类
*****************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jackie
{
    public abstract class ISys
    {
        public abstract void OnInit();

        public abstract void OnUpdate();

        public abstract void OnEnd();
       
    }
}
