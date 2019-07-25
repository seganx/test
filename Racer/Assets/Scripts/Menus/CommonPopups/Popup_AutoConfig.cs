using SeganX;
using SeganX.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_AutoConfig : GameState
{
    // Use this for initialization
    private IEnumerator Start()
    {
        UiShowHide.ShowAll(transform);
        yield return new WaitWhile(() => CameraFX_Optimizer.IsRunning);
        base.Back();
    }

    public override void Back()
    {
        //base.Back();
    }
}
