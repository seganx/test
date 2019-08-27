using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageRacerImager : MonoBehaviour
{

    public GarageRacerImager TakeAShot()
    {
        var racer = GameObject.FindObjectOfType<RacerPresenter>();
        racer.SendMessage("Awake");
        racer.SetupCustom(RacerFactory.Racer.GetConfig(racer.name.Split('_')[0].ToInt()).DefaultRacerCustom);
        GameObject.DestroyImmediate(racer.transform.Find("Shadow").gameObject);

        float currBloomSpecular = Shader.GetGlobalFloat("bloomSpecular");
        float currSkyBloom = Shader.GetGlobalFloat("skyBloom");

        Shader.SetGlobalFloat("bloomSpecular", 1);
        Shader.SetGlobalFloat("skyBloom", 0);

        var desc = new RenderTextureDescriptor(1024, 512, RenderTextureFormat.ARGB32, 32);
        desc.autoGenerateMips = false;
        var rt = RenderTexture.GetTemporary(desc);

        Camera.main.targetTexture = rt;
        Camera.main.Render();
        Camera.main.targetTexture = null;

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

        return this;
    }
}
