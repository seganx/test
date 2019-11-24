using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Profile : GameState
{
    [SerializeField] private InputField nicknameInput = null;
    [SerializeField] private Button sendNicknamebutton = null;
    [SerializeField] private Text transferKeyLabel = null;
    [SerializeField] private Text userIdLabel = null;
    [SerializeField] private LocalText nicknamePrice = null;
    [SerializeField] private Button transferButton = null;
    [SerializeField] private Button syncButton = null;

    private bool hasNickname = true;

    private bool IsProfileSynced
    {
        get { return ProfileLogic.Synced; }
        set
        {
            transferButton.SetInteractable(value);
            syncButton.SetInteractable(!value);
            syncButton.transform.parent.GetChild(1).gameObject.SetActive(!value);
            syncButton.transform.parent.GetChild(2).gameObject.SetActive(value);
        }
    }

    private IEnumerator Start()
    {
        IsProfileSynced = ProfileLogic.Synced;
        nicknameInput.text = Profile.Name;
        transferKeyLabel.text = Profile.Key;
        userIdLabel.text = Profile.UserId;
        nicknamePrice.SetText(GlobalConfig.Shop.nicknamePrice.ToString());

        hasNickname = Profile.HasName;
        sendNicknamebutton.transform.SetActiveChild(hasNickname ? 1 : 0);

        //transferButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_TransferAccount>());
        transferButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_Confirm>().Setup(111103, false, true, null));

        sendNicknamebutton.onClick.AddListener(() =>
        {
            if (nicknameInput.text.ComputeMD5(Core.Salt) == "1EB663B178CEFE01AF0C8D7FBDE59BBE")
                GlobalConfig.DebugMode = true;
            else if (hasNickname)
                Game.SpendGem(GlobalConfig.Shop.nicknamePrice, OnSendNickname);
            else
                OnSendNickname();
        });

        syncButton.onClick.AddListener(() =>
        {
            Popup_Loading.Display();
            ProfileLogic.SyncWidthServer(true, success =>
            {
                Popup_Loading.Hide();
                IsProfileSynced = ProfileLogic.Synced;
            });
        });

        GarageCamera.SetCameraId(1);
        UiShowHide.ShowAll(transform);

        var waitFor = new WaitForSeconds(0.5f);
        while (true)
        {
            IsProfileSynced = ProfileLogic.Synced;
            yield return waitFor;
        }
    }

    public void OnSendNickname()
    {
        var nickname = nicknameInput.text.Trim().CleanFromCode().CleanForPersian();
        if (nickname.HasContent(3))
        {
            if (nickname.IsLetterOrDigit() && BadWordsFinder.HasBadWord(nickname) == false)
            {
                Popup_Loading.Display();
                Network.SendNickname(nickname, msg =>
                {
                    Popup_Loading.Hide();
                    if (msg == Network.Message.ok)
                    {
                        hasNickname = true;
                        Profile.Name = nickname;
                        sendNicknamebutton.transform.SetActiveChild(1);
                    }
                });
            }
            else gameManager.OpenPopup<Popup_Confirm>().Setup(111121, false, true, null);
        }
    }

    [Console("profile", "sendname")]
    public static void SendNickname()
    {
        if (gameManager.CurrentState is State_Profile)
            gameManager.CurrentState.As<State_Profile>().OnSendNickname();
    }
}
