using System;
using System.Collections.Generic;

namespace fs
{
    /// <summary>
    /// 2d位置
    /// </summary>
    public struct Position2D
    {
        public int x;
        public int y;

        public Position2D(int _x, int _y)
        {
            x = _x; y = _y;
        }
        public Position2D Set(int _x, int _y)
        {
            x = _x; y = _y;
            return this;
        }
        public static bool operator ==(Position2D pos1, Position2D pos2)
        {
            return pos1.x == pos2.x && pos1.y == pos2.y;
        }
        public static bool operator !=(Position2D pos1, Position2D pos2)
        {
            return pos1.x != pos2.x || pos1.y != pos2.y;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public void Read(ByteArray by)
        {
            x = by.ReadInt();
            y = by.ReadInt();
        }
        public void Write(ByteArray by)
        {
            by.WriteInt(x);
            by.WriteInt(y);
        }
    }
    /// <summary>
    /// 方向
    /// </summary>
    [System.Serializable]
    public enum eDirection
    {
        NONE = 0,
        RIGHT,
        RIGHT_DOWN,
        DOWN,
        LEFT_DOWN,
        LEFT,
        LEFT_UP,
        UP,
        RIGHT_UP,
        MID,
    }

    /// <summary>
    /// 水平方向
    /// </summary>
    [System.Serializable]
    public enum eHorizontalType
    {
        LEFT,
        CENTER,
        RIGHT,
    }
    /// <summary>
    /// 对齐方式
    /// </summary>
    [System.Serializable]
    public enum eAligeType
    {
        NONE = 0,
        RIGHT,
        RIGHT_BOTTOM,
        BOTTOM,
        LEFT_BOTTOM,
        LEFT,
        LEFT_TOP,
        TOP,
        RIGHT_TOP,
        MID,
    }
    /// <summary>
    /// 8朝向
    /// </summary>
    [System.Serializable]
    public enum eFace8Type
    {
        NONE,
        RIGHT,		//正右方
        RIGHT_UP,	//右上
        UP,			//上
        LEFT_UP,	//左上
        LEFT,		//左
        LEFT_DOWN,	//左下方
        DOWN,		//下方
        RIGHT_DOWN,	//右下方

        MAX,		//最大值
    }
    /// <summary>
    /// 4朝向
    /// </summary>
    [System.Serializable]
    public enum eFace4Type
    {
        NONE,
        RIGHT,		//正右方
        UP,			//上
        LEFT,		//左
        DOWN,		//下方

        MAX,		//最大值
    }
}
