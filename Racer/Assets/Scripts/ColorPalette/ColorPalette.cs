using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ColorPalette")]
public class ColorPalette : ScriptableObject
{
    [System.Serializable]
    public class GameColor
    {
        public int id = 0;
        public Color color = Color.white;
        public Color gloss = Color.white;
    }

    public AnimationCurve glossCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
    public List<GameColor> sharedColors = new List<GameColor>();

    public Color GetColorById(int id)
    {
        var res = sharedColors.Find(x => x.id == id);
        return res != null ? res.color : Color.gray;
    }

    public Color GetGlossById(int id)
    {
        var res = sharedColors.Find(x => x.id == id);
        return res != null ? res.gloss : Color.gray;
    }

    public static Color ComputeGloss(Color color, AnimationCurve glossCurve)
    {
        //  verify gray colors
        {
            var drg = Mathf.Abs(color.r - color.g);
            var dgb = Mathf.Abs(color.g - color.b);
            var drb = Mathf.Abs(color.r - color.b);
            var d = drg + dgb + drb;
            if (d < 0.3f) return Color.black;
        }

        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        float rh = glossCurve.Evaluate(h);
        float rs = 1;
        float rv = 2 * Mathf.Max(0, 0.5f - Mathf.Abs(0.65f - v));
        var res = Color.HSVToRGB(rh, rs, rv);
        res.a = color.a;
        return res;
    }
}
