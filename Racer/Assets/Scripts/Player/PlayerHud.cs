using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriter = null;
    [SerializeField] private Sprite[] positionSprites = null;
    private PlayerPresenter player = null;
    private int currPosition = -1;
    private Color color = Color.white;

    // Use this for initialization
    private void Start()
    {
        player = GetComponentInParent<PlayerPresenter>();
    }

    // Update is called once per frame
    private void Update()
    {
        var pos = transform.localPosition;
        pos.x = player.racer.transform.localPosition.x;
        pos.y = player.racer.bluePrint.roof.transform.position.y + 1.0f;
        transform.localPosition = pos;

        if (currPosition != player.player.CurrPosition)
        {
            currPosition = player.player.CurrPosition;
            spriter.sprite = positionSprites[currPosition % positionSprites.Length];
        }

        color.a = Mathf.Clamp01((transform.position.z - Camera.main.transform.position.z - 10) / 20.0f);
        spriter.color = color;
    }
}
