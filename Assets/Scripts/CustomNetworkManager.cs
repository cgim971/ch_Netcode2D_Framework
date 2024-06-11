using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public GameObject gameServerPrefab;
    private GameObject gameServerInstance;

    public override void OnStartServer()
    {
        base.OnStartServer();
        gameServerInstance = Instantiate(gameServerPrefab);
        NetworkServer.Spawn(gameServerInstance);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);

        GameServer.Instance.RegisterPlayer(player);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            GameServer.Instance.UnregisterPlayer(conn.identity.gameObject);
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        if (gameServerInstance != null)
        {
            Destroy(gameServerInstance);
        }
    }


}
