using EditorUIMaker.Utility;
using Scriban;
using Scriban.Runtime;
using UnityEngine;

namespace EditorUIMaker.Widgets
{
    public class EUM_Space : EUM_Widget
    {
        public override GUIIconLib.E_Icon IconType => GUIIconLib.E_Icon.Space;
        EUM_Space_Info info => Info as EUM_Space_Info;
        public override string TypeName => "Space";

        protected override EUM_BaseInfo CreateInfo()
        {
            var info = new EUM_Space_Info(this);
            info.Height = 10;
            return info;
        }

        public override void DrawDragging(Vector2 position)
        {
            var rect = new Rect(position.x, position.y, 200, 20);
            GUILib.Area(rect, () => { GUILib.Label(TypeName); });
        }

        protected override void OnDrawLayout()
        {
            GUILib.Space(info.Height);
        }

        public override EUM_BaseWidget Clone()
        {
            var widget = new EUM_Space();
            Info.CopyTo(widget.Info);
            return widget;
        }

        public override string LogicCode()
        {
            return string.Empty;
        }

        public override string Code()
        {
            var code =
                @"GUILib.Space({{pixel}});";

            var sObj = new ScriptObject();
            sObj.Add("pixel", info.Height);

            var context = new TemplateContext();
            context.PushGlobal(sObj);

            var template = Template.Parse(code);
            var result = template.Render(context);

            return result;
        }
    }
}