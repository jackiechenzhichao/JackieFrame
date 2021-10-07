using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace fs
{
    /// <summary>
    /// ui遮罩，实现两个功能
    /// 1.屏蔽下层点击，一般是弹出对话框时，防止下层界面被点中
    /// 2.点击空白区域，自动关闭窗口(这种情况需要窗口本身有点击事件)
    /// @author hannibal
    /// @time 2016-2-5
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UIMaskScript : MonoBehaviour
    {
        public bool m_IsAutoClose = false;
        public string m_ParamInfo = "";

        void Awake()
        {
            Image img = this.GetComponent<Image>();
            img.raycastTarget = true;
            this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 2, Screen.height * 2);
            UIHelper.AddEventListener(gameObject, eUIEventType.Click, OnPointerClick);
        }

        void Destroy()
        {
            UIHelper.RemoveEventListener(gameObject, eUIEventType.Click, OnPointerClick);
        }

        public void OnPointerClick(UIEventArgs evt)
        {
            if (m_IsAutoClose)
            {
                UIPanel ui_view = this.gameObject.GetComponent<UIPanel>();
                if (ui_view == null) ui_view = this.gameObject.GetComponentInParent<UIPanel>();
                if (ui_view != null)
                {//如果是ui面板，由UIManager统一负责管理
                    EventDispatcher.TriggerEvent(UIEventID.REQUEST_CLOSE_GUI, ui_view.uid);
                }
                else
                {
                    GameObject.Destroy(this.gameObject);
                }
            }
        }
    }
}