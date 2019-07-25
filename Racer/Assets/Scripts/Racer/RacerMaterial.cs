using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RacerMaterial
{
    public class BaseParam
    {
        public string shader = null;
        public float reflection = 0;
        public float specularValue = 0;
        public float specularPower = 0;
        public float metalPower = 0;
    }

    private const string Reflection = "_Reflection";
    private const string SpecularValue = "_SpecularAtten";
    private const string SpecularPower = "_SpecularPower";
    private const string MetalPower = "_MetalPower";

    public static BaseParam CreateBaseParam(Material src, int matId)
    {
        var res = new BaseParam();
        res.shader = src.shader.name;
        res.reflection = src.GetFloat(Reflection + matId);
        res.specularValue = src.GetFloat(SpecularValue + matId);
        res.specularPower = src.GetFloat(SpecularPower + matId);
        res.metalPower = src.GetFloat(MetalPower + matId);
        return res;
    }

    public static void SetMaterialModel(Material dest, BaseParam baseParam, ColorModel model, int matId = 1)
    {
        if (dest.shader.name != baseParam.shader) return;
        dest.SetFloat(Reflection + matId, baseParam.reflection * model.reflection);
        dest.SetFloat(SpecularValue + matId, baseParam.specularValue * model.SpecularValue);
        dest.SetFloat(SpecularPower + matId, baseParam.specularPower * model.specularPower);
        dest.SetFloat(MetalPower + matId, baseParam.metalPower * model.metalPower);
    }

    public static void SetDiffuseColor(Material dest, int matId, Color color, bool alpha)
    {
        string propname = "_DiffColor" + matId;
        if (dest == null || dest.HasProperty(propname) == false) return;
        var destcolor = dest.GetColor(propname);
        destcolor.r = color.r;
        destcolor.g = color.g;
        destcolor.b = color.b;
        if (alpha) destcolor.a = color.a;
        dest.SetColor(propname, destcolor);
    }

    public static void SetVinylTexture(Material dest, Texture texture)
    {
        if (dest.HasProperty("_VinylTex"))
            dest.SetTexture("_VinylTex", texture);
    }

    public static void SetVinylColor(Material dest, Color color)
    {
        if (dest.HasProperty("_VinylColor"))
            dest.SetColor("_VinylColor", color);
    }
}
