using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Photon.PunBehaviour
{
    private float spawningRadius = 30;
    public int maxCount = 5;
    private Vector3 point;
    public GameObject planetPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        if (PhotonNetwork.isMasterClient)
        {
            for (int count = 1; count <= maxCount; count++)
            {
                point = Random.onUnitSphere;
                PhotonNetwork.Instantiate("Sphere", point * spawningRadius * count, transform.rotation, 0);
                PhotonNetwork.Instantiate("Sphere", -point * spawningRadius * count, transform.rotation, 0);
            }
        } 
    }

    
}
