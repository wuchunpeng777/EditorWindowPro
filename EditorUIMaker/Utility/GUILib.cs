using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorUIMaker
{
    public static class GUILib
    {
        public static readonly Color s_DefaultColor = new Color(56f / 255, 56f / 255, 56f / 255, 1f);
        public static readonly float s_SplitSize = 2;
        public const float s_DefaultLineHeight = 1f;

        internal static Dictionary<string, GUIContent> tooltipCache = new Dictionary<string, GUIContent>();

        public static void ProgressBar(float value, string label, float height)
        {
            var rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.ProgressBar(rect, value, label);
        }

        public static Rect Frame(Rect rect, Color color, Rect clipRect, float size = 1, float alpha = 1)
        {
            var visibleRect = new Rect();

            var oldColor = GUI.color;
            color.a = alpha;

            GUI.color = color;


            //left
            if (rect.xMin < clipRect.xMin || rect.xMin > clipRect.xMax || rect.yMin > clipRect.yMax ||
                rect.yMax < clipRect.yMin)
            {
                //no left
                if (rect.xMin < clipRect.xMin)
                    visibleRect.xMin = clipRect.xMin;
                else if (rect.xMin > clipRect.xMax)
                    visibleRect.xMin = clipRect.xMax;
                else
                {
                    visibleRect.xMin = rect.xMin;
                }
            }
            else
            {
                visibleRect.xMin = rect.xMin;
                GUI.DrawTexture(new Rect(rect.xMin, Mathf.Max(rect.yMin, clipRect.yMin), size,
                        Mathf.Min(rect.yMax, clipRect.yMax) - Mathf.Max(rect.yMin, clipRect.yMin)),
                    (Texture) EditorGUIUtility.whiteTexture);
            }

            //right
            if (rect.xMax > clipRect.xMax || rect.xMax < clipRect.xMin || rect.yMin > clipRect.yMax ||
                rect.yMax < clipRect.yMin)
            {
                //no right
                if (rect.xMax > clipRect.xMax)
                    visibleRect.xMax = clipRect.xMax;
                else if (rect.xMax < clipRect.xMin)
                    visibleRect.xMax = clipRect.xMin;
                else
                {
                    visibleRect.xMax = rect.xMax;
                }
            }
            else
            {
                visibleRect.xMax = rect.xMax;
                GUI.DrawTexture(new Rect(rect.xMax, Mathf.Max(rect.yMin, clipRect.yMin), size,
                        Mathf.Min(rect.yMax, clipRect.yMax) - Mathf.Max(rect.yMin, clipRect.yMin)),
                    (Texture) EditorGUIUtility.whiteTexture);
            }

            //top
            if (rect.yMin < clipRect.yMin || rect.yMin > clipRect.yMax || rect.xMax < clipRect.xMin ||
                rect.xMin > clipRect.xMax)
            {
                //no top
                if (rect.yMin < clipRect.yMin)
                    visibleRect.yMin = clipRect.yMin;
                else if (rect.yMin > clipRect.yMax)
                    visibleRect.yMin = clipRect.yMax;
                else
                {
                    visibleRect.yMin = rect.yMin;
                }
            }
            else
            {
                visibleRect.yMin = rect.yMin;
                GUI.DrawTexture(new Rect(Mathf.Max(rect.xMin, clipRect.xMin), rect.yMin,
                        Mathf.Min(rect.xMax, clipRect.xMax) - Mathf.Max(rect.xMin, clipRect.xMin), size),
                    (Texture) EditorGUIUtility.whiteTexture);
            }

            //bottom
            if (rect.yMax > clipRect.yMax || rect.yMax < clipRect.yMin || rect.xMax < clipRect.xMin ||
                rect.xMin > clipRect.xMax)
            {
                //no bottom
                if (rect.yMax > clipRect.yMax)
                    visibleRect.yMax = clipRect.yMax;
                else if (rect.yMax < clipRect.yMin)
                    visibleRect.yMax = clipRect.yMin;
                else
                {
                    visibleRect.yMax = rect.yMax;
                }
            }
            else
            {
                visibleRect.yMax = rect.yMax;
                GUI.DrawTexture(new Rect(Mathf.Max(rect.xMin, clipRect.xMin), rect.yMax,
                        Mathf.Min(rect.xMax, clipRect.xMax) - Mathf.Max(rect.xMin, clipRect.xMin), size),
                    (Texture) EditorGUIUtility.whiteTexture);
            }

            GUI.color = oldColor;

            return visibleRect;
        }

        public static void Frame(Rect rect, Color color, float size = 1, float alpha = 1)
        {
            var oldColor = GUI.color;
            color.a = alpha;

            GUI.color = color;

            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, size), (Texture) EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - size, rect.width, size),
                (Texture) EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y + 1f, size, rect.height - 2f * size),
                (Texture) EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - size, rect.y + 1f, size, rect.height - 2f * size),
                (Texture) EditorGUIUtility.whiteTexture);

            GUI.color = oldColor;
        }

        public static void Rect(Rect r, Color c, float alpha = 1)
        {
            var cColor = GUI.color;
            c.a = alpha;

            GUI.color = c;
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = cColor;
        }

        internal static GUIContent GetTooltip(string tooltip)
        {
            if (string.IsNullOrEmpty(tooltip)) return GUIContent.none;

            GUIContent result;
            if (tooltipCache.TryGetValue(tooltip, out result)) return result;
            result = new GUIContent(string.Empty, tooltip);
            tooltipCache.Add(tooltip, result);
            return result;
        }

        public static Rect Padding(Rect r, float x, float y)
        {
            return new Rect(r.x + x, r.y + y, r.width - 2 * x, r.height - 2 * y);
        }

        internal static bool ToolbarToggle(ref bool value, Texture icon, Vector2 padding, string tooltip = null)
        {
            var vv = GUILayout.Toggle(value, GetTooltip(tooltip), EditorStyles.toolbarButton, GUILayout.Width(22f));

            if (icon != null)
            {
                var rect = GUILayoutUtility.GetLastRect();
                rect = Padding(rect, padding.x, padding.y);
                GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
            }

            if (vv == value) return false;
            value = vv;
            return true;
        }

        public static bool Toggle(ref bool value, GUIContent tex, GUIStyle style = null,
            params GUILayoutOption[] options)
        {
            bool vv = false;
            if (style == null)
                vv = GUILayout.Toggle(value, tex, options);
            else
                vv = GUILayout.Toggle(value, tex, style, options);
            if (vv == value) return false;
            value = vv;
            return true;
        }

        public static bool Toggle(ref bool value, string content, GUIStyle style = null,
            params GUILayoutOption[] options)
        {
            bool vv = false;
            if (style == null)
                vv = GUILayout.Toggle(value, content, options);
            else
                vv = GUILayout.Toggle(value, content, style, options);
            if (vv == value) return false;
            value = vv;
            return true;
        }

        public static void HelpBox(string text, MessageType type)
        {
            EditorGUILayout.HelpBox(text, type);
        }

        public static void Line(float margin = 0)
        {
            GUILayout.Space(margin);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, s_DefaultLineHeight), s_DefaultColor);
            GUILayout.Space(margin);
        }

        public static void Icon(GUIContent icon, float width, float height)
        {
            GUILayout.Label(icon, GUILayout.Width(width), GUILayout.Height(height));
        }

        public static void Area(Rect rect, Action drawContent)
        {
            GUILayout.BeginArea(rect);
            drawContent?.Invoke();
            GUILayout.EndArea();
        }

        public static void HorizontalRect(Action drawContent, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style != null)
                GUILayout.BeginHorizontal(style, options);
            else
                GUILayout.BeginHorizontal(options);
            drawContent?.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void VerticelRect(Action drawContent, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style != null)
                GUILayout.BeginVertical(style, options);
            else
                GUILayout.BeginVertical(options);
            drawContent?.Invoke();
            GUILayout.EndVertical();
        }

        public static bool Popup(ref int val, string[] contents, params GUILayoutOption[] options)
        {
            var tmpSelect = EditorGUILayout.Popup(val, contents, EditorStyles.toolbarPopup, options);
            if (Equals(tmpSelect, val))
            {
                return false;
            }

            val = tmpSelect;
            return true;
        }

        public static bool Popup(ref string val, string[] contents,
            params GUILayoutOption[] options)
        {
            var select = Array.IndexOf(contents, val);
            if (select == -1) select = 0;
            var vv = EditorGUILayout.Popup(select, contents, EditorStyles.toolbarPopup, options);
            if (Equals(vv, select))
            {
                return false;
            }

            val = contents[vv];
            return true;
        }

        public static bool PopupWithIcon(ref string val, string[] contents, GUIContent icon,
            params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            var cRect = GUILayoutUtility.GetRect(16f, 16f);
            cRect.xMin -= 2f;
            cRect.yMin += 2f;
            GUI.Label(cRect, icon);

            var select = Array.IndexOf(contents, val);
            if (select == -1) select = 0;
            var vv = EditorGUILayout.Popup(select, contents, EditorStyles.toolbarPopup, options);
            if (Equals(vv, select))
            {
                GUILayout.EndHorizontal();
                return false;
            }

            val = contents[vv];
            GUILayout.EndHorizontal();
            return true;
        }

        public static bool EnumPopup<T>(ref T mode, GUIContent icon, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            var obj = (Enum) (object) mode;
            var cRect = GUILayoutUtility.GetRect(16f, 16f);
            cRect.xMin -= 2f;
            cRect.yMin += 2f;
            GUI.Label(cRect, icon);

            var vv = EditorGUILayout.EnumPopup(obj, EditorStyles.toolbarPopup, options);
            if (Equals(vv, obj))
            {
                GUILayout.EndHorizontal();
                return false;
            }

            mode = (T) (object) vv;
            GUILayout.EndHorizontal();
            return true;
        }

        public static bool IntSlider(string label, ref int val, int min, int max, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.IntSlider(label, val, min, max, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool Slider(string label, ref float val, float min, float max, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.Slider(label, val, min, max, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool MinMaxSlider(string label, ref float minVal, ref float maxVal, float min, float max,
            bool readOnly = false,
            params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            var tmpMin = minVal;
            var tmpMax = maxVal;
            if (readOnly)
            {
                GUILayout.Label(minVal.ToString(), GUILayout.Width(50));
                GUILayout.Label(maxVal.ToString(), GUILayout.Width(50));
            }
            else
            {
                var tmpMinStr = GUILayout.TextField(minVal.ToString(), GUILayout.Width(50));
                if (float.TryParse(tmpMinStr, out var tmpMinVal))
                {
                    tmpMin = tmpMinVal;
                }
                else
                {
                    tmpMin = min;
                }

                tmpMin = Mathf.Clamp(tmpMin, min, max);

                var tmpMaxStr = GUILayout.TextField(maxVal.ToString(), GUILayout.Width(50));
                if (float.TryParse(tmpMaxStr, out var tmpMaxVal))
                {
                    tmpMax = tmpMaxVal;
                }
                else
                {
                    tmpMax = max;
                }

                tmpMax = Mathf.Clamp(tmpMax, tmpMin, max);
            }

            EditorGUILayout.MinMaxSlider(ref tmpMin, ref tmpMax, min, max, options);

            GUILayout.EndHorizontal();

            if (Equals(tmpMin, minVal) && Equals(tmpMax, maxVal))
                return false;

            minVal = tmpMin;
            maxVal = tmpMax;
            return true;
        }

        public static bool Button(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style != null)
                return GUILayout.Button(text, style, options);
            else
                return GUILayout.Button(text, options);
        }

        public static bool Button(string text, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, options);
        }

        public static void FlexibleSpace()
        {
            GUILayout.FlexibleSpace();
        }

        public static void Space(float pixel)
        {
            GUILayout.Space(pixel);
        }

        public static Rect GetLastRect()
        {
            return GUILayoutUtility.GetLastRect();
        }

        public static bool ObjectField<T>(string label, ref T val, params GUILayoutOption[] options) where T : Object
        {
            var tmpObj = EditorGUILayout.ObjectField(label, val, typeof(T), options);
            if (Equals(tmpObj, val))
                return false;
            val = tmpObj as T;
            return true;
        }

        public static bool Color(string label, ref Color color, params GUILayoutOption[] options)
        {
            var tmpColor = EditorGUILayout.ColorField(label, color, options);
            if (Equals(tmpColor, color))
                return false;
            color = tmpColor;
            return true;
        }

        public static bool Vector3IntField(string label, ref Vector3Int val, params GUILayoutOption[] options)
        {
            var tmpVal = EditorGUILayout.Vector3IntField(label, val, options);
            if (Equals(tmpVal, val))
                return false;
            val = tmpVal;
            return true;
        }

        public static bool Vector2IntField(string label, ref Vector2Int val, params GUILayoutOption[] options)
        {
            var tmpVal = EditorGUILayout.Vector2IntField(label, val, options);
            if (Equals(tmpVal, val))
                return false;
            val = tmpVal;
            return true;
        }

        public static bool Vector3Field(string label, ref Vector3 val, params GUILayoutOption[] options)
        {
            var tmpVal = EditorGUILayout.Vector3Field(label, val, options);
            if (Equals(tmpVal, val))
                return false;
            val = tmpVal;
            return true;
        }

        public static bool Vector2Field(string label, ref Vector2 val, params GUILayoutOption[] options)
        {
            var tmpVal = EditorGUILayout.Vector2Field(label, val, options);
            if (Equals(tmpVal, val))
                return false;
            val = tmpVal;
            return true;
        }

        public static bool TextField(ref string val, params GUILayoutOption[] options)
        {
            var tmpVal = EditorGUILayout.TextField(val, options);
            if (Equals(tmpVal, val))
                return false;
            val = tmpVal;
            return true;
        }

        public static void LabelField(string label, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style != null)
                EditorGUILayout.LabelField(label, style, options);
            else
                EditorGUILayout.LabelField(label, options);
        }

        public static void Label(Rect rect, string content)
        {
            GUI.Label(rect, content);
        }

        public static void Label(Rect rect, GUIContent content)
        {
            GUI.Label(rect, content);
        }

        public static void Label(GUIContent label, params GUILayoutOption[] options)
        {
            GUILayout.Label(label, options);
        }

        public static void Label(string content, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, options);
        }

        public static void Label(string content, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, style, options);
        }

        public static bool IntField(ref int val, GUIContent label, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.IntField(label, val, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool IntField(ref int val, string label, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.IntField(label, val, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool LongField(ref long val, string label, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.LongField(label, val, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool LongField(ref long val, GUIContent label, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.LongField(label, val, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool FloatField(ref float val, GUIContent label, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.FloatField(label, val, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool FloatField(ref float val, string label, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.FloatField(label, val, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool DoubleField(ref double val, string label, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.DoubleField(label, val, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool DoubleField(ref double val, GUIContent label, params GUILayoutOption[] options)
        {
            var tmp = EditorGUILayout.DoubleField(label, val, options);
            if (Equals(tmp, val))
                return false;
            val = tmp;
            return true;
        }

        public static bool SearchBar(ref string _SearchString, params GUILayoutOption[] options)
        {
            var tmpStr = GUILayout.TextField(_SearchString, GUI.skin.FindStyle("ToolbarSeachTextField"), options);
            if (Equals(tmpStr, _SearchString))
                return false;
            _SearchString = tmpStr;
            return true;
        }

        public static bool IntMinMaxSlider(string label, ref int minVal, ref int maxVal, int min, int max,
            bool readOnly = false,
            params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            float tmpMin = minVal;
            float tmpMax = maxVal;
            if (readOnly)
            {
                GUILayout.Label(minVal.ToString(), GUILayout.Width(50));
                GUILayout.Label(maxVal.ToString(), GUILayout.Width(50));
            }
            else
            {
                var tmpMinStr = GUILayout.TextField(minVal.ToString(), GUILayout.Width(50));
                if (float.TryParse(tmpMinStr, out var tmpMinVal))
                {
                    tmpMin = tmpMinVal;
                }
                else
                {
                    tmpMin = min;
                }

                tmpMin = Mathf.Clamp(tmpMin, min, max);

                var tmpMaxStr = GUILayout.TextField(maxVal.ToString(), GUILayout.Width(50));
                if (float.TryParse(tmpMaxStr, out var tmpMaxVal))
                {
                    tmpMax = tmpMaxVal;
                }
                else
                {
                    tmpMax = max;
                }

                tmpMax = Mathf.Clamp(tmpMax, tmpMin, max);
            }

            EditorGUILayout.MinMaxSlider(ref tmpMin, ref tmpMax, min, max, options);

            GUILayout.EndHorizontal();

            if (Equals(Mathf.FloorToInt(tmpMin), minVal) && Equals(Mathf.FloorToInt(tmpMax), maxVal))
                return false;

            minVal = Mathf.FloorToInt(tmpMin);
            maxVal = Mathf.FloorToInt(tmpMax);
            return true;
        }

        public static void ScrollView(ref Vector2 scrollPosition, Action drawContent, params GUILayoutOption[] options)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, options);
            drawContent?.Invoke();
            GUILayout.EndScrollView();
        }

        public static void Foldout(string label, ref bool foldout, GUIStyle style = null)
        {
            if (style == null)
                foldout = EditorGUILayout.Foldout(foldout, label);
            else
            {
                foldout = EditorGUILayout.Foldout(foldout, label, style);
            }
        }
    }
}