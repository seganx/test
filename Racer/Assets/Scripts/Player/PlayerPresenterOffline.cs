using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPresenterOffline : PlayerPresenter
{

    public static PlayerPresenterOffline Create(PlayerData playerdata)
    {
        var player = Resources.Load<PlayerPresenterOffline>("Prefabs/OfflinePlayer").Clone<PlayerPresenterOffline>();
        player.Setup(playerdata);

        player.racer.boxCollider.isTrigger = false;
        player.racer.bodyTransform.gameObject.AddComponent<RacerCollisionContact>();
        player.AddRigidBody();

        return player;
    }
}
