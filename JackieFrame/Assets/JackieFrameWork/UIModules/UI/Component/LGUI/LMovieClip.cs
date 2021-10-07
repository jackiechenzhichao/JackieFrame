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
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace fs
{
    /// <summary>
    /// 序列帧动画
    /// </summary>
    [RequireComponent(typeof(Image))]
	public class LMovieClip : MonoBehaviour
    {
        public float fps = 15f;                                     //每秒播放的帧数
		public bool isPlayOnwake = false;                           //是否刚开始就播放
		public string path;                                         //资源路径
        public string prefix=string.Empty;                          //前缀

        protected Image _comImage;                                  //图片
        protected SpriteRenderer _comSprender;                      //精灵
        protected float _time;                      
        protected int _frameLenght;                                 //帧数
        protected bool _isPlaying = false;                          //是否正在播放
		protected int _currentIndex = 0;                            //当前的索引值
        protected List<Sprite> _spriteArr = new List<Sprite>();     //存放精灵的列变

        void Start()
        {
            _comImage = gameObject.GetComponent<Image>();
            _comSprender = gameObject.GetComponent<SpriteRenderer>();
            loadTexture();
			if (isPlayOnwake) 
            {
				play ();
			}
        }

        /// <summary>
        /// 加载序列帧精灵
        /// </summary>
		void loadTexture()
		{
            Sprite[] sprites = LocalResLoader.instance.LoadAllSprite(path);
            Debug.Log(sprites.Length);
            if (sprites == null) return;
            for (int i = 0; i < sprites.Length; ++i)
            {
                if (string.IsNullOrEmpty(prefix) || sprites[i].name.IndexOf(prefix) >= 0)
                    _spriteArr.Add(sprites[i]);
            }
            _currentIndex = 0;
            _frameLenght = _spriteArr.Count;
        }

        void Update()
        {
            if (_isPlaying)
            {
                drawAnimation();
            }
        }

        protected void drawAnimation()
        {
            if (_spriteArr == null) return;
            if(_comImage)
                _comImage.sprite = _spriteArr[_currentIndex];
            else if(_comSprender)
                _comSprender.sprite = _spriteArr[_currentIndex];

            if (_currentIndex < _frameLenght)
            {
                _time += Time.deltaTime;
                if (_time >= 1.0f / fps)
                {
					_currentIndex++;
                    _time = 0;
                    if (_currentIndex == _frameLenght)
                    {
                        _currentIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        public void play()
        {
            _isPlaying = true;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void stop()
        {
            _isPlaying = false;
            _currentIndex = 0;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void pause()
        {
            _isPlaying = false;
        }
    }
}
