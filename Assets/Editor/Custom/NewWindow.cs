
using System;
using EditorUIMaker;
using UnityEditor;
using UnityEngine;
public class NewWindow : EditorWindow
{
    [MenuItem("Tools/NewWindow")]
    public static void ShowWindow()
    {
        var window = GetWindow<NewWindow>();
        window.titleContent = new GUIContent("NewWindow");
        window.Show();
    }

    private NewWindow_Logic _Logic;

    public NewWindow()
    {
        _Logic = new NewWindow_Logic();
    }

    void OnGUI()
    {
        
        GUILayout.BeginHorizontal();
        
        GUILayout.EndHorizontal();
        
        if(GUILib.Popup(ref _Logic.Dropdown1Str,_Logic.Dropdown1Options))
        {
            _Logic.Dropdown1Index = Array.IndexOf(_Logic.Dropdown1Options,_Logic.Dropdown1Str);
            _Logic.Dropdown1ValueChange();
        }

    }
}
