using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerRotation : NetworkBehaviour
{
    [SyncVar (hook="OnSyncPlayerRotation")]
    private float syncPlayerRot;
    [SyncVar(hook = "OnSyncCameraRotation")]
    private float syncCameraRot;

    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    Transform cameraTransform;

    private float rotLerpRate = 20f;

    private float lastPlayerRot;
    private float lastCameraRot;
    private float thresholdRot = 0.5f;

    private List<float> syncPlayerRotQue = new List<float>();
    private List<float> syncCameraRotQue = new List<float>();
    private float minQueuedApproachRot = 0.4f;

    [SerializeField]
    private bool useQueuedInterpolation = false;

	// Update is called once per frame
	void Update () 
    {
        LerpRotations();

        // TODO: working out how to animate player rotation in place. 
        //if ((lastPlayerRot - playerTransform.localEulerAngles.y) != 0)
        //{
        //    Debug.Log("PlayerRotChange: " + (lastPlayerRot - playerTransform.localEulerAngles.y).ToString());
        //}
	}

    // FixedUpdate is called on a fixed interval
    void FixedUpdate()
    {
        SendRotations();
    }

    void LerpRotations()
    {
        if (!isLocalPlayer)
        {
            if(useQueuedInterpolation)
            {
                QueuedInterpolation();
            }
            else 
            {
                StandardInterpolation();
            }
        }
    }

    private void StandardInterpolation()
    {
        InterpolatePlayerRotation(syncPlayerRot);
        InterpolateCameraRotation(syncCameraRot);
    }

    private void QueuedInterpolation()
    {
        if(syncPlayerRotQue.Count > 0)
        {
            InterpolatePlayerRotation(syncPlayerRotQue[0]);
            if (Mathf.Abs(playerTransform.localEulerAngles.y - syncPlayerRotQue[0]) < minQueuedApproachRot)
            {
                syncPlayerRotQue.RemoveAt(0);
            }
        }

        if(syncCameraRotQue.Count > 0)
        {
            InterpolateCameraRotation(syncCameraRotQue[0]);
            if (Mathf.Abs(cameraTransform.localEulerAngles.x - syncCameraRotQue[0]) < minQueuedApproachRot)
            {
                syncCameraRotQue.RemoveAt(0);
            }
        }

        // Debug.Log(string.Format("PLayer: {0} | Camera: {1}", syncPlayerRotQue.Count.ToString(), syncCameraRotQue.Count.ToString()));
    }

    private void InterpolateCameraRotation(float cameraRot)
    {
        var cameraRotation = new Vector3(cameraRot, 0, 0);
        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(cameraRotation), rotLerpRate * Time.deltaTime);
    }

    private void InterpolatePlayerRotation(float playerRot)
    {
        var playerRotation = new Vector3(0, playerRot, 0);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(playerRotation), rotLerpRate * Time.deltaTime);
    }

    [Command]
    void CmdSendRotations(float playerRot, float cameraRot)
    {
        syncPlayerRot = playerRot;
        syncCameraRot = cameraRot;
    }

    [Client]
    void SendRotations()
    {
        if (isLocalPlayer)
        {
            if (CheckIsBeyondThreshold(playerTransform.localEulerAngles.y, lastPlayerRot) || CheckIsBeyondThreshold(cameraTransform.localEulerAngles.x, lastCameraRot))
            {
                lastPlayerRot = playerTransform.localEulerAngles.y;
                lastCameraRot = cameraTransform.localEulerAngles.x;

                CmdSendRotations(lastPlayerRot, lastCameraRot);
            }
        }
    }

    bool CheckIsBeyondThreshold(float rot1, float rot2)
    {
        return (Mathf.Abs(rot1 - rot2) > thresholdRot) ? true : false;
    }

    [Client]
    private void OnSyncPlayerRotation(float latestPlayerRot)
    {
        syncPlayerRot = latestPlayerRot;
        syncPlayerRotQue.Add(syncPlayerRot);
    }

    [Client]
    private void OnSyncCameraRotation(float latestCameraRot)
    {
        syncCameraRot = latestCameraRot;
        syncCameraRotQue.Add(syncCameraRot);
    }
}
