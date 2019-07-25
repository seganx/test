using SeganX;
using SeganX.Effects;
using UnityEngine;
using UnityEngine.Rendering;

public class GameMap : MonoBehaviour
{
    [Header("Environment")]
    public Material skyBoxMaterial = null;
    public Light sunSource = null;

    [Header("Environment Lighting")]
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

    [Header("SeganX Effects")]
    public CameraFX camerafx = null;
    public float skyBloom = 0.5f;
    public float bloomSpecular = 0.8f;
    public float bloomOffsetFactor = 1;
    public int bloomDownScale = 16;
    public Material bloomDownScaleMaterial = null;
    public Material bloomPostMaterial = null;
    public Material postMaterial = null;


    public int Id { get; private set; }

    // Use this for initialization
    private void Start()
    {
        RenderSettings.skybox = skyBoxMaterial;
        RenderSettings.sun = sunSource;

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

        Shader.SetGlobalFloat("bloomSpecular", bloomSpecular);
        Shader.SetGlobalFloat("skyBloom", skyBloom);

        camerafx.postMaterial = postMaterial.Clone();
        camerafx.bloom.downScaleMaterial = bloomDownScaleMaterial.Clone();
        camerafx.bloom.postMaterial = bloomPostMaterial.Clone();
        if (camerafx.bloom.scaleFactor != bloomDownScale || camerafx.bloom.offsetFactor != bloomOffsetFactor)
        {
            camerafx.bloom.scaleFactor = bloomDownScale;
            camerafx.bloom.offsetFactor = bloomOffsetFactor;
            camerafx.bloom.Clear();
        }
    }

    private void OnValidate()
    {
        Start();
    }

    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    public static GameMap Current = null;

    private static GameMap CreateMap(int id)
    {
        var prefab = ResourceEx.Load<GameMap>("Maps/", id);
        var res = prefab != null ? prefab.Clone<GameMap>() : null;
        if (res != null) res.Id = id;
        return res;
    }

    public static GameMap Load(int id)
    {
        if (Current != null)
        {
            Current.gameObject.SetActive(false);
            Destroy(Current.gameObject);
        }
        return Current = CreateMap(id);
    }
}
