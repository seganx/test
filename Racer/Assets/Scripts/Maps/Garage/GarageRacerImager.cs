using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GarageRacerImager : MonoBehaviour
{
    [SerializeField] private Camera photoCamera = null;
    [SerializeField] private Transform cameras = null;

    private RacerPresenter racer = null;

    public Texture2D TakeAShot(int racerId, RacerCustomData custom, bool hideShadow, int width, int height, int cameraId)
    {
        racer = RacerFactory.Racer.Create(racerId, transform);
        racer.SetupCustom(custom).SetupCameras(false).SetTransparent(false);

        foreach (var wheel in racer.frontWheels)
            wheel.transform.localEulerAngles = Vector3.up * 30;

        if (hideShadow)
            DestroyImmediate(racer.transform.Find("Shadow").gameObject);

        float currBloomSpecular = Shader.GetGlobalFloat("bloomSpecular");
        float currSkyBloom = Shader.GetGlobalFloat("skyBloom");
        Shader.SetGlobalFloat("bloomSpecular", 1);
        Shader.SetGlobalFloat("skyBloom", 0);

        var desc = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 32);
        desc.autoGenerateMips = false;
        var rt = RenderTexture.GetTemporary(desc);

        photoCamera.transform.CopyValuesFrom(cameras.GetChild(cameraId));
        photoCamera.targetTexture = rt;
        photoCamera.Render();
        photoCamera.targetTexture = null;

        Shader.SetGlobalFloat("bloomSpecular", currBloomSpecular);
        Shader.SetGlobalFloat("skyBloom", currSkyBloom);

        RenderTexture.active = rt;
        var screenShot = new Texture2D(desc.width, desc.height, TextureFormat.ARGB32, false);
        screenShot.ReadPixels(new Rect(0, 0, desc.width, desc.height), 0, 0);
        RenderTexture.active = null;

        var pixels = screenShot.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
            if (pixels[i].a > 0)
                pixels[i].a = 255;
        screenShot.SetPixels32(pixels);
        screenShot.Apply();

        return screenShot;
    }


    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    [System.Serializable]
    private class ImageInfo
    {
        public static int version = 1;

        public int ver = version;
        public int width = 0;
        public int height = 0;
    }

    private const string tag_opaque = "_opaque";
    private const string tag_transparent = "_transparent";

    private static string BasePath
    {
        get
        {
            var res = Application.persistentDataPath + "/images";
            if (Directory.Exists(res) == false) Directory.CreateDirectory(res);
            res += "/racers";
            if (Directory.Exists(res) == false) Directory.CreateDirectory(res);
            return res + "/";
        }
    }

    private static string GetPath(int id, string tag)
    {
        return BasePath + id + tag;
    }

    private static string GetInfoPath(string tag, int id, int width, int height)
    {
        return GetPath(id, tag) + "_" + width + "x" + height + ".info";
    }

    private static string GetDataPath(string tag, int id, int width, int height)
    {
        return GetPath(id, tag) + "_" + width + "x" + height + ".data";
    }

    private static Texture2D LoadFromFile(int id, string tag, int width, int height)
    {
        var infoPath = GetInfoPath(tag, id, width, height);
        var dataPath = GetDataPath(tag, id, width, height);
        if (File.Exists(infoPath) == false || File.Exists(dataPath) == false) return null;
        var info = JsonUtility.FromJson<ImageInfo>(File.ReadAllText(infoPath));
        if (info.ver != ImageInfo.version) return null;
        var res = new Texture2D(info.width, info.height, TextureFormat.ARGB32, false);
        res.LoadRawTextureData(File.ReadAllBytes(dataPath));
        res.Apply();
        return res;
    }

    private static void SaveToFile(int id, string tag, Texture2D texture)
    {
        File.WriteAllText(GetInfoPath(tag, id, texture.width, texture.height), JsonUtility.ToJson(new ImageInfo() { width = texture.width, height = texture.height }));
        File.WriteAllBytes(GetDataPath(tag, id, texture.width, texture.height), texture.GetRawTextureData());
    }

    private static void RemoveFiles(int id, string tag)
    {
        var search = GetPath(id, tag);
        var files = Directory.GetFiles(BasePath);
        foreach (var item in files)
            if (item.Contains(search))
                File.Delete(item);
    }

    private static Sprite GetSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

    private static Texture2D CreateAndSaveOpaque(int racerId, RacerCustomData custom, int width, int height)
    {
        var imager = Resources.Load<GarageRacerImager>("Prefabs/RacerImager" + tag_opaque).Clone<GarageRacerImager>();
        imager.transform.position = Vector3.up * 500;
        var res = imager.TakeAShot(racerId, custom, false, width, height, 0);
        SaveToFile(racerId, tag_opaque, res);
        DestroyImmediate(imager.gameObject);
        return res;
    }

    public static Sprite GetImageOpaque(int racerId, RacerCustomData custom, float width, float height)
    {
        int w = Mathf.RoundToInt(width * 2);
        int h = Mathf.RoundToInt(height * 2);
        var tex = LoadFromFile(racerId, tag_opaque, w, h);
        return GetSprite(tex != null ? tex : CreateAndSaveOpaque(racerId, custom, w, h));
    }

    public static void RemoveImageOpaque(int racerId)
    {
        RemoveFiles(racerId, tag_opaque);
    }

    private static Texture2D CreateAndSaveTransparent(int racerId, RacerCustomData custom, int width, int height)
    {
        var imager = Resources.Load<GarageRacerImager>("Prefabs/RacerImager" + tag_transparent).Clone<GarageRacerImager>();
        imager.transform.position = Vector3.up * 500;
        var res = imager.TakeAShot(racerId, custom, true, width, height, 0);
        SaveToFile(racerId, tag_opaque, res);
        DestroyImmediate(imager.gameObject);
        return res;
    }

    public static Sprite GetImageTransparent(int racerId, RacerCustomData custom, float width, float height)
    {
        int w = Mathf.RoundToInt(width * 2);
        int h = Mathf.RoundToInt(height * 2);
        var tex = LoadFromFile(racerId, tag_opaque, w, h);
        return GetSprite(tex != null ? tex : CreateAndSaveTransparent(racerId, custom, w, h));
    }

    public static void RemoveImageTransparent(int racerId)
    {
        RemoveFiles(racerId, tag_transparent);
    }

    public static Sprite GetImageTransparentTemporary(int racerId, RacerCustomData custom, float width, float height)
    {
        var imager = Resources.Load<GarageRacerImager>("Prefabs/RacerImager" + tag_transparent).Clone<GarageRacerImager>();
        imager.transform.position = Vector3.up * 500;
        var res = imager.TakeAShot(racerId, custom, true, Mathf.RoundToInt(width * 2), Mathf.RoundToInt(height * 2), 0);
        DestroyImmediate(imager.gameObject);
        return GetSprite(res);
    }
}
