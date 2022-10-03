using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.03f)]
public class PlayerPosition : NetworkBehaviour 
{
    [SyncVar(hook = "OnSyncPositionValues")]
    private Vector3 syncPos;

    [SerializeField]
    Transform playerTransform;

    float posLerpRate = 18f;
    float posNormalLerpRate = 18f;
    float posFastLerpRate = 27f;

    private Vector3 lastPos;
    private float threshholdPos = 0.3f;

    private List<Vector3> syncPosQue = new List<Vector3>();
    private float minQueuedApproachDist = 0.1f;

    [SerializeField]
    private bool useQueuedInterpolation = false;

    void Start()
    {
        posLerpRate = posNormalLerpRate;
    }

    // Update is called once per frame
    void Update ()
    {
        LerpPosition();
    }

	// FixedUpdate is called on a fixed interval
	void FixedUpdate () 
    {
        SendPosition();
	}

    void LerpPosition()
    {
        if(!isLocalPlayer)
        {
            if (useQueuedInterpolation)
            {
                QueuedInterpolation();
            }
            else
            {
                StandardInterpolation();
            }
        }
    }

    [Command]
    void CmdSendPosition(Vector3 pos)
    {
        syncPos = pos;
    }

    [Client]
    void SendPosition()
    {
        if(isLocalPlayer && Vector3.Distance(playerTransform.position, lastPos) > threshholdPos)
        {
            // Debug.Log("ClientCallback:SendPosition()");
            CmdSendPosition(playerTransform.position);
            lastPos = playerTransform.position;
            // Debug.Log(playerTransform.position.ToString());
        }
    }

    [Client]
    void OnSyncPositionValues(Vector3 latestPosition)
    {
        syncPos = latestPosition;
        syncPosQue.Add(syncPos);
    }

    void StandardInterpolation()
    {
        playerTransform.position = Vector3.Lerp(playerTransform.position, syncPos, Time.deltaTime * posLerpRate);
    }

    void QueuedInterpolation()
    {
        // Only lerp if we have a destination
        if (syncPosQue.Count > 0)
        {
            playerTransform.position = Vector3.Lerp(playerTransform.position, syncPosQue[0], Time.deltaTime * posLerpRate);

            if (Vector3.Distance(playerTransform.position, syncPosQue[0]) < minQueuedApproachDist)
            {
                syncPosQue.RemoveAt(0);
            }

            if (syncPosQue.Count > 10)
            {
                posLerpRate = posFastLerpRate;
            }
            else
            {
                posLerpRate = posNormalLerpRate;
            }

            // Debug.Log(syncPosQue.Count.ToString());
        }
    }
}
