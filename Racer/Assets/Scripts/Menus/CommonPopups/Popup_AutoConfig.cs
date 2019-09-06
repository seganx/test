using SeganX;
using SeganX.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_AutoConfig : GameState
{
    [SerializeField] private Text infoLabel = null;

    // Use this for initialization
    private IEnumerator Start()
    {
        UiShowHide.ShowAll(transform);

        while (CameraFX_Optimizer.IsRunning)
        {
            infoLabel.text = "Step: " + CameraFX_Optimizer.Step + " FPS:" + CameraFX_Optimizer.Fps + " Resolution:" + CameraFX.Resolution + " Quality :" + CameraFX.Quality + " Bloom :" + CameraFX.Bloom;
            yield return new WaitForSeconds(0.5f);
        }

        base.Back();
    }

    public override void Back()
    {
        //base.Back();
    }
}
