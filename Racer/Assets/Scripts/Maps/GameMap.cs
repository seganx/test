using SeganX;
using SeganX.Effects;
using UnityEngine;
using UnityEngine.Rendering;

public class GameMap : MonoBehaviour
{
    [Header("Environment")]
    public Light sunSource = null;

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

    private GameMap Setup(int id, int skyId)
    {
        Id = id;
        var sky = GetSky(id * 10 + skyId);
        RaceModel.specs.nightMode = sky.isNight;
        sky.Perform(this);
        return this;
    }

    private void Start()
    {
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

    private static GameMap CreateMap(int id, int skyId)
    {
        var prefab = ResourceEx.Load<GameMap>("Maps/", id);
        return prefab == null ? null : prefab.Clone<GameMap>().Setup(id, skyId);        
    }

    public static GameMap Load(int id, int skyId)
    {
        if (Current != null)
        {
            Current.gameObject.SetActive(false);
            Destroy(Current.gameObject);
        }
        return Current = CreateMap(id, skyId);
    }

    private static MapSky GetSky(int id)
    {
        return ResourceEx.Load<MapSky>("Maps/", id);
    }
}
