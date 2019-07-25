using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorPalette))]
public class ColorPaletteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cp = target as ColorPalette;
        var list = cp.sharedColors;
        list.Sort((x, y) => x.id - y.id);

        if (GUILayout.Button("Add"))
        {
            var maxitem = list.FindMax(x => x.id);
            var maxid = maxitem != null ? maxitem.id + 10 : 10;
            list.Add(new ColorPalette.GameColor() { id = maxid, color = Color.white });
        }

        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            GUILayout.BeginHorizontal("BOX");

            EditorGUILayout.LabelField(item.id + ":", GUILayout.MaxWidth(30));
            var newid = EditorGUILayout.DelayedIntField(item.id, GUILayout.MaxWidth(40));
            if (newid != item.id)
            {
                var dublicated = list.Find(x => x != item && x.id == newid);
                if (dublicated != null)
                    EditorUtility.DisplayDialog("Duplicated", "Entered id is exist", "OK");
                else
                    item.id = newid;
            }

            item.color = EditorGUILayout.ColorField(item.color);
            if (GUILayout.Button("X", GUILayout.Width(EditorGUIUtility.singleLineHeight * 1.2f)))
            {
                list.RemoveAt(i--);
            }
            GUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(cp);
    }
}
