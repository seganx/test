using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiInventoryCardItem : MonoBehaviour
{
    [SerializeField] private Image racerImage = null;
    [SerializeField] private LocalText countLabel = null;
    [SerializeField] private LocalText groupLabel = null;
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private Button sellButton = null;

    public UiInventoryCardItem Setup(RacerConfig config, int count)
    {
        int eachPrice = Mathf.CeilToInt(config.CardCount / GlobalConfig.Shop.inventorySellFactor);
        int price = eachPrice * count;

        TotalCount += count;
        TotalPrice += price;

        racerImage.sprite = config.halfIcon;
        countLabel.SetFormatedText(count);
        groupLabel.SetText(config.GroupId.ToString());
        nameLabel.text = config.Name;
        priceLabel.SetText(price.ToString("#,0"));

        sellButton.onClick.AddListener(() =>
        {
            sellButton.SetInteractable(false);
            Profile.AddRacerCard(config.Id, -count);
            Profile.EarnResouce(price, 0);
            TotalCount -= count;
            TotalPrice -= price;
            Destroy(gameObject);
        });

        return this;
    }

    public void Sell()
    {
        sellButton.onClick.Invoke();
    }

    ////////////////////////////////////////////////////////
    /// STATIC MEMBER
    ////////////////////////////////////////////////////////
    public static int TotalCount { get; set; }
    public static int TotalPrice { get; set; }
}
