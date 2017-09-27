using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : Photon.PunBehaviour {

    public float size;
    public float velocityIncrease = 0.5f;
    private float gravityMaxDistance = 100;
    
    private float gravityMultiplier = 30;
    private float onPlanetMultiplier = 60;

    public float orbitDotProduct = 0.3f;
    public float orbitMaxSpeed = 40f;
    public float orbitMinSpeed = 6f;
    public float orbitRadius = 8;
    public float orbitVelocityIncrease = 0.05f;
    private float orbitCorrectorMultiplier = 0.4f;

    public float standingRadius = 2f;

	void Start () {
        if (PhotonNetwork.isMasterClient) { size = Random.Range(15, 30); Debug.Log("UWIAgfiuasgbf"); }
        Debug.Log(size + PhotonNetwork.playerName);
        transform.localScale = new Vector3(size,size,size);
    }

    private void FixedUpdate()
    {
        Debug.Log(size);
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetPhotonView().isMine) {
                Rigidbody rigi = player.GetComponent<Rigidbody>();
                if ((player.transform.position - transform.position).magnitude <= gravityMaxDistance)
                {
                    if (Mathf.Abs(Vector3.Dot(player.GetComponent<PlayerController>().worldVelocity(player, gameObject).normalized, (player.transform.position - transform.position).normalized)) < orbitDotProduct && rigi.velocity.magnitude <= orbitMaxSpeed && rigi.velocity.magnitude > orbitMinSpeed && (player.transform.position - transform.position).magnitude < size / 2 + orbitRadius && !Input.GetKey(KeyCode.LeftShift))
                    {
                        rigi.AddForce(((transform.position - player.transform.position).normalized * (rigi.velocity.magnitude * rigi.velocity.magnitude / (player.transform.position - transform.position).magnitude + (orbitCorrectorMultiplier * Vector3.Dot(player.transform.position - transform.position, rigi.velocity)))), ForceMode.Acceleration);
                        rigi.velocity = Vector3.Lerp(rigi.velocity, rigi.velocity.normalized * orbitMaxSpeed, 0.01f);
                    }

                    else if ((transform.position - player.transform.position).magnitude < size / 2 + standingRadius && !Input.GetKey(KeyCode.LeftShift))
                    {
                        rigi.AddForce((transform.position - player.transform.position).normalized * size * onPlanetMultiplier / (Mathf.Pow((transform.position - player.transform.position).magnitude, 2f)), ForceMode.Acceleration);
                    }

                    else
                    {
                        rigi.AddForce((transform.position - player.transform.position).normalized * size * gravityMultiplier / (Mathf.Pow((transform.position - player.transform.position).magnitude, 2f)), ForceMode.Acceleration);
                    }
                }
            }
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        if (PhotonNetwork.isMasterClient)
        {
            photonView.RPC("SendData", PhotonTargets.Others, size);
            Debug.Log("Sen");
        }
    }

    [PunRPC]
    private void SendData(float realSize)
    {
        size = realSize;
        transform.localScale = new Vector3(realSize, realSize, realSize);
    }
}
