using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatePresenter : Base
{
    private TextMesh caption = null;

    private void Awake()
    {
        caption = GlobalFactory.CreateRacerPlate(transform);
    }

    public void SetPlateText(string text)
    {
        caption.SetText(text.SubString(0, 11));
    }
}
