using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLoadingBoxFreePackage : MonoBehaviour
{
    [SerializeField] private int index = 0;
    [SerializeField] private LocalText desc = null;
    [SerializeField] private LocalText remainedLabel = null;
    [SerializeField] private Button purchaseButton = null;

    private GlobalConfig.Data.Shop.LoadingBox data = null;

    private bool IsSameDay
    {
        get { return PlayerPrefsEx.GetInt(name + ".day", 0) == TimerManager.ServerTime.DayOfYear; }
        set { if (value) PlayerPrefsEx.SetInt(name + ".day", TimerManager.ServerTime.DayOfYear); }
    }

    private int UseCount
    {
        get { return PlayerPrefsEx.GetInt(name + ".used", 0); }
        set { PlayerPrefsEx.SetInt(name + ".used", value); }
    }

    private void Start()
    {
        data = index < GlobalConfig.Shop.loadingBoxPackage.Count ? GlobalConfig.Shop.loadingBoxPackage[index] : null;
        if (data != null)
        {
            if (IsSameDay == false)
            {
                IsSameDay = true;
                UseCount = 0;
            }

            UpdateVisual();
        }
        else Destroy(gameObject);

        purchaseButton.onClick.AddListener(() =>
        {
            UseCount++;
            UpdateVisual();

            switch (Random.Range(0, 100) % 4)
            {
                case 0:
                    {
                        var gems = data.gemValues.RandomOne();
                        Profile.EarnResouce(gems, 0);
                        Popup_Rewards.AddResource(gems, 0);
                    }
                    break;

                case 1:
                    {
                        var coins = data.coinValues.RandomOne();
                        Profile.EarnResouce(0, coins);
                        Popup_Rewards.AddResource(0, coins);
                    }
                    break;

                case 2:
                    {
                        var list = RacerFactory.Racer.AllConfigs.FindAll(x => x.GroupId.Between(data.cardsGroups.x, data.cardsGroups.y));
                        var racerid = list.Count > 0 ? list.RandomOne().Id : RewardLogic.SelectRacerReward();
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

    private void UpdateVisual()
    {
        desc.SetFormatedText(data.nextTime > 3600 ? data.nextTime / 3600 : data.nextTime / 60);
        purchaseButton.SetInteractable(UseCount < data.dailyCount);

        if (remainedLabel)
        {
            var ramained = Mathf.Max(0, data.dailyCount - UseCount);
            remainedLabel.SetFormatedText(ramained, data.dailyCount);
        }
    }
}
