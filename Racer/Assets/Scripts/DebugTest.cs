using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeganX;
using SeganX.Effects;

public class DebugTest : MonoBehaviour
{
    public void OnCameraFx()
    {
        CameraFX.Activated = !CameraFX.Activated;
    }

    public void OnBloom()
    {
        CameraFX.Bloom = !CameraFX.Bloom;
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.End))
        {
            StartCoroutine(TakeScreenShot());
        }
    }

    public IEnumerator TakeScreenShot()
    {
        yield return new WaitForEndOfFrame();
        var filename = System.IO.Path.Combine(Application.dataPath + "/../../Documents/Screenshots", "gameplay_" + System.DateTime.Now.Ticks + ".png");
        ScreenCapture.CaptureScreenshot(filename, 2);
    }
#endif
}
