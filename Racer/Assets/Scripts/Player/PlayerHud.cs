using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(200)]
public class PlayerHud : MonoBehaviour
{
    [SerializeField] private CanvasGroup box = null;
    [SerializeField] private Image numberImage = null;
    [SerializeField] private LocalText nameLabel = null;
    [SerializeField] private FlyingNumber nitrosLabel = null;
    [SerializeField] private Sprite[] positionSprites = null;

    private PlayerPresenter player = null;
    private Vector3 nitrosLabelBasePos = Vector3.zero;

    private IEnumerator Start()
    {
        player = GetComponentInParent<PlayerPresenter>();

        // send box on the racer
        if (DisplayBox)
        {
            box.transform.SetParent(player.racer.transform);
            var pos = box.transform.localPosition;
            pos.y = player.racer.bluePrint.roof.transform.position.y + 0.5f;
            box.transform.localPosition = pos;
        }
        else box.gameObject.SetActive(false);

        // send nitros label to the back
        {
            nitrosLabel.transform.SetParent(player.racer.transform);
            nitrosLabelBasePos = player.racer.bluePrint.spoiler.transform.position + Vector3.back * 0.2f + Vector3.down * 0.1f;
        }

        if (player.player.IsPlayer == false)
        {
            nameLabel.SetText(player.player.name);
            var wait = new WaitForSeconds(0.1f);
            while (true)
            {
                numberImage.sprite = positionSprites[player.player.CurrRank % positionSprites.Length];
                var depth = transform.position.z - Camera.main.transform.position.z;
                float fadeFrom = 3;
                float fadeTo = 20;
                float fadeDis = (fadeTo - fadeFrom) * 0.5f;
                box.alpha = Mathf.Clamp01(1 - Mathf.Pow((depth - fadeFrom - 1) / fadeDis - 1, 2));
                box.transform.localScale = Vector3.one * Mathf.Lerp(0.002f, 0.015f, Mathf.Clamp01(depth / 50.0f));
                yield return wait;
            }
        }
        else
        {
            instance = this;
            box.gameObject.SetActive(false);
        }
    }


    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    private static PlayerHud instance = null;

    public static bool DisplayBox
    {
        get { return PlayerPrefs.GetInt("PlayerHud.DisplayBox", 1) > 0; }
        set { PlayerPrefs.SetInt("PlayerHud.DisplayBox", value ? 1 : 0); }
    }

    public static void DisplaySideNitros(float value, bool sideLeft)
    {
        if (instance == null) return;
        instance.nitrosLabelBasePos.x = instance.player.racer.Size.x * (sideLeft ? -0.5f : 0.5f);
        instance.nitrosLabel.Play("%" + value.ToString("0").Persian(), instance.nitrosLabelBasePos, instance.nitrosLabelBasePos + Vector3.up * 0.5f, 1, 0.5f);
    }
}
