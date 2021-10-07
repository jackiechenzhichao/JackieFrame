using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace fs
{
    public class UIGridWrapContentConfig
    {
        public int mDataCnt = 0;
        public Action<int, GameObject> mDisplayCellAction = null;
        public Action<int, GameObject> mConcealCellAction = null;//Òþ²Ø»Øµ÷

        public Func<GameObject> mCreateFunc = null;

        public GameObject CreateCell()
        {
            if (mCreateFunc == null)
                return null;

            return mCreateFunc();
        }
    }

}