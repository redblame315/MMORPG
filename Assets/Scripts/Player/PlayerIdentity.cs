using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerIdentity : NetworkBehaviour 
{
    [SyncVar]
    private string playerUniqueIdentity;

    private NetworkInstanceId playerNetworkIdentity;
    private Transform playerTransform;

    public override void OnStartLocalPlayer()
    {
        GetNetworkIdentity();
        SetIdentity();

        Debug.Log("OnStartLocalPlayer():" + playerUniqueIdentity);
    }

    void Awake()
    {
        playerTransform = transform;
    }

    void Update()
    {
        if (!isLocalPlayer && (playerTransform.name == "" || playerTransform.name == "Player(Clone)"))
        {
            SetIdentity();
        }
    }

    void SetIdentity()
    {
        if(!isLocalPlayer)
        {
            playerTransform.name = playerUniqueIdentity;
        }
        else
        {
            playerTransform.name = GenerateUniqueIdentity();
        }
    }

    [Client]
    void GetNetworkIdentity()
    {
        playerNetworkIdentity = GetComponent<NetworkIdentity>().netId;
        CmdSendUniqueIdentity(GenerateUniqueIdentity());
    }

    [Command]
    void CmdSendUniqueIdentity(string name)
    {
        playerUniqueIdentity = name;
    }

    string GenerateUniqueIdentity()
    {
        return string.Format("Client {0}", playerNetworkIdentity.ToString());
    }

    

}
