using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;

public class PlayNetworkLogger : MonoBehaviour, IPunCallbacks
{
    private ClientState lastConnectionState;

    private void Update()
    {
        if (PhotonNetwork.connectionStateDetailed != lastConnectionState)
        {
            lastConnectionState = PhotonNetwork.connectionStateDetailed;
            print(name + " : connectionStateDetailed : " + lastConnectionState);
        }
    }

    public void OnDisconnectedFromPhoton()
    {
        print(name + " : OnDisconnectedFromPhoton");
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        print(name + " : OnPhotonPlayerConnected");
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        print(name + " : OnPhotonPlayerDisconnected");
    }

    public void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        print(name + " : OnFailedToConnectToPhoton");
    }

    public void OnConnectionFail(DisconnectCause cause)
    {
        print(name + " : OnConnectionFail");
    }

    public void OnConnectedToMaster()
    {
        print(name + " : OnConnectedToMaster");
    }

    public void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        print(name + " : OnPhotonCreateRoomFailed");
    }

    public void OnJoinedRoom()
    {
        print(name + " : OnJoinedRoom : " + PhotonNetwork.room.Name);
    }

    public void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        print(name + " : OnPhotonJoinRoomFailed");
    }

    public void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        print(name + " : OnPhotonRandomJoinFailed");
    }

    public void OnLeftRoom()
    {
        print(name + " : OnLeftRoom");
    }

    public void OnPhotonMaxCccuReached()
    {
        print(name + " : OnPhotonMaxCccuReached");
    }

    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        print(name + " : OnPhotonCustomRoomPropertiesChanged");
    }

    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        print(name + " : OnPhotonPlayerPropertiesChanged");
    }

    public void OnUpdatedFriendList()
    {
        print(name + " : OnUpdatedFriendList");
    }

    public void OnWebRpcResponse(OperationResponse response)
    {
        print(name + " : OnWebRpcResponse");
    }

    public void OnOwnershipRequest(object[] viewAndPlayer)
    {
        print(name + " : OnOwnershipRequest");
    }

    public void OnLobbyStatisticsUpdate()
    {
        print(name + " : OnLobbyStatisticsUpdate");
    }

    public void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
    {
        print(name + " : OnPhotonPlayerActivityChanged");
    }

    public void OnOwnershipTransfered(object[] viewAndPlayers)
    {
        print(name + " : OnOwnershipTransfered");
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        print(name + " : OnCustomAuthenticationFailed");
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        print(name + " : OnCustomAuthenticationResponse");
    }

    public virtual void OnJoinedLobby()
    {
        print(name + " : OnJoinedLobby : " + PhotonNetwork.lobby.Name);
    }

    public void OnConnectedToPhoton()
    {
        print(name + " : OnConnectedToPhoton");
    }

    public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        print(name + " : OnMasterClientSwitched");
    }

    public void OnLeftLobby()
    {
        print(name + " : OnLeftLobby");
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        print(name + " : OnPhotonInstantiate");
    }

    public void OnReceivedRoomListUpdate()
    {
        print(name + " : OnReceivedRoomListUpdate");
    }

    public void OnCreatedRoom()
    {
        print(name + " : OnCreatedRoom");
    }
}
