using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpManager : Photon.PunBehaviour {

    
    public int health;
    private RaycastHit hit;

    [HideInInspector] public int maxHealth;
    [HideInInspector] public int shootDamage;
    [HideInInspector] public int meleeDamage;
    [HideInInspector] public float spread;
    [HideInInspector] public float cooldownTime;

    private Vector3 shotmodifier;
    private float cooldown = 0;
    
    private Slider healthBar;

    private LineRenderer ln;
    public GameObject lineRenderer;

    public void CallSetStats(int nhealth, int nshootDamage, int nmeleeDamage, float nspread, float ncooldownTime)
    {
        photonView.RPC("SetStats", PhotonTargets.All, nhealth, nshootDamage, nmeleeDamage, nspread, ncooldownTime);
    }

    [PunRPC]
    public void SetStats(int nhealth, int nshootDamage, int nmeleeDamage, float nspread, float ncooldownTime)
    {
        
        maxHealth = nhealth;
        shootDamage = nshootDamage;
        meleeDamage = nmeleeDamage;
        spread = nspread;
        cooldownTime = ncooldownTime;

        health = maxHealth;
        healthBar = GameObject.FindGameObjectWithTag("Health Bar").GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;

    }

	void SetHealth () {
        health = maxHealth;
        healthBar = GameObject.FindGameObjectWithTag("Health Bar").GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
    }
	
	void Update () {
        if (photonView.isMine)
        {
            healthBar.value = health;
            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }
            
            if (Input.GetMouseButton(0))
            {
                Debug.Log("Shooting");
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("damage: " + shootDamage + ", health: " + maxHealth + ", cooldown:" + cooldownTime + ", health left:" + health + ", User: "+ PhotonNetwork.playerName);
        }
    }

    private void Shoot()
    {
        if (cooldown <= 0)
        {
            shotmodifier = Random.onUnitSphere;
            if (Physics.Raycast(transform.position, transform.GetChild(0).gameObject.transform.forward + shotmodifier * spread, out hit))
            {
                photonView.RPC("Hit", PhotonTargets.All, photonView.ownerId, photonView.viewID, true, hit.transform.position, transform.position, transform.GetChild(0).gameObject.transform.forward + shotmodifier * spread, hit.transform.gameObject.GetPhotonView().viewID, hit.transform.gameObject.GetPhotonView().ownerId, shootDamage);
                Debug.Log("hit " + hit.transform.gameObject.name);
            }
            else
            {
                Debug.Log("ZWAGFDZUgwfu");
                photonView.RPC("Hit", PhotonTargets.All, photonView.ownerId, photonView.viewID, false, Vector3.zero, transform.position, transform.GetChild(0).gameObject.transform.forward + shotmodifier * spread, 0, 0, shootDamage);
            }
            Debug.DrawRay(transform.position, transform.GetChild(0).gameObject.transform.forward + shotmodifier * spread, Color.green, 500);
            cooldown = cooldownTime;
        }
    }

    [PunRPC]
    public void Hit(int ownerShooterId, int viewShooterId, bool hit, Vector3 target, Vector3 pos, Vector3 direction, int viewTargetId, int ownerTargetId, int damage)
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
                        player.GetComponent<PvpManager>().health -= damage;
                        Debug.Log(player.GetPhotonView().viewID + " has been hit");
                        if (gameObject == player)
                        {
                            
                        }
                        
                        
                        
                       
                        if (player.GetComponent<PvpManager>().health <= 0)
                        {
                            //Die
                            transform.position = Vector3.zero;
                            health = maxHealth;
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
            a -= 2 * Time.deltaTime;
            yield return null;
        }
        Destroy(lRenderer.gameObject);
    }
}
