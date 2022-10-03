using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour 
{
    private int layers;
    private float damage = 25;
    private float range = 200;
    private RaycastHit hit;

    [SerializeField]
    private Transform cameraTransform;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        CheckIsShooting();
	}

    void CheckIsShooting()
    {
        if (!isLocalPlayer) { return; }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();

        }
    }

    void Shoot()
    {
        layers = layers | ~(1 << LayerMask.NameToLayer("Player"));
        if (Physics.Raycast(cameraTransform.TransformPoint(0, 0, 0.5f), cameraTransform.forward, out hit, range, layers))
        {
            Debug.Log(hit.transform.name);

            if(hit.transform.tag != "Player")
            {
                var uidentity = hit.transform.name;
                CmdPlayerHit(uidentity, damage);
            }
        }
    }

    [Command]
    private void CmdPlayerHit(string uniqueID, float damage)
    {
        GameObject gameObject = GameObject.Find(uniqueID);

        // apply damage to player.
    }
}
