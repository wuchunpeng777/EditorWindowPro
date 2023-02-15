using Amazing.Editor.Library;
using EditorUIMaker.Widgets;
using UnityEngine;

namespace EditorUIMaker
{
    public class EUM_Horizontal : EUM_Container
    {
        public override string TypeName => "Horizontal";
        
        protected override void OnDrawLayout()
        {
            GUILayout.BeginHorizontal();
            DrawItems();
            GUILayout.EndHorizontal();
        }

        protected override void DrawItems()
        {
            if (!EUM_Helper.Instance.Preview && InViewport)
            {
                GUI.enabled = false;
                GUI.color = new Color(1, 1, 1, 2);
            }
            
            foreach (var widget in Widgets)
            {
                widget.DrawLayout();
            }
            
            if (!EUM_Helper.Instance.Preview && InViewport)
            {
                GUI.color = Color.white;
                GUI.enabled = true;
            }
        }

        public override void DrawDraging(Vector2 position)
        {
            var rect = new Rect(position.x, position.y, 200, 20);
            GUILib.Frame(rect, Color.white, 1);
        }

        public override EUM_BaseWidget Clone()
        {
            var widget = new EUM_Horizontal();
            return widget;
        }
    }
}