using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingNumber : MonoBehaviour
{
    private TextMesh text = null;
    private float timer = 0;
    private Vector3 from = Vector3.zero;
    private Vector3 to = Vector3.zero;
    private float duration = 2;
    private float fadeDuration = 1;

    private void Awake()
    {
        text = transform.GetComponent<TextMesh>(true, true);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        if (t > 1)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.localPosition = Vector3.Lerp(from, to, t);

        float dt = duration - timer;
        float ft = dt / fadeDuration;
        if (ft < 1)
        {
            var c = text.color;
            c.a = Mathf.Lerp(0, 1, ft);
            text.color = c;
        }
    }

    public FlyingNumber Play(string text, Vector3 from, Vector3 to, float duration, float fadeDuration)
    {
        this.text.text = text;
        this.from = from;
        this.to = to;
        this.duration = duration;
        this.fadeDuration = fadeDuration;

        timer = 0;
        var c = this.text.color;
        c.a = 1;
        this.text.color = c;

        gameObject.SetActive(true);
        return this;
    }

}
