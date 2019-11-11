using SeganX;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RacerGlobalConfigs))]
public class RacerGlobalConfigsEditor : Editor
{
    private static bool foldRacersGrid = false;
    private static bool foldUpgradeGrid = false;

    private void OnEnable()
    {
        var curr = target as RacerGlobalConfigs;
        var racers = RacerFactory.Racer.AllConfigs;

        //  add new items
        foreach (var item in racers)
        {
            if (curr.data.racers.Exists(x => x.id == item.Id)) continue;
            curr.data.racers.Add(new RacerGlobalConfigs.Racer() { id = item.Id, name = item.name });
        }

        //  remove removed items
        curr.data.racers.RemoveAll(x => racers.Exists(y => x.id == y.Id) == false);

        if (curr.data.maxUpgradeLevel.Count < 5)
            curr.data.maxUpgradeLevel = new List<int>(5) { 8, 5, 3, 1, 1 };

        while (curr.data.speedUpgradeValue.Count < curr.data.TotalUpgradeLevels)
            curr.data.speedUpgradeValue.Add(curr.data.speedUpgradeValue.Count);

        while (curr.data.nitroUpgradeValue.Count < curr.data.TotalUpgradeLevels)
            curr.data.nitroUpgradeValue.Add(curr.data.nitroUpgradeValue.Count);

        while (curr.data.steeringUpgradeValue.Count < curr.data.TotalUpgradeLevels)
            curr.data.steeringUpgradeValue.Add(curr.data.steeringUpgradeValue.Count);

        while (curr.data.bodyUpgradeValue.Count < curr.data.TotalUpgradeLevels)
            curr.data.bodyUpgradeValue.Add(curr.data.bodyUpgradeValue.Count);
    }

    public override void OnInspectorGUI()
    {
        var obj = target as RacerGlobalConfigs;
        var list = obj.data.racers;
        list.Sort((x, y) => x.id - y.id);

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("Upgrade Levels:", GUILayout.MinWidth(100));
        for (int i = 0; i < obj.data.maxUpgradeLevel.Count; i++)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(i.ToString(), GUILayout.MaxWidth(10));
            obj.data.maxUpgradeLevel[i] = EditorGUILayout.DelayedIntField(obj.data.maxUpgradeLevel[i], GUILayout.MaxWidth(30));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        const float wint = 35;
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("Power Ratio:", GUILayout.MinWidth(100));
        EditorGUILayout.LabelField("Speed", GUILayout.MaxWidth(40));
        obj.data.speedPowerRatio = EditorGUILayout.DelayedFloatField(obj.data.speedPowerRatio, GUILayout.MaxWidth(wint));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Nitro", GUILayout.MaxWidth(30));
        obj.data.nitroPowerRatio = EditorGUILayout.DelayedFloatField(obj.data.nitroPowerRatio, GUILayout.MaxWidth(wint));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Steering", GUILayout.MaxWidth(55));
        obj.data.steeringPowerRatio = EditorGUILayout.DelayedFloatField(obj.data.steeringPowerRatio, GUILayout.MaxWidth(wint));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Body", GUILayout.MaxWidth(35));
        obj.data.bodyPowerRatio = EditorGUILayout.DelayedFloatField(obj.data.bodyPowerRatio, GUILayout.MaxWidth(wint));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        DrawUpgradeLists(obj);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DrawRacersList(list, obj);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Export"))
        {
            var path = Directory.GetParent(Application.dataPath).Parent.FullName + "/Configs/" + GlobalConfig.Instance.version;
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
            var filename = EditorUtility.SaveFilePanel("Save exported data", path, "racers", "txt");
            if (filename.HasContent(4))
                File.WriteAllText(filename, JsonUtility.ToJson(obj.data, false), System.Text.Encoding.UTF8);
        }
        if (GUILayout.Button("Import"))
        {
            var filename = EditorUtility.OpenFilePanel("Load data from", Path.GetDirectoryName(Application.dataPath), "txt");
            if (filename.HasContent(4))
                obj.data = JsonUtility.FromJson<RacerGlobalConfigs.ConfigData>(File.ReadAllText(filename, System.Text.Encoding.UTF8));
        }

        EditorUtility.SetDirty(obj);
    }

