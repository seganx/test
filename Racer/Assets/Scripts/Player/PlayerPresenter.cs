using Photon;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPresenter : Base
{
    [HideInInspector] public PlayerData player = null;
    [HideInInspector] public RacerPresenter racer = null;

    private VelocityReader velocityReader = new VelocityReader(10);
    private float currGrade = 0;
    private float changeGradeSpeed = 1;
    private float currSteeringSpeed = 0;
    private float maxSteeringSpeed = 0;

    public virtual bool IsInactive { get { return false; } }
    public virtual bool IsMine { get { return true; } }
    public float ForwardValue { get; set; }
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
        UiPlayingBoard.AddPlayer(player);
    }

    protected virtual void OnDisable()
    {
        player.CurrGrade = -100 * all.Count;
        all.Remove(this);
        UiPlayingBoard.RemovePlayer(player);
        if (all.Count < 2) allPlayers.Clear();
    }

    public virtual void UpdateForwardPosition()
    {
        var pos = ForwardValue + currGrade * GlobalConfig.Race.racerDistance;
        transform.position = RoadPresenter.GetPositionByDistance(pos);
        transform.forward = Vector3.Lerp(transform.forward, RoadPresenter.GetForwardByDistance(pos), Time.deltaTime * 10);
    }

    public virtual void UpdateSteeringPosition()
    {
        velocityReader.Update(racer.transform.position);
        var stdest = Mathf.Clamp01(velocityReader.z / 30.0f) * SteeringValue * maxSteeringSpeed;
        currSteeringSpeed = Mathf.MoveTowards(currSteeringSpeed, stdest, maxSteeringSpeed * Time.deltaTime * 3/** (Mathf.Abs(stdest) > Mathf.Epsilon ? 5 : 5)*/);
        var localPos = racer.transform.localPosition;
        localPos.x = localPos.x + currSteeringSpeed * Time.deltaTime;
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

    public virtual void SetGrade(int grade, bool dontBlend = false)
    {
        changeGradeSpeed = grade - player.CurrGrade > 1 ? 1 : 0.5f;
        player.CurrGrade = grade;
        if (dontBlend) currGrade = grade;
        UpdatePositons();
        UiPlayingBoard.UpdatePositions();
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
            Debug.Log("Use CurrNitrous: " + player.CurrNitrous);
            player.CurrNitrous = Mathf.Clamp(player.CurrNitrous - 1, 0, 1);
            Debug.Log("Curr CurrNitrous: " + player.CurrNitrous);
            SetGrade(Grade + 1);
            BroadcastMessage("StartNitors", SendMessageOptions.DontRequireReceiver);
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

    public virtual void Update()
    {
        if (currGrade != Grade)
        {
            currGrade = Mathf.MoveTowards(currGrade, Grade, Time.deltaTime * changeGradeSpeed);
            if (currGrade == Grade)
                BroadcastMessage("StopNitors", SendMessageOptions.DontRequireReceiver);
        }
        UpdateForwardPosition();
        UpdateSteeringPosition();
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static List<PlayerData> allPlayers = new List<PlayerData>();
    public static List<PlayerPresenter> all = new List<PlayerPresenter>();
    public static PlayerPresenter local = null;

    public static int FindNextFreeGrade(int grade)
    {
        if (all.Count < 2) return grade;
        if (all.Exists(x => x.Grade == grade))
            return FindNextFreeGrade(grade + 1);
        return grade;
    }

    public static int FindPrevFreeGrade(int grade)
    {
        if (all.Count < 2) return grade;
        if (all.Exists(x => x.Grade == grade))
            return FindPrevFreeGrade(grade - 1);
        return grade;
    }

    public static bool ValidateGrade(PlayerPresenter player, int grade)
    {
        if (grade > player.Grade) return true;
        int mingrade = all.FindMin(x => x == player ? 999 : x.Grade).Grade - 2;
        return grade >= mingrade;
    }

    private static void UpdatePositons()
    {
        allPlayers.Sort((x, y) => y.CurrGrade - x.CurrGrade);
        for (int i = 0; i < allPlayers.Count; i++)
            allPlayers[i].CurrPosition = i;
    }
}