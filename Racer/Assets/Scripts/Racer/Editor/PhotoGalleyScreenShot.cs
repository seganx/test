using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class PhotoGalleyScreenShot
{
    [MenuItem("SeganX/Take Screenshot")]
    public static void TakeScreenShot()
    {
        var desc = new RenderTextureDescriptor(1024, 512, RenderTextureFormat.ARGB32, 32);
        desc.autoGenerateMips = false;
        var rt = RenderTexture.GetTemporary(desc);

        Camera.main.targetTexture = rt;
        Camera.main.Render();
        Camera.main.targetTexture = null;

        RenderTexture.active = rt;
        var screenShot = new Texture2D(desc.width, desc.height, TextureFormat.ARGB32, false);
        screenShot.ReadPixels(new Rect(0, 0, desc.width, desc.height), 0, 0);
        RenderTexture.active = null;

        SaveToFile(screenShot);
    }


    [MenuItem("SeganX/Take Racer Screenshot")]
    public static void TakeRacerScreenShot()
    {
        var racer = GameObject.FindObjectOfType<RacerPresenter>();
        racer.SendMessage("Awake");
        racer.SetupCustom(RacerFactory.Racer.GetConfig(racer.name.Split('_')[0].ToInt()).DefaultRacerCustom);
        GameObject.DestroyImmediate(racer.transform.Find("Shadow").gameObject);

        Shader.SetGlobalFloat("bloomSpecular", 1);
        Shader.SetGlobalFloat("skyBloom", 0);

        var desc = new RenderTextureDescriptor(1024, 512, RenderTextureFormat.ARGB32, 32);
        desc.autoGenerateMips = false;
        var rt = RenderTexture.GetTemporary(desc);

        Camera.main.targetTexture = rt;
        Camera.main.Render();
        Camera.main.targetTexture = null;

        RenderTexture.active = rt;
        var screenShot = new Texture2D(desc.width, desc.height, TextureFormat.ARGB32, false);
        screenShot.ReadPixels(new Rect(0, 0, desc.width, desc.height), 0, 0);
        RenderTexture.active = null;

        var pixels = screenShot.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
            if (pixels[i].a > 0)
                pixels[i].a = 255;
        screenShot.SetPixels32(pixels);


        SaveToFile(screenShot);
    }

    private static void SaveToFile(Texture2D texture)
    {
        var filename = Application.persistentDataPath +
            "/screenshot" +
            System.DateTime.Now.Year +
            System.DateTime.Now.Month +
            System.DateTime.Now.Day +
            System.DateTime.Now.Hour +
            System.DateTime.Now.Minute +
            System.DateTime.Now.Second +
            ".png";

        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filename, bytes);

        EditorUtility.RevealInFinder(Path.GetDirectoryName(filename));
    }
}
