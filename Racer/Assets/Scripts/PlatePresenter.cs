using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatePresenter : Base
{
    private void Start()
    {
        var text = GlobalFactory.CreateRacerPlate(transform);
        //text.SetText(Profile.Name.SubString(0, 11));
    }
}
