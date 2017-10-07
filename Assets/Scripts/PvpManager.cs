using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpManager : Photon.PunBehaviour {

    
    public float health;
    private float healthChangeSpeed = 0.1f;
    private RaycastHit hit;

    private int playerClass;

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


    private float GruntForceModeTimeMax = 5;
    private float GruntForceModeTime = 0;
    private float GruntForceRechargeTime = 0;
    private float GruntForceRechargeTimeMax = 10;
    private float GruntForceRegen = 15;
    private float GruntForceRate = 0.1f;
    private float GruntOldRate;
    private float GruntSliderChange = 0.1f;
    private Slider GruntForceBar;
    private bool inForceMode = false;

    private float EnergyLevel = 0;
    private float EnergyChargeSpeed = 8f;
    private float EnergyShotUsage = 6f;
    private float EnergyBeamTime = 2f;
    private float EnergyBeamIntensity;
    private float EnergyBeamIntensityMultiplyer = 5f; 
    private float EnergyCurrentTime;
    private float EnergyLevelChangeSpeed = 0.1f;
    private Slider EnergySlider;
    private Text EnergyText;
    private bool inBeam = false;

    public void CallSetStats(int nPlayerClass, int nhealth, int nshootDamage, int nmeleeDamage, float nspread, float ncooldownTime)
    {
        photonView.RPC("SetStats", PhotonTargets.All, nPlayerClass, nhealth, nshootDamage, nmeleeDamage, nspread, ncooldownTime);
    }

    [PunRPC]
    public void SetStats(int nPlayerClass, int nhealth, int nshootDamage, int nmeleeDamage, float nspread, float ncooldownTime)
    {
        playerClass = nPlayerClass;
        maxHealth = nhealth;
        shootDamage = nshootDamage;
        meleeDamage = nmeleeDamage;
        spread = nspread;
        cooldownTime = ncooldownTime;

        health = maxHealth;
        healthBar = GameObject.FindGameObjectWithTag("Health Bar").GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;


        if (nPlayerClass == 3)
        {
            GruntForceBar = GameObject.FindGameObjectWithTag("GruntBar").GetComponent<Slider>();
            GruntForceBar.value = 1;
        } else if (nPlayerClass == 4)
        {
            EnergySlider = GameObject.FindGameObjectWithTag("EnergyBar").GetComponent<Slider>();
            EnergyText = GameObject.FindGameObjectWithTag("EnergyText").GetComponent<Text>();
        }
        
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
            healthBar.value = Mathf.Lerp(healthBar.value,health, healthChangeSpeed);
            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }

            
            ClassPvp();
            
            
            if (Input.GetMouseButton(0) && playerClass != 4)
            {
                Debug.Log("Shooting");
                Shoot(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("damage: " + shootDamage + ", health: " + maxHealth + ", cooldown:" + cooldownTime + ", health left:" + health + ", User: "+ PhotonNetwork.playerName);
        }
    }

    private void Shoot(bool instant)
    {
        if (cooldown <= 0 || instant)
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

    //Class Pvp-Abilities
    private void ClassPvp()
    {
        switch (playerClass)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                GruntPvp();
                break;
            case 4:
                EnergyPvp();
                break;
            default:
                break;
        }
    }

    private void GruntPvp()
    {
        if (GruntForceRechargeTime > 0 && !inForceMode)
        {
            GruntForceRechargeTime -= Time.deltaTime;
            GruntForceBar.value = Mathf.Lerp(GruntForceBar.value, 1 - GruntForceRechargeTime / GruntForceRechargeTimeMax, GruntSliderChange);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (GruntForceRechargeTime <= 0)
            {
                GruntForceRechargeTime = GruntForceRechargeTimeMax;
                StartCoroutine(GruntForce());
            }
        }
    }

    IEnumerator GruntForce()
    {
        inForceMode = true;
        GruntOldRate = cooldownTime;
        cooldownTime = GruntForceRate;
        GruntForceModeTime = GruntForceModeTimeMax;
        while (GruntForceModeTime > 0)
        {
            if (health < maxHealth)
            {
                health += Time.deltaTime * GruntForceRegen;
                photonView.RPC("SetHealth", PhotonTargets.Others, health);
            }

            GruntForceModeTime -= Time.deltaTime;
            GruntForceBar.value = Mathf.Lerp(GruntForceBar.value, GruntForceModeTime/ GruntForceModeTimeMax, GruntSliderChange);
            yield return null;
        }
        cooldownTime = GruntOldRate;
        GruntForceRechargeTime = GruntForceRechargeTimeMax;
        inForceMode = false;
    }


    private void EnergyPvp()
    {
        
        if (Input.GetMouseButton(0))
        {
            if (EnergyLevel > EnergyShotUsage && cooldown <= 0 && !inBeam)
            {
                Shoot(false);

                EnergyLevel -= EnergyShotUsage;
                cooldown = cooldownTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (EnergyLevel > 20)
            {
                StartCoroutine(EnergyChargeShot());
            }
        }

        if (EnergyLevel < 100 && !inBeam)
        {
            EnergyLevel += EnergyChargeSpeed * Time.deltaTime;
        }
        EnergySlider.value = Mathf.Lerp(EnergySlider.value, EnergyLevel / 100, EnergyLevelChangeSpeed);
        EnergyText.text = (EnergySlider.value*1000).ToString("0.00") + " MeV";
    }

    IEnumerator EnergyChargeShot()
    {
        EnergyBeamIntensity = EnergyLevel;
        Debug.Log(EnergyBeamIntensity);
        EnergyCurrentTime = EnergyBeamTime;
        inBeam = true;

        while (EnergyCurrentTime > 0)
        {
            /*for (int i=1; i < EnergyBeamIntensity * Time.deltaTime * EnergyBeamIntensityMultiplyer; i++)
            {
                Debug.Log("IUAWGFIUAWFGhi");
                Shoot(true);

            }*/
            Shoot(true);
            yield return new WaitForSeconds(EnergyBeamIntensityMultiplyer/EnergyBeamIntensity);

            if (EnergyLevel > 0)
            {
                EnergyLevel -= EnergyBeamIntensityMultiplyer / EnergyBeamTime;
            }
            EnergyCurrentTime -= Time.deltaTime + EnergyBeamIntensityMultiplyer/EnergyBeamIntensity;
        }

        inBeam = false;
    }

    [PunRPC]
    private void SetHealth(int nhealth)
    {
        health = nhealth;
    } 

    [PunRPC]
    public void Hit(int ownerShooterId, int viewShooterId, bool hit, Vector3 target, Vector3 pos, Vector3 direction, int viewTargetId, int ownerTargetId, int damage)
    {
        if (hit && PhotonView.Find(viewTargetId).gameObject.GetComponent<PvpManager>() != null)
        {
            direction = target - pos;   
        } else
        {
            direction = direction * 500;
        }

        if (gameObject.GetPhotonView().isMine)
        {
            DrawLineRender(true, pos, direction);
        } else
        {
            DrawLineRender(false, pos, direction);
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
                            
                                player.transform.position = Vector3.zero;
                                player.GetComponent<PvpManager>().health = player.GetComponent<PvpManager>().maxHealth;
                                photonView.RPC("SetHealth", PhotonTargets.Others, health);
                            } 
                        }
                        Debug.Log(player.GetPhotonView().ownerId + " has " + player.GetComponent<PvpManager>().health + " health left");

                        break;

                        
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

        ln.SetPositions(new Vector3[] { pos, pos + direction});
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
