using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_OtherUserAccount : GameState
{
    [SerializeField] private Text nameLabel = null;
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private Image leagueIcon = null;
    [SerializeField] private Image leagueImage = null;
    [SerializeField] private Text racerNameLabel = null;
    [SerializeField] private LocalText racerPowerLabel = null;
    [SerializeField] private Button nextButton = null;
    [SerializeField] private Button prevButton = null;

    private string playerName = string.Empty;
    private int currentRaceIndex = 0;
    private List<RacerProfile> racers = null;

    public State_OtherUserAccount Setup(ProfileData.NetData netdata, string nickname, int score, int position)
    {
        playerName = nickname;
        nameLabel.SetText(nickname);
        scoreLabel.SetText(score.ToString("#,0"));
        var leagueIndex = GlobalConfig.Leagues.GetIndex(score, position);
        leagueImage.sprite = GlobalFactory.League.GetBigIcon(leagueIndex);
        leagueIcon.sprite = GlobalFactory.League.GetSmallIcon(leagueIndex);

        var profile = new ProfileData() { data = netdata };
        racers = profile.racers.FindAll(x => x.cards >= RacerFactory.Racer.GetConfig(x.id).CardCount);
        racers.Sort((x, y) => x.id - y.id);
        currentRaceIndex = racers.FindIndex(x => x.id == profile.selectedRacer);
        return this;
    }

    private void Start()
    {
        UiHeader.Hide();
        UiShowHide.ShowAll(transform);

        PopupQueue.Add(.5f, () => Popup_Tutorial.Display(35));

        GarageCamera.SetCameraId(0);
        DisplayRacer();

        nextButton.onClick.AddListener(() =>
        {
            currentRaceIndex++;
            DisplayRacer();
        });

        prevButton.onClick.AddListener(() =>
        {
            currentRaceIndex--;
            DisplayRacer();
        });
    }

    private void DisplayRacer()
    {
        prevButton.gameObject.SetActive(currentRaceIndex > 0);
        nextButton.gameObject.SetActive(currentRaceIndex < racers.Count - 1);
        if (racers.Count < 1) return;

        currentRaceIndex = Mathf.Clamp(currentRaceIndex, 0, racers.Count - 1);
        var racerprofile = racers[currentRaceIndex];
        var config = RacerFactory.Racer.GetConfig(racerprofile.id);
        racerNameLabel.text = config.Name;
        racerPowerLabel.SetText(config.ComputePower(racerprofile.level.SpeedLevel, racerprofile.level.NitroLevel, racerprofile.level.SteeringLevel, racerprofile.level.BodyLevel).ToString("#,0"));
        GarageRacer.LoadRacer(racerprofile);
        if (GarageRacer.racer != null)
            GarageRacer.racer.BroadcastMessage("SetPlateText", playerName, SendMessageOptions.DontRequireReceiver);
    }

    public override float PreClose()
    {
        GarageRacer.LoadRacer(0);
        return base.PreClose();
    }
}
