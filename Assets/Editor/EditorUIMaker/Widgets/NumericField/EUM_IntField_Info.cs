using System;

namespace EditorUIMaker.Widgets
{
    public class EUM_IntField_Info : EUM_BaseInfo
    {
        public string Label;

        [NonSerialized] public int Value;

        public EUM_IntField_Info(EUM_BaseWidget widget) : base(widget)
        {
        }

        public override void CopyTo<T>(T target)
        {
            var info = target as EUM_IntField_Info;
            CopyBaseInfo(info);

            info.Label = Label;
        }
    }
}