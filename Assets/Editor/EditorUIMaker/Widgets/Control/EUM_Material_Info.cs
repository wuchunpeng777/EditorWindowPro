using System;
using UnityEngine;

namespace EditorUIMaker.Widgets
{
    public class EUM_Material_Info : EUM_BaseInfo
    {
        public string Label;

        [NonSerialized] public Material Value;
        
        public EUM_Material_Info(EUM_BaseWidget widget) : base(widget)
        {
        }

        public override void CopyTo<T>(T target)
        {
            var info = target as EUM_Material_Info;
            CopyBaseInfo(info);

            info.Label = Label;
        }
    }
}