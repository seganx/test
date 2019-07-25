using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSequentialColor : MonoBehaviour
{
    [SerializeField] private float interval = 1;
    [SerializeField] private Transform objects = null;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color highlightColor = Color.white;

    private MaskableGraphic[] graphics = null;

    private IEnumerator Start()
    {
        graphics = objects.GetComponentsInChildren<MaskableGraphic>();

        var waitTime = new WaitForSeconds(interval);
        int index = 0;
        while (true)
        {
            if (graphics.HasItem())
            {
                for (int i = 0; i < graphics.Length; i++)
                    graphics[i].color = index == i ? highlightColor : defaultColor;
                index = (index + 1) % graphics.Length;
            }
            else
            {
                objects.SetActiveChild(index);
                index = (index + 1) % objects.childCount;
            }
            yield return waitTime;
        }
    }

}
