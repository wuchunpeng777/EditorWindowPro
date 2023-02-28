using Amazing.Editor.Library;
using UnityEditor;
using UnityEngine;

namespace EditorUIMaker
{
    public class EUM_Toolbar : I_EUM_Drawable
    {
        public const float s_Height = 20;

        public EUM_Toolbar()
        {
            EUM_Helper.Instance.ZoomIndex = EUM_Helper.DefaultZoomIndex();
        }

        public void Draw(ref Rect rect)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.UpperCenter;
            
            var drawRect = new Rect(rect.x, rect.y, rect.width, s_Height);
            rect.yMin += s_Height;

            GUILayout.BeginArea(drawRect);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("File", "ToolbarPopup", GUILayout.Width(40)))
            {
                var optionsMenu = new GenericMenu();
                optionsMenu.AddItem(new GUIContent("Open"), false, EUM_Helper.Instance.OpenFile);
                optionsMenu.AddItem(new GUIContent("New"), false, EUM_Helper.Instance.NewFile);
                var buttonRect = GUILayoutUtility.GetLastRect();
                var dropdownRect = new Rect(buttonRect);
                dropdownRect.y += s_Height;
                optionsMenu.DropDown(dropdownRect);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Zoom:", style,GUILayout.Width(40));
            EUM_Helper.Instance.ZoomIndex = EditorGUILayout.Popup(EUM_Helper.Instance.ZoomIndex,
                EUM_Helper.GetZoomScalesText(), GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Fit Canvas", "ToolbarButton"))
            {
            }

            if (GUILayout.Button("Save", "ToolbarButton"))
            {
                EUM_Helper.Instance.SaveFile();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MenuPath:", style,GUILayout.Width(70));
            EUM_Helper.Instance.MenuItemPath =
                EditorGUILayout.TextField(EUM_Helper.Instance.MenuItemPath ,GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILib.Toggle(ref EUM_Helper.Instance.Preview, new GUIContent("Preview"),
                    new GUIStyle("ToolbarButton")))
            {
                if (EUM_Helper.Instance.Preview)
                {
                    EUM_Helper.Instance.SelectWidget = null;
                    EUM_Helper.Instance.HoverWidget = null;
                    EUM_Helper.Instance.OnSelectWidgetChange?.Invoke();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}