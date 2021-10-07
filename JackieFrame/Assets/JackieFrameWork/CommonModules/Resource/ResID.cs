using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 资源加载类型
    /// </summary>
    public enum eAssetType
    {
        Unknow = 0,     // 未知资源
        Scene,          // 场景
        Atlas,          // 拼图模式
        Prefab,         // 预制体
        Controller,     // 动画控制器
        Material,       // 材质
        Shader,         // Shader
        BinaryData,     // 数据
        TextData,       // 数据
        Audio,          // 声音
        Font,           // 字体
        Texture,        // 贴图
        Sprite,         // 精灵
    }
}