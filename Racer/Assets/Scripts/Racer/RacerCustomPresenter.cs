using SeganX;
using UnityEngine;

public class RacerCustomPresenter : MonoBehaviour
{
    [SpritePreview]
    public Sprite icon = null;
    public bool colorable = true;

    private int id = -1;
    public int Id { get { return id >= 0 ? id : id = name.Split('_')[0].ToInt(-1); } }


    public virtual RacerCustomPresenter SetMaterialModel(RacerMaterial.BaseParam baseParam, int model, int matId = 1)
    {
        var rendr = GetComponent<Renderer>();
        if (rendr != null) RacerFactory.Colors.SetModel(model, rendr.material, baseParam, matId);

        var rendrs = GetComponentsInChildren<Renderer>(true);
        foreach (var item in rendrs)
            RacerFactory.Colors.SetModel(model, item.material, baseParam, matId);

        return this;
    }

    public virtual RacerCustomPresenter SetColor(int id, int matId = 1)
    {
        var rendr = GetComponent<Renderer>();
        if (rendr != null) RacerFactory.Colors.SetDiffuseColor(id, rendr.material, matId, false);

        var rendrs = GetComponentsInChildren<Renderer>(true);
        foreach (var item in rendrs)
            RacerFactory.Colors.SetDiffuseColor(id, item.material, matId, false);

        return this;
    }


}
