using SeganX;
using SeganX.Network;
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
    [SerializeField] private UiShowHide socialPanel = null;
    [SerializeField] private Button racerLikeButton = null;
    [SerializeField] private Button racerUnlikeButton = null;
    [SerializeField] private LocalText racerLikesLabel = null;
    [SerializeField] private LocalText dailyReviewLabel = null;
    [SerializeField] private GameObject likeTutorialGameObject = null;

    private string playerName = string.Empty;
    private int currentRaceIndex = 0;
    private List<RacerProfile> racers = null;
    private PlayerInfoResponse data = null;

    private int CurrentRacerId { get { return GarageRacer.racer == null ? 0 : GarageRacer.racer.Id; } }

    public State_OtherUserAccount Setup(PlayerInfoResponse netdata, string nickname, int score, int position)
    {
        data = netdata;
        playerName = nickname;
        nameLabel.SetText(nickname);
        scoreLabel.SetText(score.ToString("#,0"));
        dailyReviewLabel.SetText(netdata.dailyProfileView.ToString("#,0"));
        var leagueIndex = GlobalConfig.Leagues.GetIndex(score, position);
        leagueImage.sprite = GlobalFactory.League.GetBigIcon(leagueIndex);
        leagueIcon.sprite = GlobalFactory.League.GetSmallIcon(leagueIndex);

        var profile = new ProfileData() { data = netdata.netData };
        racers = profile.racers.FindAll(x => x.cards >= RacerFactory.Racer.GetConfig(x.id).CardCount);
        racers.Sort((x, y) => x.id - y.id);
        currentRaceIndex = racers.FindIndex(x => x.id == profile.selectedRacer);

        racerLikeButton.onClick.AddListener(OnLikeClicked);
        racerUnlikeButton.onClick.AddListener(OnLikeClicked);
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

        UpdateSocialPanel();
    }

    private void UpdateSocialPanel()
    {
        likeTutorialGameObject.SetActive(!LikedOnce);
        if (GarageRacer.racer != null)
        {
            var likes = data.racerLikes.Find(x => x.racerId == CurrentRacerId);
            racerLikesLabel.SetText(likes == null ? "0" : likes.count.ToString("#,0"));

            bool IsLiked = SocialLogic.IsLiked(data.profileId, CurrentRacerId);
            racerLikeButton.gameObject.SetActive(IsLiked == false);
            racerUnlikeButton.gameObject.SetActive(IsLiked == true);
            socialPanel.Show();
        }
        else socialPanel.Hide();
    }

    public override float PreClose()
    {
        GarageRacer.LoadRacer(0);
        return base.PreClose();
    }

    private void OnLikeClicked()
    {
        if (CurrentRacerId < 1) return;
        Network.Like(data.profileId, CurrentRacerId, done =>
        {
            if (done)
            {
                var likedata = data.racerLikes.Find(x => x.racerId == CurrentRacerId);
                if (SocialLogic.Action(data.profileId, CurrentRacerId))
                {
                    if (likedata != null)
                        likedata.count++;
                    else
                        data.racerLikes.Add(new RacerLike() { racerId = CurrentRacerId, count = 1 });
                }
                else
                {
                    if (likedata != null && likedata.count > 0)
                        likedata.count--;
                    else
                        data.racerLikes.RemoveAll(x => x.racerId == CurrentRacerId);
                }
                LikedOnce = true;
                UpdateSocialPanel();
            }
        });
    }

    static string likesOnceString = "LikedOnce";
    static bool LikedOnce
    {
        get { return PlayerPrefs.GetInt(likesOnceString, 0) == 1; }
        set { PlayerPrefs.SetInt(likesOnceString, value ? 1 : 0); }
    }
}
