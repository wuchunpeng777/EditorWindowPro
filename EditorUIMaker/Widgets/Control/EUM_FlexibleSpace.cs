using EditorUIMaker.Utility;
using Scriban;
using Scriban.Runtime;
using UnityEngine;

namespace EditorUIMaker.Widgets
{
    public class EUM_FlexibleSpace : EUM_Widget
    {
        public override GUIIconLib.E_Icon IconType=> GUIIconLib.E_Icon.FlexibleSpace;
        EUM_NormalInfo info => Info as EUM_NormalInfo;
        public override string TypeName => "Flexible Space";
        protected override EUM_BaseInfo CreateInfo()
        {
            return new EUM_NormalInfo(this);
        }
        
        public override bool CanResize()
        {
            return false;
        }

        protected override void OnDrawLayout()
        {
            GUILib.FlexibleSpace();
        }

        public override string LogicCode()
        {
            return string.Empty;
        }

        public override string Code()
        {
            var code = "GUILayout.FlexibleSpace();";
            return code;
        }

        public override void DrawDragging(Vector2 position)
        {
            var rect = new Rect(position.x + 20, position.y, 200, 20);
            GUILib.Area(rect, () =>
            {
                GUILib.Label(TypeName);
            });
        }

        public override EUM_BaseWidget Clone()
        {
            var widget = new EUM_FlexibleSpace();
            Info.CopyTo(widget.Info);
            return widget;
        }
    }
}