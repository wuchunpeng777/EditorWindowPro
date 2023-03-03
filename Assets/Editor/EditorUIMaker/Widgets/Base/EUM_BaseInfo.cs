
using UnityEngine;

namespace EditorUIMaker.Widgets
{
    public abstract class EUM_BaseInfo
    {
        public const float Min_Height = 2;
        
        public string Name;
        [SerializeField]
        private EUM_BaseWidget _Widget;
        public int ID => _Widget.ID;
        public float Height = 0;

        public float GetHeight()
        {
            return Mathf.Max(Height, Min_Height);
        } 
        
        public string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(Name) ? _Widget.TypeName : Name;
            }
        }
        
        public EUM_BaseInfo(EUM_BaseWidget widget)
        {
            _Widget = widget;
        }
        
        public abstract void CopyTo<T>(T target) where T : EUM_BaseInfo;
        
        protected void CopyBaseInfo(EUM_BaseInfo info)
        {
            info.Name = Name;
        }
    }
}