
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：事件接口
// 创建时间：2019-07-24 10:53:27
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
	public interface IEvent 
	{
        void RegisterEvent();
        void UnRegisterEvent();
    }
}
