using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class TrelloWindow : EditorWindow

{
    GameObject prefab;
    Texture2D texture;

    [MenuItem("Window/My Window")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TrelloWindow));
    }
    [MenuItem("Example/ObjectField Example _h")]
    static void Init()
    {
        var window = GetWindowWithRect<TrelloWindow>(new Rect(0, 0, 165, 100));
        window.Show();
    }
    void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("MyObj", prefab, typeof(GameObject), true);

        if (prefab) {

            texture = AssetPreview.GetAssetPreview(prefab);
        }
        if (texture)
        {
            EditorGUI.DrawPreviewTexture(new Rect(0, 0, 120, 120), texture);
        }
    }
}