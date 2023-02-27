using Amazing.Editor.Library;
using UnityEngine;

namespace EditorUIMaker.Widgets
{
    public abstract class EUM_Widget : EUM_BaseWidget
    {
        public override EUM_BaseWidget SingleClone()
        {
            return Clone();
        }

        public virtual string Code()
        {
            return string.Empty;
        }
    }
}