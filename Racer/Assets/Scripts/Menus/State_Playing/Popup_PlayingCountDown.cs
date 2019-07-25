using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_PlayingCountDown : GameState
{
    public Transform counterPictures = null;

    private void Awake()
    {
        counterPictures.SetActiveChild(-1);
    }

    private IEnumerator Start()
    {
        var waitFor = new WaitForSeconds(1);
        counterPictures.SetActiveChild(3);
        yield return waitFor;
        counterPictures.SetActiveChild(2);
        yield return waitFor;
        counterPictures.SetActiveChild(1);
        yield return waitFor;
        counterPictures.SetActiveChild(0);
        yield return waitFor;
        yield return new WaitForSeconds(.5f);
        base.Back();
    }

    public override void Back()
    {
        //base.Back();
    }
}
