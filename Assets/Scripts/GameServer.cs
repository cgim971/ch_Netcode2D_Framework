using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServer : MonoBehaviour
{
    public static GameServer Instance;

    private List<GameObject> playerList = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterPlayer(GameObject player) => playerList.Add(player);

    public void UnregisterPlayer(GameObject player) => playerList.Remove(player);

    public void ProcessPlayerMovement(GameObject player, PlayerInputData playerInputdata)
    {
        Controller controller = player.GetComponent<Controller>();
        if (controller == null)
            return;

        controller.SetPlayerInputData(playerInputdata);
        SendProcessPlayerPos(player);
    }

    public void SendProcessPlayerPos(GameObject ignorePlayer)
    {
        foreach (GameObject player in playerList)
        {
            if (player == ignorePlayer)
                continue;

            Controller controller = player.GetComponent<Controller>();
            if (controller != null)
            {
                controller.RpcReceivePos(player.transform.position);
            }
        }
    }
}