    private static void DrawUpgradeLists(RacerGlobalConfigs obj)
    {
        foldUpgradeGrid = EditorGUILayout.Foldout(foldUpgradeGrid, "Upgrades");
        if (foldUpgradeGrid)
        {
            const float wint = 100;
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("speed", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("nitro", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("steering", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("body", GUILayout.MaxWidth(wint));
            GUILayout.EndHorizontal();

            for (int i = 0; i < obj.data.TotalUpgradeLevels; i++)
            {
                GUILayout.BeginHorizontal("box");
                obj.data.speedUpgradeValue[i] = EditorGUILayout.DelayedFloatField(obj.data.speedUpgradeValue[i], GUILayout.MaxWidth(wint));
                obj.data.nitroUpgradeValue[i] = EditorGUILayout.DelayedFloatField(obj.data.nitroUpgradeValue[i], GUILayout.MaxWidth(wint));
                obj.data.steeringUpgradeValue[i] = EditorGUILayout.DelayedFloatField(obj.data.steeringUpgradeValue[i], GUILayout.MaxWidth(wint));
                obj.data.bodyUpgradeValue[i] = EditorGUILayout.DelayedFloatField(obj.data.bodyUpgradeValue[i], GUILayout.MaxWidth(wint));
                GUILayout.EndHorizontal();
            }
        }
    }

    private static void DrawRacersList(List<RacerGlobalConfigs.Racer> list, RacerGlobalConfigs obj)
    {
        foldRacersGrid = EditorGUILayout.Foldout(foldRacersGrid, "Racers");
        if (foldRacersGrid)
        {
            //if (DrawLinearParamButton("Group", ref obj.editorGroupParam))
            //  for (int i = 0; i < list.Count; i++)
            //    list[i].groupId = Mathf.FloorToInt(obj.editorGroupParam.x + obj.editorGroupParam.y * i);

            //if (DrawLinearParamButton("Cards", ref obj.editorCardsParam))
            //  for (int i = 0; i < list.Count; i++)
            //    list[i].cardCount = Mathf.FloorToInt(obj.editorCardsParam.x + obj.editorCardsParam.y * i);

            //if (DrawLinearParamButton("Price", ref obj.editorPriceParam))
            //  for (int i = 0; i < list.Count; i++)
            //    list[i].price = Mathf.FloorToInt(obj.editorPriceParam.x + obj.editorPriceParam.y * i);

            if (DrawLinearParamButton("Speed", ref obj.editorSpeedParam))
                for (int i = 0; i < list.Count; i++)
                    list[i].speedBaseValue = obj.editorSpeedParam.x + obj.editorSpeedParam.y * i;

            if (DrawLinearParamButton("Nitro", ref obj.editorNitroParam))
                for (int i = 0; i < list.Count; i++)
                    list[i].nitroBaseValue = obj.editorNitroParam.x + obj.editorNitroParam.y * i;

            if (DrawLinearParamButton("Steering", ref obj.editorSteeringParam))
                for (int i = 0; i < list.Count; i++)
                    list[i].steeringBaseValue = obj.editorSteeringParam.x + obj.editorSteeringParam.y * i;

            if (DrawLinearParamButton("Body", ref obj.editorBodyParam))
                for (int i = 0; i < list.Count; i++)
                    list[i].bodyBaseValue = obj.editorBodyParam.x + obj.editorBodyParam.y * i;
            EditorGUILayout.Space();

            var wint = 40;
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("id", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("Name", GUILayout.MinWidth(150));
            EditorGUILayout.LabelField("group", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("cards", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("price", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("speed", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("nitro", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("steer", GUILayout.MaxWidth(wint));
            EditorGUILayout.LabelField("body", GUILayout.MaxWidth(wint));
            GUILayout.EndHorizontal();

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                GUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(item.id.ToString(), GUILayout.MaxWidth(wint));
                EditorGUILayout.LabelField(item.name.Persian(false), GUILayout.MinWidth(100));
                item.name = EditorGUILayout.TextField(item.name, GUILayout.MaxWidth(wint * 3));
                item.groupId = EditorGUILayout.DelayedIntField(item.groupId, GUILayout.MaxWidth(wint));
                item.cardCount = EditorGUILayout.DelayedIntField(item.cardCount, GUILayout.MaxWidth(wint));
                item.price = EditorGUILayout.DelayedIntField(item.price, GUILayout.MaxWidth(wint));
                item.speedBaseValue = EditorGUILayout.DelayedFloatField(item.speedBaseValue, GUILayout.MaxWidth(wint));
                item.nitroBaseValue = EditorGUILayout.DelayedFloatField(item.nitroBaseValue, GUILayout.MaxWidth(wint));
                item.steeringBaseValue = EditorGUILayout.DelayedFloatField(item.steeringBaseValue, GUILayout.MaxWidth(wint));
                item.bodyBaseValue = EditorGUILayout.DelayedFloatField(item.bodyBaseValue, GUILayout.MaxWidth(wint));
                GUILayout.EndHorizontal();
            }
        }
    }

    private static bool DrawLinearParamButton(string valueName, ref Vector3 equationParam)
    {
        GUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(valueName + " = ", GUILayout.MaxWidth(100));
        equationParam.x = EditorGUILayout.FloatField(equationParam.x, GUILayout.MaxWidth(60));
        EditorGUILayout.LabelField(" + ", GUILayout.MaxWidth(20));
        equationParam.y = EditorGUILayout.FloatField(equationParam.y, GUILayout.MaxWidth(60));
        EditorGUILayout.LabelField(" * index", GUILayout.MaxWidth(160));
        var res = GUILayout.Button("Perform");
        GUILayout.EndHorizontal();
        return res;
    }
}
