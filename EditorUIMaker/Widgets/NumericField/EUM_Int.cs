using EditorUIMaker.Utility;
using Scriban;
using Scriban.Runtime;
using UnityEditor;
using UnityEngine;

namespace EditorUIMaker.Widgets
{
    public class EUM_Int : EUM_Widget
    {
        public override GUIIconLib.E_Icon IconType => GUIIconLib.E_Icon.Int;
        private EUM_Int_Info info => Info as EUM_Int_Info;
        public override string TypeName => "Int";

        protected override EUM_BaseInfo CreateInfo()
        {
            var info = new EUM_Int_Info(this);
            info.Label = TypeName;
            return info;
        }

        protected override void OnDrawLayout()
        {
            GUILib.IntField(ref info.Value, info.Label,LayoutOptions());
        }

        public override string LogicCode()
        {
            var code = @"public int {{name}};

public void {{name}}ValueChange()
{
    CallMethod(""On{{name}}ValueChange"");
}
";
            var sObj = new ScriptObject();
            sObj.Add("name", Info.Name);

            var context = new TemplateContext();
            context.PushGlobal(sObj);

            var template = Template.Parse(code);
            var result = template.Render(context);

            return result;
        }

        public override string Code()
        {
            var code = @"
if(GUILib.IntField(ref _Logic.{{name}},""{{label}}"",{{layout}}))
{
    _Logic.{{name}}ValueChange();
}
";

            var sObj = new ScriptObject();
            sObj.Add("name", Info.Name);
            sObj.Add("label", info.Label);
            var layoutString = LayoutOptionsStr();
            sObj.Add("layout",layoutString);

            var context = new TemplateContext();
            context.PushGlobal(sObj);

            var template = Template.Parse(code);
            var result = template.Render(context);

            return result;
        }

        public override void DrawDragging(Vector2 position)
        {
            GUILib.Area(new Rect(position.x, position.y, 200, 20), () =>
            {
                var val = 1;
                GUILib.IntField(ref val, TypeName);
            });
        }

        public override EUM_BaseWidget Clone()
        {
            var widget = new EUM_Int();
            Info.CopyTo(widget.Info);
            return widget;
        }
    }
}