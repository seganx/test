using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageRacerImager : MonoBehaviour
{
    [SerializeField] private Camera photoCamera = null;

    private RacerPresenter racer = null;

    public Texture2D TakeAShot(int racerId, RacerCustomData custom)
    {
        racer = RacerFactory.Racer.Create(racerId, transform);
        racer.SetupCustom(custom).SetupCameras(false).SetTransparent(false);
        DestroyImmediate(racer.transform.Find("Shadow").gameObject);

        float currBloomSpecular = Shader.GetGlobalFloat("bloomSpecular");
        float currSkyBloom = Shader.GetGlobalFloat("skyBloom");
        Shader.SetGlobalFloat("bloomSpecular", 1);
        Shader.SetGlobalFloat("skyBloom", 0);

        var desc = new RenderTextureDescriptor(512, 256, RenderTextureFormat.ARGB32, 32);
        desc.autoGenerateMips = false;
        var rt = RenderTexture.GetTemporary(desc);

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

    private static void SaveToFile(int id, Texture2D texture)
    {
        var directory = Application.persistentDataPath + "/images";
        if (System.IO.Directory.Exists(directory) == false) System.IO.Directory.CreateDirectory(directory);
        directory += "/racers";
        if (System.IO.Directory.Exists(directory) == false) System.IO.Directory.CreateDirectory(directory);

        var filename = directory + "/" + id + ".png";
        var bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, bytes);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.RevealInFinder(System.IO.Path.GetDirectoryName(filename));
#endif
    }

    public static Sprite GetRacerImage(int racerId, RacerCustomData custom)
    {
        var imager = Resources.Load<GarageRacerImager>("Prefabs/GarageRacerImager").Clone<GarageRacerImager>();
        imager.transform.position = Vector3.up * 500;
        var restexture = imager.TakeAShot(racerId, custom);
        //SaveToFile(Profile.CurrentRacer.id, restexture);
        DestroyImmediate(imager.gameObject);
        return Sprite.Create(restexture, new Rect(0, 0, restexture.width, restexture.height), Vector2.one * 0.5f);
    }
}
