using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLoadingBoxFreePackage : MonoBehaviour
{
    [SerializeField] private LocalText desc = null;
    [SerializeField] private Button purchaseButton = null;

    private void Start()
    {
        desc.SetFormatedText(GlobalConfig.Shop.freePackage.nextTime / 3600);
        purchaseButton.onClick.AddListener(() =>
        {
            switch (Random.Range(0, 100) % 4)
            {
                case 0:
                    {
                        var gems = GlobalConfig.Shop.freePackage.gemValues.RandomOne();
                        Profile.EarnResouce(gems, 0);
                        Popup_Rewards.AddResource(gems, 0);
                    }
                    break;

                case 1:
                    {
                        var coins = GlobalConfig.Shop.freePackage.coinValues.RandomOne();
                        Profile.EarnResouce(0, coins);
                        Popup_Rewards.AddResource(0, coins);
                    }
                    break;

                case 2:
                    {
                        var racerid = RewardLogic.SelectRacerReward();
                        Profile.AddRacerCard(racerid, 1);
                        Popup_Rewards.AddRacerCard(racerid, 1);
                    }
                    break;

                case 3:
                    {
                        var reward = RewardLogic.GetCustomReward();
                        Profile.AddRacerCustome(reward.type, reward.racerId, reward.customId);
                        Popup_Rewards.AddCustomeCard(reward.type, reward.racerId, reward.customId);
                    }
                    break;
            }

            Popup_Rewards.Display();
        });
    }
}
