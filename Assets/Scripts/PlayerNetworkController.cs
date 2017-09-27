using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkController : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
        {
            Destroy(transform.GetChild(0).gameObject);
            Destroy(gameObject.GetComponent<PlayerController>());
        }
	}
}
