/****************************************************************************
Copyright (c) 2015 Lingjijian

Created by Lingjijian on 2015

342854406@qq.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace fs
{
    /// <summary>
    /// 富文本的类型
    /// </summary>
    public enum RichType
    {
        TEXT,       //文本
        IMAGE,      //图片
        ANIM,       //动画
        NEWLINE,    //换行文本
    }


    class LRichElement : Object
    {
        public RichType type { get; protected set; }
        public Color color { get; protected set; }
        public string data { get; protected set; }
    }
    /// <summary>
    /// 文本元素
    /// </summary>
    class LRichElementText : LRichElement
    {
        public string txt { get; protected set; }
        public bool isUnderLine { get; protected set; }
        public bool isOutLine { get; protected set; }
        public int fontSize { get; protected set; }
        public Color outLineColor { get; protected set; }

        public LRichElementText(Color color, string txt, int fontSize, bool isUnderLine, bool isOutLine,Color outLineColor, string data)
        {
            this.type = RichType.TEXT;
            this.color = color;
            this.txt = txt;
            this.fontSize = fontSize;
            this.isUnderLine = isUnderLine;
            this.isOutLine = isOutLine;
            this.outLineColor = outLineColor;
            this.data = data;
        }
    }

    /// <summary>
    /// 图片元素
    /// </summary>
    class LRichElementImage : LRichElement
    {
        public string path { get; protected set; }
        public float scale { get; protected set; }

        public LRichElementImage(string path, string data, float scale)
        {
            this.type = RichType.IMAGE;
            this.path = path;
            this.data = data;
            this.scale = scale;
        }
    }

    /// <summary>
    /// 动画元素
    /// </summary>
    class LRichElementAnim : LRichElement
    {
        public string path { get; protected set; }
        public float fs { get; protected set; }

        public LRichElementAnim(string path, float fs, string data)
        {
            this.type = RichType.ANIM;
            this.path = path;
            this.data = data;
            this.fs = fs;
        }
    }

    /// <summary>
    /// 换行元素
    /// </summary>
    class LRichElementNewline : LRichElement
    {
        public LRichElementNewline()
        {
            this.type = RichType.NEWLINE;
        }
    }

    /// <summary>
    /// 缓存结构
    /// </summary>
    class LRichCacheElement : Object
    {
        public bool isUse;
        public GameObject node;
        public LRichCacheElement(GameObject node)
        {
            this.node = node;
        }
    }

    /// <summary>
    /// 渲染结构
    /// </summary>
    struct LRenderElement
    {
        public RichType type;           //文本类型
        public string strChar;
        public int width;               //宽带
        public int height;              //高度
        public bool isOutLine;          //是否行溢出
        public bool isUnderLine;
        public Color outLineColor;
        public Font font;               //字体
        public int fontSize;            //字号
        public Color color;             //颜色
        public string data;
        public string path;
        public float fs;
        public bool isNewLine;
        public Vector2 pos;
        public float scale;
        
        public LRenderElement Clone()
        {
            LRenderElement cloneOjb;
            cloneOjb.type = this.type;
            cloneOjb.strChar = this.strChar;
            cloneOjb.width = this.width;
            cloneOjb.height = this.height;
            cloneOjb.isOutLine = this.isOutLine;
            cloneOjb.isUnderLine = this.isUnderLine;
            cloneOjb.outLineColor = this.outLineColor;
            cloneOjb.font = this.font;
            cloneOjb.fontSize = this.fontSize;
            cloneOjb.color = this.color;
            cloneOjb.data = this.data;
            cloneOjb.path = this.path;
            cloneOjb.fs = this.fs;
            cloneOjb.isNewLine = this.isNewLine;
            cloneOjb.pos = this.pos;
            cloneOjb.scale = this.scale;
            return cloneOjb;
        }

		public bool isSameStyle(LRenderElement elem)
		{
			return (this.color 			== elem.color &&
				    this.isOutLine 		== elem.isOutLine &&
					this.isUnderLine 	== elem.isUnderLine &&
                    this.outLineColor   == elem.outLineColor &&
                    this.font 			== elem.font &&
					this.fontSize 		== elem.fontSize &&
					this.data 			== elem.data);
		}
    }

    /// <summary>
    /// 富文本
    /* 格式：
        "<lab txt=\"hello world!!\" color=#ffff00 data=数据 />" +
        "<lab txt=\"测试文本内容\" isUnderLine=true size=40 data=链接/>
        <anim path=Atlas/head_85_85/icon_charactor01.png fps=5.0/>" +
        "<newline /><img path=Atlas/head_85_85/icon_charactor01.png/>" +
        "<lab txt=\"The article comes from the point of the \" color=#ff0000 />" +
        "<lab txt=\"Examination\" color=#ff0000 isOutLine=true/>";
    */
    /// </summary>
    public class LRichText : MonoBehaviour, IPointerClickHandler
    {
        public TextAnchor alignment = TextAnchor.MiddleCenter;      //文本的对其方式，默认为正中心
        public int verticalSpace = 0;                               //垂直的间距
        public int maxLineWidth = 0;                                //最大行
        public Font font;                                           //字体

        public UnityAction<string> onClickHandler;
        public int realLineHeight { get; protected set; }
        public int realLineWidth { get; protected set; }
        
        List<LRichElement> _richElements = new List<LRichElement>();
        List<LRenderElement> _elemRenderArr = new List<LRenderElement>();
        List<LRichCacheElement> _cacheLabElements = new List<LRichCacheElement>();
        List<LRichCacheElement> _cacheImgElements = new List<LRichCacheElement>();
        List<LRichCacheElement> _cacheFramAnimElements = new List<LRichCacheElement>();
        Dictionary<GameObject, string> _objectDataMap = new Dictionary<GameObject, string>();
        //custom content parser setting
        public int defaultLabSize = 20;
        public string defaultLabColor = "#ff00ff";
        public bool raycastTarget = false;
        
        void Awake()
        {
            if (maxLineWidth == 0) maxLineWidth = (int)this.GetComponent<RectTransform>().sizeDelta.x;
        }

        public void reloadData()
        {
            this.removeAllElements();

            RectTransform rtran = this.GetComponent<RectTransform>();
            rtran.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            int len = _richElements.Count;
            for (int i =0;i< len;i++)
            {
                LRichElement elem = _richElements[i];
                if (elem.type == RichType.TEXT)
                {
                    LRichElementText elemText = elem as LRichElementText;
                    char[] _charArr = elemText.txt.ToCharArray();
                    TextGenerator gen = new TextGenerator();

                    foreach (char strChar in _charArr)
                    {
                        LRenderElement rendElem = new LRenderElement();
                        rendElem.type = RichType.TEXT;
                        rendElem.strChar = strChar.ToString();
                        rendElem.isOutLine = elemText.isOutLine;
                        rendElem.isUnderLine = elemText.isUnderLine;
                        rendElem.outLineColor = elemText.outLineColor;
                        rendElem.font = this.font;
                        rendElem.fontSize = elemText.fontSize;
                        rendElem.data = elemText.data;
                        rendElem.color = elemText.color;
                        rendElem.scale = 1;

                        TextGenerationSettings setting = new TextGenerationSettings();
                        setting.font = this.font;
                        setting.fontSize = elemText.fontSize;
                        setting.lineSpacing = 0;
                        setting.scaleFactor = 1;
                        setting.verticalOverflow = VerticalWrapMode.Overflow;
                        setting.horizontalOverflow = HorizontalWrapMode.Overflow;

                        rendElem.width = (int)gen.GetPreferredWidth(rendElem.strChar, setting);
                        rendElem.height = (int)gen.GetPreferredHeight(rendElem.strChar, setting);
                        _elemRenderArr.Add(rendElem);
                    }
                }
                else if (elem.type == RichType.IMAGE)
                {
                    LRichElementImage elemImg = elem as LRichElementImage;
                    LRenderElement rendElem = new LRenderElement();
                    rendElem.type = RichType.IMAGE;
                    rendElem.path = elemImg.path;
                    rendElem.data = elemImg.data;
                    rendElem.scale = elemImg.scale;
                    ResourceManager.instance.LoadResource<Sprite>(rendElem.path, delegate(Sprite sprite)
                    {
                        rendElem.width = (int)sprite.rect.size.x;
                        rendElem.height = (int)sprite.rect.size.y;
                        _elemRenderArr.Add(rendElem);
                    });
                }
                else if (elem.type == RichType.ANIM)
                {
                    LRichElementAnim elemAnim = elem as LRichElementAnim;
                    LRenderElement rendElem = new LRenderElement();
                    rendElem.type = RichType.ANIM;
                    rendElem.path = elemAnim.path;
                    rendElem.data = elemAnim.data;
                    rendElem.fs = elemAnim.fs;
                    rendElem.scale = 1;
                    ResourceManager.instance.LoadResource<Sprite>(rendElem.path, delegate(Sprite sprite)
                    {
                        rendElem.width = (int)sprite.rect.size.x;
                        rendElem.height = (int)sprite.rect.size.y;
                        _elemRenderArr.Add(rendElem);
                    });
                }
                else if (elem.type == RichType.NEWLINE)
                {
                    LRenderElement rendElem = new LRenderElement();
                    rendElem.isNewLine = true;
                    rendElem.type = RichType.NEWLINE;
                    rendElem.scale = 1;
                    _elemRenderArr.Add(rendElem);
                }
            }
            _richElements.Clear();
            formatRenderers();
            adjustAlign();
        }

        protected void formatRenderers()
        {
            int oneLine = 0;
            int lines = 1;
            bool isReplaceInSpace = false;
            int len = _elemRenderArr.Count;

            for (int i = 0; i < len; i++)
            {
                isReplaceInSpace = false;
                LRenderElement elem = _elemRenderArr[i];
                if (elem.isNewLine) // new line
                {
                    oneLine = 0;
                    lines++;
                    elem.width = 10;
                    elem.height = 27;
                    elem.pos = new Vector2(oneLine, -lines * 27);
                }
                else //other elements
                {
                    if (oneLine + elem.width > maxLineWidth)
                    {
                        if (elem.type == RichType.TEXT)
                        {
                            if (isChinese(elem.strChar) || elem.strChar == " ")
                            {
                                oneLine = 0;
                                lines++;

                                elem.pos = new Vector2(oneLine, -lines * 27);
                                oneLine = elem.width;
                            }
                            else // en
                            {
                                int spaceIdx = 0;
                                int idx = i;
                                while (idx > 0)
                                {
                                    idx--;
                                    if (_elemRenderArr[idx].strChar == " " &&
                                        _elemRenderArr[idx].pos.y == _elemRenderArr[i - 1].pos.y) // just for the same line
                                    {
                                        spaceIdx = idx;
                                        break;
                                    }
                                }
                                // can't find space , force new line
                                if (spaceIdx == 0)
                                {
                                    oneLine = 0;
                                    lines++;
                                    elem.pos = new Vector2(oneLine, -lines * 27);
                                    oneLine = elem.width;
                                }
                                else
                                {
                                    oneLine = 0;
                                    lines++;
                                    isReplaceInSpace = true; //reset cuting words position

                                    for (int _i = spaceIdx + 1; _i <= i; ++_i)
                                    {
                                        LRenderElement _elem = _elemRenderArr[_i];
                                        _elem.pos = new Vector2(oneLine, -lines * 27);
                                        oneLine += _elem.width;

                                        _elemRenderArr[_i] = _elem;
                                    }
                                }
                            }
                        }
                        else if (elem.type == RichType.ANIM || elem.type == RichType.IMAGE)
                        {
                            lines++;
                            elem.pos = new Vector2(0, -lines * 27);
                            oneLine = (int)(elem.width * elem.scale);
                        }
                    }
                    else
                    {
                        elem.pos = new Vector2(oneLine, -lines * 27);
                        oneLine += (int)(elem.width * elem.scale);
                    }
                }
                if (isReplaceInSpace == false)
                {
                    _elemRenderArr[i] = elem;
                }
            }
            //sort all lines
            Dictionary<int, List<LRenderElement>> rendElemLineMap = new Dictionary<int, List<LRenderElement>>();
            List<int> lineKeyList = new List<int>();
            len = _elemRenderArr.Count;
            for (int i = 0; i < len; i++)
            {
                LRenderElement elem = _elemRenderArr[i];
                List<LRenderElement> lineList;

                if (!rendElemLineMap.ContainsKey((int)elem.pos.y))
                {
                    lineList = new List<LRenderElement>();
                    rendElemLineMap[(int)elem.pos.y] = lineList;
                }
                rendElemLineMap[(int)elem.pos.y].Add(elem);
            }
            // all lines in arr
            List<List<LRenderElement>> rendLineArrs = new List<List<LRenderElement>>();
            var e = rendElemLineMap.GetEnumerator();
            while (e.MoveNext())
            {
                lineKeyList.Add(-1 * e.Current.Key);
            }
            lineKeyList.Sort();
            len = lineKeyList.Count;

            for (int i = 0; i < len; i++)
            {
                int posY = -1 * lineKeyList[i];
                string lineString = "";
                LRenderElement _lastEleme = rendElemLineMap[posY][0];
                LRenderElement _lastDiffStartEleme = rendElemLineMap[posY][0];
                if (rendElemLineMap[posY].Count > 0)
                {
                    List<LRenderElement> lineElemArr = new List<LRenderElement>();

                    int _len2 = rendElemLineMap[posY].Count;
                    for (int _i = 0; _i < _len2; _i++)
                    {
                        LRenderElement elem = rendElemLineMap[posY][_i];
                        if (_lastEleme.type == RichType.TEXT && elem.type == RichType.TEXT)
                        {
							if (_lastEleme.isSameStyle(elem))
                            {
								// the same style can mergin one element
                                lineString += elem.strChar;
                            }
                            else // diff style
                            {
                                if (_lastDiffStartEleme.type == RichType.TEXT)
                                {
                                    LRenderElement _newElem = _lastDiffStartEleme.Clone();
                                    _newElem.strChar = lineString;
                                    lineElemArr.Add(_newElem);

                                    _lastDiffStartEleme = elem;
                                    lineString = elem.strChar;
                                }
                            }
                        }
                        else if (elem.type == RichType.IMAGE || elem.type == RichType.ANIM || elem.type == RichType.NEWLINE)
                        {
                            //interrupt
                            if (_lastDiffStartEleme.type == RichType.TEXT)
                            {
                                LRenderElement _newEleme = _lastDiffStartEleme.Clone();
                                _newEleme.strChar = lineString;
                                lineString = "";
                                lineElemArr.Add(_newEleme);
                            }
                            lineElemArr.Add(elem);
                        }
                        else if (_lastEleme.type != RichType.TEXT)
                        {
                            //interrupt
                            _lastDiffStartEleme = elem;
                            if (elem.type == RichType.TEXT)
                            {
                                lineString = elem.strChar;
                            }
                        }
                        _lastEleme = elem;
                    }
                    // the last elementText
                    if (_lastDiffStartEleme.type == RichType.TEXT)
                    {
                        LRenderElement _newElem = _lastDiffStartEleme.Clone();
                        _newElem.strChar = lineString;
                        lineElemArr.Add(_newElem);
                    }
                    rendLineArrs.Add(lineElemArr);
                }
            }

            // offset position
            int _offsetLineY = 0;
            realLineHeight = 0;
            len = rendLineArrs.Count;
            for (int i = 0; i < len; i++)
            {
                List<LRenderElement> _lines = rendLineArrs[i];
                int _lineHeight = 0;
                int _len3 = _lines.Count;
                for (int _i=0;_i< _len3; _i++)
                {
                    _lineHeight = Mathf.Max(this.verticalSpace,Mathf.Max(_lineHeight, _lines[_i].height));
                }

                realLineHeight += _lineHeight;
                _offsetLineY += (_lineHeight - 27);

                for (int j = 0; j < _len3; j++)
                {
                    LRenderElement elem = _lines[j];
                    elem.pos = new Vector2(elem.pos.x, elem.pos.y - _offsetLineY);
                    realLineHeight = Mathf.Max(realLineHeight, (int)Mathf.Abs(elem.pos.y));
                    _lines[j] = elem;
                }
                rendLineArrs[i] = _lines;
            }

            // place all position
            realLineWidth = 0;
            GameObject obj = null;
            int _len = rendLineArrs.Count;
            for (int i = 0; i < _len; i++)
            {
                int _lineWidth = 0;
                int _leng = rendLineArrs[i].Count;
                for (int j = 0; j < _leng; j++)
                {
                    LRenderElement elem = rendLineArrs[i][j];
                    if (elem.type != RichType.NEWLINE)
                    {
                        int offset = 0;
                        if (elem.type == RichType.TEXT)
                        {
                            obj = getCacheLabel();
                            makeLabel(obj, elem);
                            _lineWidth += (int)obj.GetComponent<Text>().preferredWidth;
                        }
                        else if (elem.type == RichType.IMAGE)
                        {
                            obj = getCacheImage(true);
                            makeImage(obj, elem);
                            _lineWidth += (int)(obj.GetComponent<Image>().preferredWidth * elem.scale);
                            //offset = (int)(40 * 0.3f);
                            offset = 0;
                        }
                        else if (elem.type == RichType.ANIM)
                        {
                            obj = getCacheFramAnim();
                            makeFramAnim(obj, elem);
                            _lineWidth += elem.width;
                            offset = (int)(40 * 0.3f);
                        }
                        obj.SetActive(true);
                        obj.transform.SetParent(transform);
                        obj.transform.localPosition = new Vector2(elem.pos.x, elem.pos.y + offset);
                        obj.transform.localScale = new Vector3(elem.scale, elem.scale, 1);
                        _objectDataMap[obj] = elem.data;
                    }
                }
                realLineWidth = Mathf.Max(_lineWidth, realLineWidth);
            }
        }
        /// <summary>
        /// 对齐方式
        /// </summary>
        void adjustAlign()
        {
            RectTransform rtran = this.GetComponent<RectTransform>();
            float half_size_width = rtran.sizeDelta.x * 0.5f;
            float half_size_height = rtran.sizeDelta.y * 0.5f;
            Vector2 offset_pos = Vector2.zero;
            switch (alignment)
            {
                case TextAnchor.UpperLeft:
                    offset_pos.x = -half_size_width;
                    offset_pos.y = half_size_height;
                    break;
                case TextAnchor.MiddleLeft:
                    offset_pos.x = -half_size_width;
                    offset_pos.y = realLineHeight * 0.5f;
                    break;
                case TextAnchor.LowerLeft:
                    offset_pos.x = -half_size_width;
                    offset_pos.y = -(half_size_height - realLineHeight);
                    break;

                case TextAnchor.UpperCenter:
                    offset_pos.x = -realLineWidth * 0.5f;
                    offset_pos.y = half_size_height;
                    break;
                case TextAnchor.MiddleCenter:
                    offset_pos.x = -realLineWidth * 0.5f;
                    offset_pos.y = realLineHeight * 0.5f;
                    break;
                case TextAnchor.LowerCenter:
                    offset_pos.x = -realLineWidth * 0.5f;
                    offset_pos.y = -(half_size_height - realLineHeight);
                    break;

                case TextAnchor.UpperRight:
                    offset_pos.x = (half_size_width - realLineWidth);
                    offset_pos.y = half_size_height;
                    break;
                case TextAnchor.MiddleRight:
                    offset_pos.x = (half_size_width - realLineWidth);
                    offset_pos.y = realLineHeight * 0.5f;
                    break;
                case TextAnchor.LowerRight:
                    offset_pos.x = (half_size_width - realLineWidth);
                    offset_pos.y = -(half_size_height - realLineHeight);
                    break;
            }
            foreach (var elem in _objectDataMap)
            {
                RectTransform _rt = elem.Key.transform as RectTransform;
                _rt.anchoredPosition = _rt.anchoredPosition + offset_pos;
            }
        }

        void makeLabel(GameObject lab, LRenderElement elem)
        {
            Text comText = lab.GetComponent<Text>();
            if (comText != null)
            {
                comText.text = elem.strChar;
                comText.font = elem.font;
                comText.fontSize = elem.fontSize;
                comText.fontStyle = FontStyle.Normal;
                comText.color = elem.color;
                comText.lineSpacing = 0;
                comText.raycastTarget = !string.IsNullOrEmpty(elem.data) ? true : this.raycastTarget;
            }

            Outline outline = lab.GetComponent<Outline>();
            if (elem.isOutLine)
            {
                if (outline == null)
                {
                    outline = lab.AddComponent<Outline>();
                    outline.effectColor = elem.outLineColor;
                }
            }
            else {
                if (outline)
                {
                    Destroy(outline);
                }
            }

            if (elem.isUnderLine)
            {
                GameObject underLine = getCacheImage(false);
                Image underImg = underLine.GetComponent<Image>();
                underImg.color = elem.color;
                underImg.GetComponent<RectTransform>().sizeDelta = new Vector2(comText.preferredWidth, 3);
                underLine.SetActive(true);
                underLine.transform.SetParent(transform);
                underLine.transform.localScale = new Vector3(1, 1,1);
                underLine.transform.localPosition = new Vector2(elem.pos.x, elem.pos.y);
                comText.raycastTarget = true;
            }
        }

        void makeImage(GameObject img, LRenderElement elem)
        {
            Image comImage = img.GetComponent<Image>();
            if (comImage != null)
            {
                ResourceManager.instance.LoadResource<Sprite>(elem.path, delegate(Sprite sprite)
                {
                    comImage.sprite = sprite;
                    comImage.raycastTarget = this.raycastTarget;
                    comImage.transform.localScale = Vector3.one * 0.3f;
                });
            }
        }

        void makeFramAnim(GameObject anim, LRenderElement elem)
        {
            LMovieClip comFram = anim.GetComponent<LMovieClip>();
            if (comFram != null)
            {
                comFram.path = System.IO.Path.GetDirectoryName(elem.path);
                comFram.fps = elem.fs;
                comFram.play();
                comFram.GetComponent<Image>().raycastTarget = this.raycastTarget;
            }
        }

        protected GameObject getCacheLabel()
        {
            GameObject ret = null;
            int len = _cacheLabElements.Count;
            for (int i = 0; i < len; i++)
            {
                LRichCacheElement cacheElem = _cacheLabElements[i];
                if (cacheElem.isUse == false)
                {
                    cacheElem.isUse = true;
                    ret = cacheElem.node;
                    break;
                }
            }
            if (ret == null)
            {
                ret = new GameObject();
                ret.AddComponent<Text>();
                ContentSizeFitter fit = ret.AddComponent<ContentSizeFitter>();
                fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                fit.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                RectTransform rtran = ret.GetComponent<RectTransform>();
                rtran.pivot = Vector2.zero;
                rtran.anchorMax = new Vector2(0, 1);
                rtran.anchorMin = new Vector2(0, 1);

                LRichCacheElement cacheElem = new LRichCacheElement(ret);
                cacheElem.isUse = true;
                _cacheLabElements.Add(cacheElem);
            }
            return ret;
        }

        protected GameObject getCacheImage(bool isFitSize)
        {
            GameObject ret = null;
            int len = _cacheImgElements.Count;
            for (int i = 0; i < len; i++)
            {
                LRichCacheElement cacheElem = _cacheImgElements[i];
                if (cacheElem.isUse == false)
                {
                    cacheElem.isUse = true;
                    ret = cacheElem.node;
                    break;
                }
            }
            if (ret == null)
            {
                ret = new GameObject();
                ret.AddComponent<Image>();
                ContentSizeFitter fit = ret.AddComponent<ContentSizeFitter>();
                fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                fit.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                RectTransform rtran = ret.GetComponent<RectTransform>();
                rtran.pivot = Vector2.zero;
                rtran.anchorMax = new Vector2(0, 1);
                rtran.anchorMin = new Vector2(0, 1);

                LRichCacheElement cacheElem = new LRichCacheElement(ret);
                cacheElem.isUse = true;
                _cacheImgElements.Add(cacheElem);
            }
            ContentSizeFitter fitCom = ret.GetComponent<ContentSizeFitter>();
            fitCom.enabled = isFitSize;
            return ret;
        }

        protected GameObject getCacheFramAnim()
        {
            GameObject ret = null;
            int len = _cacheFramAnimElements.Count;
            for (int i = 0; i < len; i++)
            {
                LRichCacheElement cacheElem = _cacheFramAnimElements[i];
                if (cacheElem.isUse == false)
                {
                    cacheElem.isUse = true;
                    ret = cacheElem.node;
                    break;
                }
            }
            if (ret == null)
            {
                ret = new GameObject();
                ret.AddComponent<Image>();
                ContentSizeFitter fit = ret.AddComponent<ContentSizeFitter>();
                fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                fit.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                RectTransform rtran = ret.GetComponent<RectTransform>();
                rtran.pivot = Vector2.zero;
                rtran.anchorMax = new Vector2(0, 1);
                rtran.anchorMin = new Vector2(0, 1);

                ret.AddComponent<LMovieClip>();

                LRichCacheElement cacheElem = new LRichCacheElement(ret);
                cacheElem.isUse = true;
                _cacheFramAnimElements.Add(cacheElem);
            }
            return ret;
        }

        protected bool isChinese(string text)
        {
            bool hasChinese = false;
            for(int i=0;i<text.Length;i++)
            {
                if((int)text[i] > 127)
                    hasChinese = true;
            }
            return hasChinese;
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (_objectDataMap.ContainsKey(data.pointerEnter))
            {
                if ((onClickHandler != null) && (_objectDataMap[data.pointerEnter] != ""))
                {
                    onClickHandler.Invoke(_objectDataMap[data.pointerEnter]);
                }
            }
        }
        //---------------------parse rich element content from string----------------------------------------
        private string[] SaftSplite(string content, char separater)
        {
            List<string> arr = new List<string>();
            char[] charArr = content.ToCharArray();
            bool strFlag = false;
            List<char> line = new List<char>();
            for (int i =0;i< charArr.Length; i++)
            {
                if((charArr[i] == '"') && (charArr[i-1] != '\\')) //string start
                {
                    strFlag = !strFlag;
                }
                if(charArr[i] == separater)
                {
                    if (!strFlag)
                    {
                        arr.Add(new string(line.ToArray()));
                        line.Clear();
                    }
                    else
                    {
                        line.Add(charArr[i]);
                    }
                }
                else
                {
                    line.Add(charArr[i]);
                }
            }
            if(line.Count > 0)
            {
                arr.Add(new string(line.ToArray()));
            }
            return arr.ToArray();
        }

        private List<string> executeParseRichElem(List<string> result, string content, int startIndex)
        {
            bool hasMatch = false;
            int matchIndex = content.IndexOf("<", startIndex);
            if (matchIndex != -1)
            {
                result.Add(string.Format("lab txt=\"{0}\"", content.Substring(startIndex, matchIndex - startIndex))); //match head
                startIndex = matchIndex;

                matchIndex = content.IndexOf("/>", startIndex);
                if (matchIndex != -1)
                {
                    hasMatch = true;
                    result.Add(content.Substring(startIndex + 1, matchIndex - (startIndex + 1))); //match tail
                    startIndex = matchIndex + 2;
                }
            }

            if (hasMatch)
            {
                return executeParseRichElem(result, content, startIndex);
            }
            else
            {
                result.Add(string.Format("lab txt=\"{0}\"", content.Substring(startIndex, content.Length - startIndex)));
                return result;
            }
        }

        private void parseRichElemString(string content, UnityAction<string, Dictionary<string, string>> handleFunc)
        {
            List<string> elemStrs = new List<string>();

            int startIndex = 0;
            elemStrs = executeParseRichElem(elemStrs, content, startIndex);

            int len = elemStrs.Count;
            for (int i = 0; i < len; i++)
            {
                string flag = elemStrs[i].Substring(0, elemStrs[i].IndexOf(" "));
                string paramStr = elemStrs[i].Substring(elemStrs[i].IndexOf(" ") + 1);
                string[] paramArr = SaftSplite(paramStr,' ');

                Dictionary<string, string> param = new Dictionary<string, string>();
                int paramArrLen = paramArr.Length;
                for (int j = 0; j < paramArrLen; j++)
                {
                    string[] paramObj = SaftSplite(paramArr[j], '=');
                    string left = paramObj[0].Trim();
                    string right = paramObj[1].Trim();

                    if (right.EndsWith("\"") && right.StartsWith("\""))
                    {
                        param.Add(left, right.Trim('"'));
                    }
                    else
                    {
                        param.Add(left, right);
                    }
                }
                handleFunc.Invoke(flag, param);
            }
        }

        public void parseRichDefaultString(string content, UnityAction<string, Dictionary<string, string>> specHandleFunc=null)
        {
            parseRichElemString(content, (flag, param) =>
            {
                if (flag == "lab")
                {
                    this.insertElement(
                        param.ContainsKey("txt") ? param["txt"] : "",
                        ColorUtils.HexToColor(param.ContainsKey("color") ? param["color"] : defaultLabColor),
                        param.ContainsKey("size") ? System.Convert.ToInt32(param["size"]) : defaultLabSize,
                        param.ContainsKey("isUnderLine") ? System.Convert.ToBoolean(param["isUnderLine"]) : false,
                        param.ContainsKey("isOutLine") ? System.Convert.ToBoolean(param["isOutLine"]) : false,
                        ColorUtils.HexToColor(param.ContainsKey("outLineColor") ? param["outLineColor"] : "#000000"),
                        param.ContainsKey("data") ? param["data"] : ""
                        );
                }else if(flag == "img")
                {
                    this.insertElement(
                         param.ContainsKey("path") ? param["path"] : "",
                         param.ContainsKey("data") ? param["data"] : "",
                         param.ContainsKey("scale") ? System.Convert.ToSingle(param["scale"]) : 1f);
                }
                else if(flag == "anim")
                {
                    this.insertElement(param["path"],
                         param.ContainsKey("fps") ? System.Convert.ToSingle(param["fps"]) : 15f,
                         param.ContainsKey("data") ? param["data"] : "");
                }else if(flag == "newline")
                {
                    this.insertElement(1);
                }
                else
                {
                    if(specHandleFunc != null) specHandleFunc.Invoke(flag, param);
                }
            });
            this.reloadData();
        }

        private void removeAllElements()
        {
            int len = _cacheLabElements.Count;
            for (int i = 0; i < len; i++)
            {
                _cacheLabElements[i].isUse = false;
                _cacheLabElements[i].node.gameObject.SetActive(false);
            }
            len = _cacheImgElements.Count;
            for (int i = 0; i < len; i++)
            {
                _cacheImgElements[i].isUse = false;
                _cacheImgElements[i].node.gameObject.SetActive(false);
            }
            len = _cacheFramAnimElements.Count;
            for (int i = 0; i < len; i++)
            {
                _cacheFramAnimElements[i].isUse = false;
                _cacheFramAnimElements[i].node.gameObject.SetActive(false);
            }
            _elemRenderArr.Clear();
            _objectDataMap.Clear();
        }

        private void insertElement(string txt, Color color, int fontSize, bool isUnderLine, bool isOutLine, Color outLineColor, string data)
        {
            _richElements.Add(new LRichElementText(color, txt, fontSize, isUnderLine, isOutLine, outLineColor, data));
        }

        private void insertElement(string path, float fp, string data)
        {
            _richElements.Add(new LRichElementAnim(path, fp, data));
        }

        private void insertElement(string path, string data, float scale)
        {
            _richElements.Add(new LRichElementImage(path, data, scale));
        }

        private void insertElement(int newline)
        {
            _richElements.Add(new LRichElementNewline());
        }
    }
}