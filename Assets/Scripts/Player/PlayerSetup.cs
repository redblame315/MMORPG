using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour 
{
    [SerializeField]
    Camera playerCamera;
    [SerializeField]
    AudioListener playerAudioListener;

    private NetworkClient nClient;
    private int latency;
    private Text latencyText;

	// Use this for initialization
	void Start () 
    {
	    if(isLocalPlayer)
        {
            try
            {
                nClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
            }
            catch(Exception ex)
            {
                Debug.Log("Error resolving local client." + ex.Message);
            }

            playerCamera.enabled = true;
            playerAudioListener.enabled = true;
 
            GetComponent<CharacterController>().enabled = true;
            GetComponent<PlayerCharacterController>().enabled = true;

            latencyText = GameObject.Find("Latency Text").GetComponent<Text>();
        }
	}

    void Update()
    {
        ShowLatency();
    }

    void ShowLatency()
    {
        if(isLocalPlayer)
        {
            latency = nClient.GetRTT();
            latencyText.text = latency.ToString();
        }
    }
}
