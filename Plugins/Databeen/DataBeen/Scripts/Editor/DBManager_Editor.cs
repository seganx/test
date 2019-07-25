using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System;

public class DBManager_Window : EditorWindow {


    public static bool inited
    {
        get
        {
            return FindObjectOfType<DataBeen>() != null;
        }
    }

    [MenuItem("DataBeen/Setup")]
    static void Init()
    {
        SetupBI();
    }

    public void AnalyticGroupFade()
    {
        
    }

    public static void SetupBI()
    {
        if (inited)
            return;
        new GameObject("DataBeen").AddComponent<DataBeen>();
    }
}

[CustomEditor(typeof(DataBeen))]
public class BIManager_Editor : Editor
{
    private DataBeen t;
    private SerializedObject getTarget;
    private SerializedProperty apiKey;
    private SerializedProperty storeTarget;
    private SerializedProperty customtarget;

    void OnEnable()
    {
        t = (DataBeen)target;
        getTarget = new SerializedObject(t);
        apiKey = getTarget.FindProperty("apiKey");
        storeTarget = getTarget.FindProperty("target");
        customtarget = getTarget.FindProperty("customTarget");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("SDK Version: ");
        EditorGUILayout.LabelField(DataBeenConnection.PacketsIdentity.sdkVersion);

        EditorGUILayout.EndHorizontal();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        EditorGUI.EndDisabledGroup();


        EditorGUILayout.PrefixLabel("SECRET KEY");
        apiKey.stringValue = EditorGUILayout.TextField(apiKey.stringValue);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.PrefixLabel("MARKET");
        storeTarget.enumValueIndex = EditorGUILayout.Popup(storeTarget.enumValueIndex, storeTarget.enumNames);

        if ((DataBeenConnection.MarketType)storeTarget.enumValueIndex == DataBeenConnection.MarketType.Custom)
        {
            EditorGUILayout.Space();
            EditorGUILayout.PrefixLabel("MARKET NAME");
            customtarget.stringValue = EditorGUILayout.TextField(customtarget.stringValue);
        }


        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space();

        EventRow("1- Purchase", "http://databeen.ir/documents/#purchase");
        EditorGUILayout.Space();
        EventRow("2- Start Level", "http://databeen.ir/documents/#startlevel");
        EditorGUILayout.Space();
        EventRow("3- End Level", "http://databeen.ir/documents/#endlevel");
        EditorGUILayout.Space();
        EventRow("4- Content View", "http://databeen.ir/documents/#contentview");
        EditorGUILayout.Space();
        EventRow("4- Shared", "http://databeen.ir/documents/#sharebtn");
        EditorGUILayout.Space();
        EventRow("5- Rated", "http://databeen.ir/documents/#rateapp");
        EditorGUILayout.Space();
        EventRow("6- Custom Event", "http://databeen.ir/documents/#customevent");
        getTarget.ApplyModifiedProperties();
    }

    public void EventRow(string bName, string url)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox(bName, MessageType.None);
        if (GUILayout.Button("Help"))
        {
            Application.OpenURL(url);
        }
        EditorGUILayout.EndHorizontal();
    }
}