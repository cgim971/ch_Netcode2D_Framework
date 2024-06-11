using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Controller : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public SpriteRenderer sr;

    private PlayerInputData currentPlayerInputData;

    private void Start()
    {
        Application.targetFrameRate = 60;

        if (isLocalPlayer)
        {
            sr.color = Color.blue;
        }
    }

    float timer = 0.0f;
    private void Update()
    {
        if (isLocalPlayer)
        {
            LocalPlayerUpdate();
        }
        else
        {
            PlayerUpdate();
        }
    }

    void LocalPlayerUpdate()
    {
        PlayerInputData playerInputData = new PlayerInputData(
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D));

        Vector2 input = GetInputVector(playerInputData);
        ProcessInput(input);

        if (ComparisonPlayerInputData(playerInputData))
        {
            CmdSendInput(playerInputData);
            currentPlayerInputData = playerInputData;
            return;
        }

        if (timer < 1.0f)
        {
            timer += Time.deltaTime;
        }
        else
        {
            CmdSendInput(playerInputData);
            timer = 0.0f;
            return;
        }
    }

    void PlayerUpdate()
    {
        Vector2 input = GetInputVector(currentPlayerInputData);
        ProcessInput(input);
    }

    public void SetPlayerInputData(PlayerInputData playerInputData)
    {
        currentPlayerInputData = playerInputData;
    }

    Vector2 GetInputVector(PlayerInputData playerInputData)
    {
        int horizontal = 0;
        int vertical = 0;

        if (playerInputData.isUp)
            vertical += 1;

        if (playerInputData.isDown)
            vertical -= 1;

        if (playerInputData.isLeft)
            horizontal -= 1;

        if (playerInputData.isRight)
            horizontal += 1;

        return new Vector2(horizontal, vertical).normalized;
    }

    void ProcessInput(Vector2 input)
    {
        Vector2 move = input * moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }

    [Command]
    void CmdSendInput(PlayerInputData playerInputdata)
    {
        GameServer.Instance.ProcessPlayerMovement(gameObject, playerInputdata);
    }

    [ClientRpc]
    public void RpcReceivePos(Vector2 pos)
    {
        transform.position = pos;
    }

    public bool ComparisonPlayerInputData(PlayerInputData playerInputData)
    {
        if (currentPlayerInputData.isUp != playerInputData.isUp)
            return true;

        if (currentPlayerInputData.isDown != playerInputData.isDown)
            return true;

        if (currentPlayerInputData.isLeft != playerInputData.isLeft)
            return true;

        if (currentPlayerInputData.isRight != playerInputData.isRight)
            return true;

        return false;
    }
}

public struct PlayerInputData : NetworkMessage
{
    public bool isUp;
    public bool isDown;
    public bool isLeft;
    public bool isRight;

    public PlayerInputData(bool isUp, bool isDown, bool isLeft, bool isRight)
    {
        this.isUp = isUp;
        this.isDown = isDown;
        this.isLeft = isLeft;
        this.isRight = isRight;
    }
}
