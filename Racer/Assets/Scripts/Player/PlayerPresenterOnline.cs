using Photon;
using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPresenterOnline : PlayerPresenter
{
    private PhotonView photonView = null;

    private int[] eventInts = new int[1];           //  4 bytes
    private float[] eventFloats = new float[2];     //  8 bytes
    private byte[] eventObjects = new byte[12];     //  12 bytes

    public override bool IsInactive { get { return photonView.owner == null ? false : photonView.owner.IsInactive; } }
    public override bool IsMine { get { return photonView.isMine; } }
    public override bool IsMaster { get { return photonView.owner == null ? false : photonView.owner.IsMasterClient; } }

    protected override void OnEnable()
    {
        base.OnEnable();
        UiPlayingBoard.AddPlayer(player);
        PlayNetwork.OnEventCall += OnNetworkEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UiPlayingBoard.RemovePlayer(player);
        PlayNetwork.OnEventCall -= OnNetworkEvent;
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (local == null && photonView.isMine)
            local = this;
        else
            transform.RemoveChildren(0, 1);

        //  read data from network
        player = PlayerData.FromJson(photonView.instantiationData[0] as string);
        player.IsPlayer = local == this;

        //  load racer from player data
        Setup(player);

        //  initialize presneter
        if (photonView.isMine)
        {
            if (RaceModel.IsOnline)
            {
                racer.boxCollider.isTrigger = !player.IsPlayer;
                racer.boxCollider.gameObject.AddComponent<RacerCollisionContact>();
            }
            else
            {
                racer.boxCollider.isTrigger = false;
                racer.bodyTransform.gameObject.AddComponent<RacerCollisionContact>();
                AddRigidBody();
            }
        }
        else racer.boxCollider.isTrigger = true;
    }

    public override void UpdateSteeringPosition(float deltaTime)
    {
        if (photonView.isMine)
        {
            base.UpdateSteeringPosition(deltaTime);

            if (eventFloats[1] != racer.transform.localPosition.x)
            {
                eventInts[0] = photonView.viewID;
                eventFloats[0] = (float)PhotonNetwork.time;
                eventFloats[1] = racer.transform.localPosition.x;
                System.Buffer.BlockCopy(eventInts, 0, eventObjects, 0, 4);
                System.Buffer.BlockCopy(eventFloats, 0, eventObjects, 4, 8);
                PlayNetwork.SendEvent(PlayNetwork.Events.Movement, eventObjects, false);
                PhotonNetwork.SendOutgoingCommands();
            }
        }
        else
        {
            var localPos = racer.transform.localPosition;
            localPos.x = Mathf.Clamp(Mathf.Lerp(localPos.x, SteeringValue, deltaTime * 10), -RoadPresenter.RoadWidth, RoadPresenter.RoadWidth);
            racer.transform.localPosition = localPos;
        }
    }

    private void OnNetworkEvent(PlayNetwork.Events code, object content, int senderId)
    {
        if (code == PlayNetwork.Events.Movement)
        {
            System.Buffer.BlockCopy((byte[])content, 0, eventInts, 0, 4);
            System.Buffer.BlockCopy((byte[])content, 4, eventFloats, 0, 8);
            if (photonView.viewID == eventInts[0])
            {
                //float sendTime = eventFloats[0];
                SteeringValue = eventFloats[1];
            }
        }
    }

    public override void ReadyToRace(float nosPos, float steerPos)
    {
        base.ReadyToRace(nosPos, steerPos);
        photonView.RPC("NetReadyToRace", PhotonTargets.Others, nosPos, steerPos);
    }

    [PunRPC]
    private void NetReadyToRace(float nosPos, float steerPos)
    {
        base.ReadyToRace(nosPos, steerPos);
    }

    public override void OnCrashed()
    {
        base.OnCrashed();
        if (IsMine)
            photonView.RPC("NetOnCrashed", PhotonTargets.Others, nosPosition);
    }

    [PunRPC]
    private void NetOnCrashed(float nospos)
    {
        nosPosition = nospos;
        base.OnCrashed();
    }

    public override void UseNitrous()
    {
        if (IsNitrosReady && IsMine)
        {
            base.UseNitrous();
            photonView.RPC("NetUseNitrous", PhotonTargets.Others, player.CurrNitrous, nosPosition);
        }
    }

    [PunRPC]
    private void NetUseNitrous(float currnitros, float nospos)
    {
        player.CurrNitrous = currnitros;
        nosPosition = nospos;
        base.UseNitrous();
    }

    public override void BoostNitros()
    {
        base.BoostNitros();
        photonView.RPC("NetBoostNitros", PhotonTargets.Others, player.CurrNitrous, nosPosition);
    }

    [PunRPC]
    private void NetBoostNitros(float currnitros, float nospos)
    {
        player.CurrNitrous = currnitros;
        nosPosition = nospos;
        base.BoostNitros();
    }

    public override void Horn(bool play)
    {
        base.Horn(play);
        photonView.RPC("NetHorn", PhotonTargets.Others, play);
    }

    [PunRPC]
    private void NetHorn(bool play)
    {
        base.Horn(play);
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static PlayerPresenterOnline Create(PlayerData data, bool asBot)
    {
        var ndata = new object[] { JsonUtility.ToJson(data) };

        if (asBot)
            return PhotonNetwork.InstantiateSceneObject("Prefabs/Player", Vector3.zero, Quaternion.identity, 0, ndata).AddComponent<BotPresenter>().GetComponent<PlayerPresenterOnline>();
        else
            return PhotonNetwork.Instantiate("Prefabs/Player", Vector3.zero, Quaternion.identity, 0, ndata).GetComponent<PlayerPresenterOnline>();
    }
}
