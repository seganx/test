using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiShopSocialPackage : MonoBehaviour
{
    [SerializeField] private LocalText gemLabel = null;
    [SerializeField] private Button purchaseButton = null;

    private void Start()
    {
        gemLabel.gameObject.SetActive(Profile.JoinedInstagram == false);
        gemLabel.SetText(GlobalConfig.Shop.instaToGem.ToString("#,0"));

        purchaseButton.onClick.AddListener(() =>
        {
            Application.OpenURL(GlobalConfig.Socials.instagramUrl);
            if (Profile.JoinedInstagram == false)
            {
                Profile.JoinedInstagram = true;
                Profile.EarnResouce(GlobalConfig.Shop.instaToGem, 0);
                gemLabel.gameObject.SetActive(false);
            }
        });
    }

}
