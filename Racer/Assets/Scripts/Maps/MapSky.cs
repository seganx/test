using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Game/Sky")]
public class MapSky : ScriptableObject
{
    [Header("Environment")]
    public bool isNight = false;
    public Material skyBoxMaterial = null;

    [Header("Environment Lighting")]
    public Color sunColor = Color.black;
    [ColorUsage(false, true, 0, 8, 0.125f, 3)]
    public Color skyColor = Color.black;
    [ColorUsage(false, true, 0, 8, 0.125f, 3)]
    public Color equatorColor = Color.black;
    [ColorUsage(false, true, 0, 8, 0.125f, 3)]
    public Color groundColor = Color.black;

    [Header("Environment Reflection")]
    public Cubemap reflectionCubemap = null;

    [Header("Fog Settings")]
    public bool fogActive = false;
    public Color fogColor = Color.black;
    public float fogStrat = 300;
    public float fogEnd = 500;


    public MapSky Perform(GameMap map)
    {
        RenderSettings.skybox = skyBoxMaterial;
        RenderSettings.sun = map.sunSource;
        map.sunSource.color = sunColor;

        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = skyColor;
        RenderSettings.ambientEquatorColor = equatorColor;
        RenderSettings.ambientGroundColor = groundColor;

        RenderSettings.customReflection = reflectionCubemap;
        RenderSettings.defaultReflectionMode = reflectionCubemap == null ? DefaultReflectionMode.Skybox : DefaultReflectionMode.Custom;

        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fog = fogActive;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogStartDistance = fogStrat;
        RenderSettings.fogEndDistance = fogEnd;

        return this;
    }

#if UNITY_EDITOR
    [InspectorButton(100, "Copy sky", "Copy", "Perform", "EditorPerform", true)]
    public bool insbutton = false;
    public void Copy(object sender)
    {
        var map = FindObjectOfType<GameMap>();
        if (map == null) return;
        sunColor = map.sunSource.color;
    }

    public void EditorPerform(object sender)
    {
        var map = FindObjectOfType<GameMap>();
        if (map == null) return;
        Perform(map);
    }
#endif

}
