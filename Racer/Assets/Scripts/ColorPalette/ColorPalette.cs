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
    }

    public List<GameColor> sharedColors = new List<GameColor>();

    public Color GetColorById(int id)
    {
        var res = sharedColors.Find(x => x.id == id);
        return res != null ? res.color : Color.gray;
    }
}
