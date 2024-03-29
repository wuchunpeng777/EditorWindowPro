using System;
using System.Collections;
using System.Collections.Generic;
using EditorUIMaker.Widgets;
using OdinSerializer;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EditorUIMaker
{
    public class EditorUIMaker : EditorWindow,ISerializationCallbackReceiver
    {
        public static Action OnInit;
        
        private static EditorUIMaker _Instance;
        public static bool IsOpen
        {
            get
            {
                return _Instance != null;
            }
        }
        
        [MenuItem("Tools/EditorUIMaker")]
        public static void OpenWindow()
        {
            var window = GetWindow<EditorUIMaker>();
            window.Init();
            window.titleContent = new GUIContent(EUM_Helper.Instance.WindowName);
            window.Show();
            window.Focus();
        }
        
        static void OpenWindowWithData(EUM_Object data,string filePath)
        {
            if(!EditorUIMaker.IsOpen)
                OpenWindow();
            EUM_Helper.Instance.LoadData(data,filePath);
        }

        [OnOpenAsset()]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is EUM_Object data)
            {
                var filePath = AssetDatabase.GetAssetPath(instanceID);
                OpenWindowWithData(data,filePath);
                return true;
            }

            return false;
        }


        public const float s_SplitSize = 2f;
        public const float s_MinInspectorWidth = 300;
        public const float s_MinViewportWidth = 200;
        public const float s_MinOperationWidth = 200;

        public EUM_OperationArea OperationArea;
        public EUM_Viewport Viewport;
        public EUM_Inspector Inspector;
        public EUM_Inputer Input;

        public float RatioInspector = 0.2f;
        public float RatioOperationArea = 0.1f;
        public bool ResizeInspector = false;
        public bool ResizeOperationArea = false;

        public Rect _InspectorRect;
        public EUM_Helper _Helper;

        void Init()
        {
            _Helper = new EUM_Helper();
            EUM_Helper.Instance = _Helper;


            Viewport = new EUM_Viewport();
            OperationArea = new EUM_OperationArea();
            Inspector = new EUM_Inspector();
            Input = new EUM_Inputer();

            EUM_Helper.Instance.OnRemoveItemFromWindow += OnRemoveItemFromWindow;
            EUM_Helper.Instance.OnAddItemToWindow += OnAddItemToWindow;
            EUM_Helper.Instance.OnItemIndexChange += OnItemIndexChange;
            
            EUM_Helper.Instance.ClearData();
            
            OnInit?.Invoke();
        }

        public void OnEnable()
        {
            _Instance = this;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        public void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        void OnBeforeAssemblyReload()
        {
            EUM_Helper.Instance.OnBeforeReloadDomain?.Invoke();
        }
        
        void OnAfterAssemblyReload()
        {
            EUM_Helper.Instance = _Helper;
            EUM_Helper.Instance.OnAfterReloadDomain?.Invoke();
        }

        void OnItemIndexChange()
        {
            OnModified();
        }

        void OnAddItemToWindow(EUM_BaseWidget widget)
        {
            OnModified();
        }

        void OnRemoveItemFromWindow(EUM_BaseWidget widget)
        {
            OnModified();
        }

        void OnModified()
        {
            EUM_Helper.Instance.Modified = true;
        }

        public void OnGUI()
        {
            DoOnGUI();
        }
        
        void DoOnGUI()
        {
            EUM_Helper.Instance.MouseRects.Clear();

            OperationArea.Library.HandleDrag();

            EUM_Helper.Instance.Fade();

            var operationWidth = position.width * RatioOperationArea;
            operationWidth = Mathf.Max(operationWidth, s_MinOperationWidth);
            
            var inspectorWidth = position.width * RatioInspector;
            inspectorWidth = Mathf.Max(inspectorWidth, s_MinInspectorWidth);
            
            var viewportWidth = position.width - inspectorWidth - operationWidth;
            viewportWidth = Mathf.Max(viewportWidth, s_MinViewportWidth);
            
            var viewportRect = new Rect(operationWidth,0 , viewportWidth - s_SplitSize, position.height);
            Viewport.Draw(ref viewportRect);

            var inspectorRect = new Rect(viewportRect.x + viewportRect.width + s_SplitSize, 0, inspectorWidth,
                position.height);
            Inspector.Draw(ref inspectorRect);
            EUM_Helper.Instance.MouseRects.Add(inspectorRect);
            _InspectorRect = inspectorRect;

            var operationRect = new Rect(0, 0, operationWidth, position.height);
            OperationArea.Draw(ref operationRect);

            var operationSplitRect = new Rect(operationRect.x + operationRect.width, 0, 2, position.height);
            GUILib.Rect(operationSplitRect, Color.black, 0.4f);
            var operationSplitCursorRect = GUILib.Padding(operationSplitRect, -2f, -2f);
            EditorGUIUtility.AddCursorRect(operationSplitCursorRect, MouseCursor.ResizeHorizontal);
            EUM_Helper.Instance.MouseRects.Add(operationSplitCursorRect);

            var inspectorSplitRect = new Rect(viewportRect.x + viewportRect.width, 0, 2, position.height);
            GUILib.Rect(inspectorSplitRect, Color.black, 0.4f);
            var inspectorSplitCursorRect = GUILib.Padding(inspectorSplitRect, -2f, -2f);
            EditorGUIUtility.AddCursorRect(inspectorSplitCursorRect, MouseCursor.ResizeHorizontal);
            EUM_Helper.Instance.MouseRects.Add(inspectorSplitCursorRect);

            if (!EUM_Helper.Instance.Preview)
            {
                DrawDraging();
                DrawHoverRect();
                CheckSelectRect();
                DrawSelectRect();
            }
            else
            {
                DrawPreviewFrame();
            }

            if (Event.current.type == EventType.MouseDown &&
                inspectorSplitCursorRect.Contains(Event.current.mousePosition))
            {
                ResizeInspector = true;
                RefreshInspectorSplitPosition();
            }

            if (ResizeInspector)
            {
                RefreshInspectorSplitPosition();
            }

            if (Event.current.type == EventType.MouseDown &&
                operationSplitCursorRect.Contains(Event.current.mousePosition))
            {
                ResizeOperationArea = true;
                RefreshOperationSplitPosition();
            }

            if (ResizeOperationArea)
            {
                RefreshOperationSplitPosition();
            }

            if (Event.current.rawType == EventType.MouseUp)
            {
                if (ResizeInspector)
                    ResizeInspector = false;
                if (ResizeOperationArea)
                    ResizeOperationArea = false;
                if(ResizeWidgetHeight)
                    ResizeWidgetHeight = false;
            }

            Input.CheckInput();

            Repaint();
        }

        void DrawPreviewFrame()
        {
            GUILib.Frame(EUM_Helper.Instance.ViewportRect, new Color(255f / 255, 139f / 255, 40f / 255), 2);
        }

        void DrawDraging()
        {
            if (EUM_Helper.Instance.DraggingWidget != null)
            {
                EUM_Helper.Instance.DraggingWidget.DrawDragging(Event.current.mousePosition);

                if (!EUM_Helper.Instance.ViewportRect.Contains(Event.current.mousePosition))
                {
                    EUM_Helper.Instance.DraggingOverContainer = null;
                    return;
                }

                if (!EUM_Helper.Instance.VitualWindowRect.Contains(Event.current.mousePosition))
                {
                    EUM_Helper.Instance.DraggingOverContainer = null;
                    return;
                }

                EUM_Container container = null;

                foreach (var item in EUM_Helper.Instance.Containers)
                {
                    if (!item.InViewport)
                        continue;
                    if (!item.Contains(Event.current.mousePosition))
                        continue;
                    if (container == null)
                    {
                        container = item;
                    }
                    else
                    {
                        if (item.Depth > container.Depth)
                            container = item;
                    }
                }

                if (container != null)
                {
                    DrawDraggingOverRect(container);
                    if (EUM_Helper.Instance.DraggingOverContainer != container)
                        EUM_Helper.Instance.ResetFade();
                    EUM_Helper.Instance.DraggingOverContainer = container;
                }
                else
                {
                    EUM_Helper.Instance.DraggingOverContainer = null;
                }
            }
            else
            {
                EUM_Helper.Instance.DraggingOverContainer = null;
            }
        }

        void DrawDraggingOverRect(EUM_Container container)
        {
            GUILib.Frame(container.Rect, Color.blue, EUM_Helper.Instance.ViewportRect);
        }

        void CheckSelectRect()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if(operationCursorRect.Contains(Event.current.mousePosition))
                    return;
                
                GUIUtility.keyboardControl = 0;

                //如果点击鼠标拖拽区域了，则不处理选中逻辑
                foreach (var mouseRect in EUM_Helper.Instance.MouseRects)
                {
                    if (mouseRect.Contains(Event.current.mousePosition))
                        return;
                }

                var oldSelect = EUM_Helper.Instance.SelectWidget;
                if (EUM_Helper.Instance.HoverWidget == null)
                {
                    EUM_Helper.Instance.SelectWidget = null;
                }
                else
                {
                    var widget = EUM_Helper.Instance.HoverWidget;
                    if (widget.Contains(Event.current.mousePosition))
                    {
                        EUM_Helper.Instance.SelectWidget = widget;
                    }
                }

                if (EUM_Helper.Instance.SelectWidget != oldSelect)
                    EUM_Helper.Instance.OnSelectWidgetChange?.Invoke();
            }
        }

        public bool ResizeWidgetHeight = false;
        public Vector2 LastResizeHeightDownPosition;
        public Rect operationCursorRect;
        void DrawSelectRect()
        {
            if (EUM_Helper.Instance.SelectWidget == null)
                return;
            var widget = EUM_Helper.Instance.SelectWidget;
            var visibleRect = GUILib.Frame(widget.Rect, Color.green, EUM_Helper.Instance.ViewportRect);

            if (widget.CanResize())
            {
                var resizeRect = new Rect(visibleRect.xMin, visibleRect.yMax, visibleRect.width, 1);
                operationCursorRect = GUILib.Padding(resizeRect, 5f, -2f);
                EditorGUIUtility.AddCursorRect(operationCursorRect, MouseCursor.ResizeVertical);
                if (operationCursorRect.Contains(Event.current.mousePosition))
                {
                    GUILib.Rect(operationCursorRect, Color.white);
                }

                if (Event.current.type == EventType.MouseDown &&
                    operationCursorRect.Contains(Event.current.mousePosition))
                {
                    LastResizeHeightDownPosition = Event.current.mousePosition;
                    ResizeWidgetHeight = true;
                    RefreshResizeWidgetHeight();
                }

                if (ResizeWidgetHeight)
                    RefreshResizeWidgetHeight();
            }
        }

        void RefreshResizeWidgetHeight()
        {
            if(Event.current.type != EventType.Repaint)
                return;
            var curHeight = EUM_Helper.Instance.SelectWidget.Rect.height;
            var delta = Event.current.mousePosition.y - LastResizeHeightDownPosition.y;
            var newHeight = curHeight + delta;
            newHeight = Mathf.Max(newHeight, EUM_BaseInfo.Min_Height);
            EUM_Helper.Instance.SelectWidget.Info.Height = newHeight;
            LastResizeHeightDownPosition = Event.current.mousePosition;
        }

        void DrawHoverRect()
        {
            if (EUM_Helper.Instance.DraggingWidget != null)
            {
                EUM_Helper.Instance.HoverWidget = null;
                return;
            }

            EUM_BaseWidget widget = null;

            Queue<EUM_BaseWidget> checkList = new Queue<EUM_BaseWidget>();

            foreach (var item in EUM_Helper.Instance.Containers)
            {
                if (!item.InViewport)
                    continue;
                if (!item.Contains(Event.current.mousePosition))
                    continue;
                //在区域内，进而判断是否在子控件内
                checkList.Enqueue(item);
                while (checkList.Count > 0)
                {
                    var checkItem = checkList.Dequeue();
                    //if depth == 0 , is root window,dont check hover
                    if (checkItem.Depth != 0 && checkItem.Contains(Event.current.mousePosition))
                    {
                        if (widget == null)
                        {
                            widget = checkItem;
                        }
                        else
                        {
                            if (checkItem.Depth > widget.Depth)
                                widget = checkItem;
                        }
                    }

                    if (checkItem is EUM_Container)
                    {
                        var container = checkItem as EUM_Container;
                        foreach (var child in container.Widgets)
                        {
                            checkList.Enqueue(child);
                        }
                    }
                }
            }

            if (widget == null)
            {
            }

            if (widget != null)
            {
                GUILib.Frame(widget.Rect, Color.blue, EUM_Helper.Instance.ViewportRect, 1.5f);
                if (EUM_Helper.Instance.HoverWidget != widget)
                    EUM_Helper.Instance.ResetFade();
                EUM_Helper.Instance.HoverWidget = widget;
            }
            else
            {
                var treeHoverItem = OperationArea.Hierarchy.TreeView.HoverItem;
                if (treeHoverItem != null && EUM_Helper.Instance.Widgets.ContainsKey(treeHoverItem.id))
                {
                    widget = EUM_Helper.Instance.Widgets[treeHoverItem.id];
                    GUILib.Frame(widget.Rect, Color.blue, EUM_Helper.Instance.ViewportRect, 1.5f);
                }

                EUM_Helper.Instance.HoverWidget = null;
            }
        }


        void RefreshOperationSplitPosition()
        {
            if (position.width <= s_MinInspectorWidth + s_MinViewportWidth + s_MinOperationWidth)
                return;
            var delta = Event.current.mousePosition.x;
            if (delta < s_MinOperationWidth)
                return;

            delta = Mathf.Max(delta, s_MinOperationWidth);
            delta = Mathf.Min(delta,position.width - s_MinViewportWidth- s_MinInspectorWidth);
            
            RatioOperationArea = delta / position.width;
            
            var inspectorWidth = RatioInspector * position.width;
            if(delta + inspectorWidth + s_MinViewportWidth > position.width)
                RatioInspector = (position.width - delta - s_MinViewportWidth) / position.width;
        }

        void RefreshInspectorSplitPosition()
        {
            if (position.width <= s_MinInspectorWidth + s_MinViewportWidth + s_MinOperationWidth)
                return;
            var delta = position.width - Event.current.mousePosition.x;
            if (delta < s_MinInspectorWidth)
                return;
            
            delta = Mathf.Max(delta, s_MinInspectorWidth);
            delta = Mathf.Min(delta,position.width - s_MinViewportWidth- s_MinOperationWidth);
            
            RatioInspector = delta / position.width;
            
            var operationWidth = RatioOperationArea * position.width;
            if(delta + operationWidth + s_MinViewportWidth > position.width)
                RatioOperationArea = (position.width - delta - s_MinViewportWidth) / position.width;
        }

       
        private void OnDestroy()
        {
            if (!EUM_Helper.Instance.Modified)
                return;
            EUM_Helper.Instance.WarningModified();
        }

        [SerializeField]
        [HideInInspector]
        private SerializationData serializationData;
        public void OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject((UnityEngine.Object) this, ref this.serializationData); 
        }

        public void OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject((UnityEngine.Object) this, ref this.serializationData); 
        }
        
    }
}