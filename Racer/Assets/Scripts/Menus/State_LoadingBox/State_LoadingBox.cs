using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_LoadingBox : GameState
{
    private void Start()
    {
        GarageCamera.SetCameraId(1);
        UiHeader.Show();
        UiShowHide.ShowAll(transform);

        PopupQueue.Add(.5f, () => Popup_Tutorial.Display(51));
    }
}
