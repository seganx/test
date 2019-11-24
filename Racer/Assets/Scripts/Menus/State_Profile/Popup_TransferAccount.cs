using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_TransferAccount : GameState
{
    [SerializeField] private InputField keyInput = null;
    [SerializeField] private Button transferButton = null;

    private void Start()
    {
        transferButton.onClick.AddListener(() =>
        {
            if (keyInput.text.Length < 3) return;

            Popup_Loading.Display();
            Network.TransferAccount(keyInput.text, msg =>
            {
                Popup_Loading.Hide();
                if (msg == Network.Message.ok)
                {
                    gameManager.OpenPopup<Popup_Confirm>().Setup(111064, false, true, ok =>
                    {
                        PlayerPrefs.DeleteAll();
                        PlayerPrefsEx.ClearData();
                        Profile.ResetData(1);
                        Application.Quit();
                    });
                }
                else gameManager.OpenPopup<Popup_Confirm>().Setup(111065, false, true, null);
            });
        });

        UiShowHide.ShowAll(transform);
    }
}
