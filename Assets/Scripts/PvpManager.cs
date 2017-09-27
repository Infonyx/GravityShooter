using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpManager : Photon.PunBehaviour {

    private int maxHealth = 100;
    public int health;

    private RaycastHit hit;

    private int shootDamage = 25;
    private int meleeDamage = 100;

    private Slider healthBar;

	void Start () {
        health = maxHealth;
        healthBar = GameObject.FindGameObjectWithTag("Health Bar").GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
    }
	
	void Update () {
        if (Input.GetMouseButtonDown(0) && photonView.isMine)
        {
            Debug.Log("Shooting");
            Shoot();
        }
    }

    private void Shoot()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            photonView.RPC("Hit", PhotonTargets.All, hit.rigidbody.gameObject);
            Debug.Log("hit player");
        }
        Debug.DrawRay(transform.position, transform.forward, Color.green, 500);
    }

    [PunRPC]
    public void Hit(GameObject target)
    {
        if (PhotonNetwork.isMasterClient)
        {
            target.GetComponent<PvpManager>().health -= shootDamage;
            target.GetComponent<PvpManager>().healthBar.value = health;
            Debug.Log("has been hit");
            if (target.GetComponent<PvpManager>().health <= 0)
            {
                Debug.Log("Player is dead");
            }
        }
    }
}
