using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;


namespace fs
{
    /// <summary>
    /// 开启和关闭的UI元素过度细节动画基类
    /// 提供给美术的基础UI元素动画效果脚本类
    /// 1.本质上是用到了Dotween功能，简化版。方便美术优化界面
    /// 2.todo：经讨论，退场动画功能暂时不需要实现
    /// 3.继承类：UIPanelMove、UIPanelScale、UIPanelFade
    /// @author Qc_Chao
    /// @time 2016-01-05
    /// </summary>
    public class UIPanelAnimation : MonoBehaviour
    {
        public float m_Duration;
        public float m_Delay;
        public Ease m_easeType = Ease.OutQuad;

        protected bool m_Running;

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void OnEnable()
        {
            m_Running = false;
        }

        public virtual void OnDisable()
        {
        }

        public virtual void Rollback()
        {

        }
        public bool Running
        {
            get { return m_Running; }
        }
    }
}