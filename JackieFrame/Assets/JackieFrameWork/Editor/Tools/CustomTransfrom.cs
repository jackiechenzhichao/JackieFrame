//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2020-08-28 17:29:56
//=======================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Text;

namespace fs
{

    [CustomEditor(typeof(Transform))]
    public class CustomTransfrom : Editor
    {

        private Editor editor;
        private Transform transform;



        public override void OnInspectorGUI()
        {
             

            if (editor == null)
            {
                editor = CreateEditor(target, Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.TransformInspector", true));
                transform = target as Transform;
            }

   
            editor.OnInspectorGUI();


            Rect rect = GUILayoutUtility.GetRect(0, 16);

            if (GUI.Button(GetRect(rect, new Rect(0, -54, 0 - rect.width * 0.75f, 0)), "Position"))
            {
                transform.localPosition = Vector3.zero;
            }


            if (GUI.Button(GetRect(rect, new Rect(0, -36, 0 - rect.width * 0.75f, 0)), "Rotation"))
            {
                transform.localEulerAngles = Vector3.zero;
            }

            if (GUI.Button(GetRect(rect, new Rect(0, -18, 0 - rect.width * 0.75f, 0)), "Scale"))
            {
                transform.localScale = Vector3.one;
            }
        }


        Rect GetRect(Rect rect, Rect inRect)
        {
            return new Rect(rect.x + inRect.x, rect.y + inRect.y, rect.width + inRect.width, rect.height + inRect.height);
        }

    }
}
