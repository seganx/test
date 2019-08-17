using Photon;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPresenter : Base
{
    [HideInInspector] public PlayerData player = null;
    [HideInInspector] public RacerPresenter racer = null;

    private float currGrade = 0;
    private float changeGradeSpeed = 1;
    private float currSteeringSpeed = 0;
    private float maxSteeringSpeed = 0;

    public virtual bool IsInactive { get { return false; } }
    public virtual bool IsMine { get { return true; } }
    public float SteeringValue { get; set; }
    public bool PlayingHorn { get; set; }
    public int Grade { get { return player.CurrGrade; } }
    public float Grading { get { return Mathf.Approximately(currGrade, player.CurrGrade) ? 0 : player.CurrGrade - currGrade; } }
    public float Nitros { get { return player.CurrNitrous; } }
    public bool NitrosReady { get { return Nitros >= 1.0f; } }


    protected virtual void OnEnable()
    {
        all.Add(this);
        allPlayers.Add(player);
        UpdatePositons();
    }

    protected virtual void OnDisable()
    {
        player.CurrGrade = -100 * all.Count;
        all.Remove(this);
        UpdatePositons();
        if (all.Count < 2) allPlayers.Clear();
    }

    public virtual float UpdateForwardPosition(float deltaTime)
    {
        var pos = RaceModel.stats.forwardPosition + currGrade * GlobalConfig.Race.racerDistance;
        transform.position = RoadPresenter.GetPositionByDistance(pos);
        transform.forward = Vector3.Lerp(transform.forward, RoadPresenter.GetForwardByDistance(pos), deltaTime * 10);
        return pos;
    }

    public virtual void UpdateSteeringPosition(float deltaTime)
    {
        var stdest = Mathf.Clamp01(RaceModel.stats.speed / 30.0f) * SteeringValue * maxSteeringSpeed;
        currSteeringSpeed = Mathf.MoveTowards(currSteeringSpeed, stdest, maxSteeringSpeed * deltaTime * 3);
        var localPos = racer.transform.localPosition;
        localPos.x = localPos.x + currSteeringSpeed * deltaTime;
        if (localPos.x > RoadPresenter.RoadWidth)
        {
            localPos.x = RoadPresenter.RoadWidth;
            currSteeringSpeed = 0;
        }
        if (localPos.x < -RoadPresenter.RoadWidth)
        {
            localPos.x = -RoadPresenter.RoadWidth;
            currSteeringSpeed = 0;
        }
        racer.transform.localPosition = localPos;
    }

    public virtual void LoadRacer(PlayerData playerdata, bool isplayer)
    {
        player = playerdata;
        racer = RacerFactory.Racer.Create(player.RacerId, transform);
        racer.SetupCustom(player.RacerCustom).SetupCameras(isplayer);
        maxSteeringSpeed = player.RacerSteering;
        if (isplayer) racer.gameObject.AddComponent<AudioListener>();
        racer.BroadcastMessage("SetPlateText", player.name, SendMessageOptions.DontRequireReceiver);
    }

    public virtual void AddNitors()
    {
        if (NitrosReady) return;
        var value = Nitros + player.RacerNitrous * 0.1f;
        player.CurrNitrous = value;
    }

    public virtual void UseNitrous()
    {
        if (NitrosReady)
        {
            player.CurrNitrous = Mathf.Clamp(player.CurrNitrous - 1, 0, 1);
            SetGrade(Grade + 1);
        }
    }

    public virtual void Horn(bool play)
    {
        if (PlayingHorn == play) return;
        PlayingHorn = play;
        if (play)
            BroadcastMessage("PlayHornAudio", SendMessageOptions.DontRequireReceiver);
        else
            BroadcastMessage("StopHornAudio", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void OnCrashed()
    {
        player.CurrNitrous = Mathf.Max(0.0f, Nitros - (1.0f - player.RacerBody * 0.1f));
        BroadcastMessage("PlayCrashAudio", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void SetGrade(int grade, bool dontBlend = false)
    {
        changeGradeSpeed = grade - player.CurrGrade > 1 ? 1 : 0.5f;
        player.CurrGrade = grade;
        if (dontBlend) currGrade = grade;
        UpdatePositons();

        if (Mathf.Approximately(currGrade, Grade) == false)
            BroadcastMessage("StartNitors", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void PlayingUpdate(float deltaTime)
    {
        if (currGrade != Grade)
        {
            currGrade = Mathf.MoveTowards(currGrade, Grade, deltaTime * changeGradeSpeed);
            if (Mathf.Approximately(currGrade, Grade))
            {
                currGrade = Grade;
                BroadcastMessage("StopNitors", SendMessageOptions.DontRequireReceiver);
            }
        }
        UpdateForwardPosition(deltaTime);
        UpdateSteeringPosition(deltaTime);
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static List<PlayerData> allPlayers = new List<PlayerData>(10);
    public static List<PlayerPresenter> all = new List<PlayerPresenter>(10);
    public static PlayerPresenter local = null;

    public static void UpdateAll(float deltaTime)
    {
        for (int i = 0; i < all.Count; i++)
            all[i].PlayingUpdate(deltaTime);
    }

    public static int FindNextFreeGrade(int grade)
    {
        if (allPlayers.Count < 2) return grade + 1;

        allPlayers.Sort((x, y) => y.CurrGrade - x.CurrGrade);
        int index = allPlayers.FindIndex(x => x.CurrGrade == grade);
        if (index < 1) return grade + 1;

        var nextgrade = allPlayers[index - 1].CurrGrade;
        if (Mathf.Abs(nextgrade - grade) == 1)
        {
            while (allPlayers.Exists(x => x.CurrGrade == grade))
                grade++;
            return grade;
        }
        else return nextgrade - 1;
    }

    private static void UpdatePositons()
    {
        allPlayers.Sort((x, y) => y.CurrGrade - x.CurrGrade);
        for (int i = 0; i < allPlayers.Count; i++)
            allPlayers[i].CurrPosition = i;
    }
}