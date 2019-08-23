using Photon;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPresenter : Base
{
    [HideInInspector] public PlayerData player = null;
    [HideInInspector] public RacerPresenter racer = null;

    private float nosPosition = 0;
    private float speedPosition = 0;
    private float currSteeringSpeed = 0;
    private float maxSteeringSpeed = 0;

    public virtual bool IsInactive { get { return false; } }
    public virtual bool IsMine { get { return true; } }
    public float SteeringValue { get; set; }
    public bool PlayingHorn { get; set; }
    public float Nitros { get { return player.CurrNitrous; } }
    public bool IsNitrosFull { get { return IsNitrosUsing == false && player.CurrNitrous >= 1.0f; } }
    public bool IsNitrosReady { get { return IsNitrosUsing == false && player.CurrNitrous >= 0.2f; } }
    public bool IsNitrosUsing { get; private set; }
    public bool IsNitrosPerfect { get; private set; }

    protected virtual void OnEnable()
    {
        all.Add(this);
        allPlayers.Add(player);
    }

    protected virtual void OnDisable()
    {
        all.Remove(this);
        if (all.Count < 2) allPlayers.Clear();
    }

    public virtual void Setup(PlayerData playerdata)
    {
        player = playerdata;
        maxSteeringSpeed = player.RacerSteering;

        racer = RacerFactory.Racer.Create(player.RacerId, transform);
        racer.SetupCustom(player.RacerCustom).SetupCameras(player.IsPlayer);
        racer.BroadcastMessage("SetPlateText", player.name, SendMessageOptions.DontRequireReceiver);
        racer.AutoSteeringWheel = true;
        racer.AutoWheelRotation = true;

        if (player.IsPlayer)
        {
            racer.gameObject.AddComponent<AudioListener>();
            transform.GetChild(0).ScaleLocalPosition(1, racer.Size.y / 1.279f, 0.5f + 0.4f * (racer.Size.z / 3.4f));
        }
    }

    public virtual Rigidbody AddRigidBody()
    {
        var rigid = racer.bodyTransform.gameObject.AddComponent<Rigidbody>();
        rigid.mass = 20;
        rigid.useGravity = false;
        rigid.constraints = RigidbodyConstraints.FreezeAll;
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        return rigid;
    }

    public virtual float UpdateForwardPosition(float deltaTime)
    {
        player.CurrPosition = speedPosition + nosPosition;
        transform.position = RoadPresenter.GetPositionByDistance(player.CurrPosition);
        transform.forward = Vector3.Lerp(transform.forward, RoadPresenter.GetForwardByDistance(player.CurrPosition), deltaTime * 10);
        return player.CurrPosition;
    }

    public virtual void UpdateSteeringPosition(float deltaTime)
    {
        var stdest = Mathf.Clamp01(player.CurrSpeed / 30.0f) * SteeringValue * maxSteeringSpeed;
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

    public virtual void SetNosPosition(float nosPos)
    {
        nosPosition = nosPos;
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

    public virtual void UseNitrous()
    {
        if (IsNitrosReady == false) return;
        IsNitrosPerfect = player.CurrNitrous > 0.99f;
        IsNitrosUsing = true;
        BroadcastMessage("StartNitors", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void PlayingUpdate(float gameTime, float deltaTime)
    {
        float forwardSpeedDelta = player.RacerMaxSpeed - RaceModel.specs.minForwardSpeed;
        player.CurrSpeed = Mathf.Min(gameTime * forwardSpeedDelta + RaceModel.specs.minForwardSpeed, player.RacerMaxSpeed);
        speedPosition += player.CurrSpeed * deltaTime;

        if (IsNitrosUsing)
        {
            nosPosition += 1.0f * (0 + player.RacerNitrous) * deltaTime; // toodoo: use deltaTime??
            player.CurrNitrous -= deltaTime / (.7f * (2 + player.RacerNitrous) * (IsNitrosPerfect ? 1 : 0.8f));
            if (player.CurrNitrous <= 0)
            {
                IsNitrosUsing = false;
                player.CurrNitrous = 0;
                BroadcastMessage("StopNitors", SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (IsNitrosFull == false)
        {
            player.CurrNitrous += 0.04f * (0 + player.RacerNitrous) * Time.deltaTime;
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

    public static void UpdateAll(float gameTime, float deltaTime)
    {
        for (int i = 0; i < all.Count; i++)
            all[i].PlayingUpdate(gameTime, deltaTime);
    }

    public static void UpdateRanks()
    {
        allPlayers.Sort((x, y) => y.CurrPosition > x.CurrPosition ? 1 : (y.CurrPosition == x.CurrPosition ? 0 : -1));
        for (int i = 0; i < allPlayers.Count; i++)
            allPlayers[i].CurrRank = i;
    }

    public static void SetReadyToRace()
    {
        all.Sort((x, y) => Random.Range(-99999, 99999));
        for (int i = 0; i < all.Count; i++)
            all[i].SetNosPosition(i * GlobalConfig.Race.racerDistance);
    }
}