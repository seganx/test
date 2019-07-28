using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatePresenter : Base
{
    private TextMesh text = null;

    private void Start()
    {
        text = new GameObject("TextMesh").AddComponent<TextMesh>();
        text.transform.SetParent(transform, false);
        text.transform.localPosition = Vector3.zero;
        text.transform.localRotation = Quaternion.identity;
        text.transform.localScale = Vector3.one * 0.018f;

        text.font = GlobalFactory.Fonts.Get(2);
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.characterSize = 1;
        text.fontSize = 35;
        text.richText = false;
        text.color = Color.black;
        text.SetText(Profile.Name);
    }
}
