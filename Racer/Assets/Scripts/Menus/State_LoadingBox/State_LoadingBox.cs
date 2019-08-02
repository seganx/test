using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_LoadingBox : GameState
{
    private void Start()
    {
        UiHeader.Show();
        UiShowHide.ShowAll(transform);
    }
}
