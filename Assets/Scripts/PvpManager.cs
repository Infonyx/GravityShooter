using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpManager : Photon.PunBehaviour {

    private int maxHealth = 100;
    public int health;

    private RaycastHit hit;

    private int shootDamage = 24;
    private int meleeDamage = 100;

    private Slider healthBar;

    private LineRenderer ln;
    public GameObject lineRenderer;

	void Start () {
        health = maxHealth;
        healthBar = GameObject.FindGameObjectWithTag("Health Bar").GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
    }
	
	void Update () {
        if (photonView.isMine)
        {
            healthBar.value = health;
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Shooting");
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        if (Physics.Raycast(transform.position, transform.GetChild(0).gameObject.transform.forward, out hit))
        {
            photonView.RPC("Hit", PhotonTargets.All, photonView.ownerId, photonView.viewID, true, hit.transform.position, transform.position, transform.GetChild(0).gameObject.transform.forward, hit.transform.gameObject.GetPhotonView().viewID, hit.transform.gameObject.GetPhotonView().ownerId);
            Debug.Log("hit " + hit.transform.gameObject.name);
        } else
        {
            Debug.Log("ZWAGFDZUgwfu");
            photonView.RPC("Hit", PhotonTargets.All, photonView.ownerId, photonView.viewID,false, Vector3.zero, transform.position, transform.GetChild(0).gameObject.transform.forward, 0, 0);
        }
        Debug.DrawRay(transform.position, transform.GetChild(0).gameObject.transform.forward, Color.green, 500);
    }

    [PunRPC]
    public void Hit(int ownerShooterId, int viewShooterId, bool hit, Vector3 target, Vector3 pos, Vector3 direction, int viewTargetId, int ownerTargetId)
    {
        if (hit && PhotonView.Find(viewTargetId).gameObject.GetComponent<PvpManager>() != null)
        {
            direction = target - pos;   
        }

        if (gameObject.GetPhotonView().isMine)
        {
            DrawLineRender(true, pos, (direction).normalized);
        } else
        {
            DrawLineRender(false, pos, (direction).normalized);
        }

        if (hit)
        {
            if (PhotonView.Find(viewTargetId).gameObject.GetComponent<PvpManager>() != null)
            {
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (player.GetPhotonView().viewID == viewTargetId)
                    {
                        player.GetComponent<PvpManager>().health -= shootDamage;
                        Debug.Log(player.GetPhotonView().viewID + " has been hit");
                        if (gameObject == player)
                        {
                            
                        }
                        
                        
                        
                       
                        if (player.GetComponent<PvpManager>().health <= 0)
                        {
                            Debug.Log("Player is dead");
                        }
                        Debug.Log(player.GetPhotonView().ownerId + " has " + player.GetComponent<PvpManager>().health + " healt left");

                        break;

                        
                    }
                }
            }


            
        }
    }

    private void DrawLineRender(bool isShooter, Vector3 pos, Vector3 direction)
    {
        Debug.Log("Drawing");
        ln = Instantiate(lineRenderer, transform).GetComponent<LineRenderer>();
        if (isShooter)
        {            
            ln.startColor = Color.green;
            ln.endColor = Color.green;
        } else
        {
            ln.startColor = Color.red;
            ln.endColor = Color.red;
        }

        ln.SetPositions(new Vector3[] { pos, pos + direction * 500});
        StartCoroutine(Disapear(ln));

    }

    IEnumerator Disapear(LineRenderer lRenderer)
    {
        Debug.Log("This");
        float a = 1;
        while (a > 0) {
            lRenderer.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(lRenderer.endColor.r, lRenderer.endColor.g, lRenderer.endColor.b, a));
            a -= Time.deltaTime;
            yield return null;
        }
        Destroy(lRenderer.gameObject);
    }
}
