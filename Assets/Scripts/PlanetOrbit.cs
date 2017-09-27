using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetOrbit : Photon.PunBehaviour {

    private Rigidbody rigi;
    private Vector3 randomVector3;
    private Vector3 primaryPostition;

    private Vector3 oldTransform;

    public float orbitSpeed = 500;
    public Vector3 orbitVelocity;
    private Vector3 oldOrbitPos;
    private Vector3 orbitAxis;
    private Vector3 startPosition;

    public long timePosition;

	void Start () {
        startPosition = transform.position;
        rigi = gameObject.GetComponent<Rigidbody>();
        primaryPostition = transform.position;
        orbitAxis = Vector3.Cross(primaryPostition, Vector3.Cross(Vector3.up, primaryPostition));
        //StartCoroutine(Connecter());
    }
	
	void FixedUpdate () {
        timePosition++;
        oldTransform = transform.position;
        transform.position = startPosition;
        
        transform.RotateAround(Vector3.zero, orbitAxis, timePosition * orbitSpeed * Time.fixedDeltaTime / transform.position.magnitude);
        transform.RotateAround(transform.position, -orbitAxis, timePosition * orbitSpeed * Time.fixedDeltaTime / transform.position.magnitude);
        orbitVelocity = orbitSpeed * Vector3.Cross(transform.position, -orbitAxis).normalized * 2*Mathf.PI / 360;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PlayerController>().standingOnPlanet(gameObject))
            {
                player.transform.RotateAround(player.transform.position - gameObject.transform.position, orbitAxis, orbitSpeed * Time.fixedDeltaTime / gameObject.transform.position.magnitude);
                player.transform.RotateAround(player.transform.position, orbitAxis, -orbitSpeed * Time.fixedDeltaTime / gameObject.transform.position.magnitude);
            }
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("Player Connected");
        if (PhotonNetwork.isMasterClient)
        {
            photonView.RPC("SendData", PhotonTargets.Others, primaryPostition, timePosition);
        } else
        {
            Debug.Log("Isn't master Client");
        }
    }

    [PunRPC]
    private void SendData(Vector3 position, long time)
    {
        primaryPostition = position;
        timePosition = time;
        Debug.Log("Recieved: " + primaryPostition + " and Time: " + timePosition);
    }
    
}
