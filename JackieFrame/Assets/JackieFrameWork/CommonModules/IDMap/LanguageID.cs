
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：语言
// 创建时间：2019-07-31 10:12:55
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fs
{
    [System.Serializable]
	public enum eLanguageType
	{
        /// <summary>
        /// 简体中文(中国)，值为0，不要修改
        /// </summary>
        zh_cn = 0,

        /// <summary>
        /// 英语
        /// </summary>
        en,
        /// <summary>
        /// 繁体中文(台湾地区)
        /// </summary>
        zh_tw,
   
        /// <summary>
        /// 韩文(韩国)
        /// </summary>
        ko,
        /// <summary>
        /// 日语(日本)
        /// </summary>
        ja,
        /// <summary>
        /// 法语(法国)
        /// </summary>
        fr,
        /// <summary>
        /// 西班牙语(西班牙)
        /// </summary>
        es,
        /// <summary>
        /// 德语(德国)
        /// </summary>
        de,
        /// <summary>
        /// 俄语(俄罗斯)
        /// </summary>
        ru,
        /// <summary>
        /// 意大利语(意大利)
        /// </summary>
        it,
        /// <summary>
        /// 阿拉伯语
        /// </summary>
        ar,
        /// <summary>
        /// 越南
        /// </summary>
        vi,
        /// <summary>
        /// 泰国
        /// </summary>
        th,
        /// <summary>
        /// 印度
        /// </summary>
        hi,
        /// <summary>
        /// 印尼
        /// </summary>
        id,
        /// <summary>
        /// 马来西亚
        /// </summary>
        ms,
        /// <summary>
        /// 蒙古
        /// </summary>
        mn,


        /// <summary>
        /// 放最后
        /// </summary>
        MAX,
    }
}
