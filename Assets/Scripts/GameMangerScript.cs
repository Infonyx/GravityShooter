using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMangerScript : Photon.PunBehaviour {

    public GameObject playerPrefab;
    private GameObject player;
    private PvpManager pvp;
    private PlayerController playerController;
    private static int playerClass;

    public override void OnJoinedRoom()
    {
        if (PlayerController.LocalPlayerInstance == null)
        {
            player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
            
        }
        ApplyClass(player);
    }

    public override void OnLeftRoom()
    {
        Destroy(GameObject.FindGameObjectWithTag("PlayerManager"));
        SceneManager.LoadScene(0);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //player.GetComponent<PlayerNameDisplayer>().DestroyName();
            PhotonNetwork.LeaveRoom();
        }
    }

    void LoadGame()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("Failed at 1");
        }
        Debug.Log("Photun: Loading level 1");
        PhotonNetwork.LoadLevel(1);
        ApplyClass(player);
    }

    private void ApplyClass(GameObject user)
    {
        
        playerClass = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerClassManager>().playerClass;
        switch (playerClass)
        {
            case 1:
                SetStats(user, 90, 70, 30, 0, 1, 12, 8);
                break;
            case 2:
                SetStats(user, 60, 7, 90, 0.02f, 0.05f, 16, 20);
                break;
            case 3:
                SetStats(user, 185, 23, 50, 0.1f, 0.25f, 10, 4);
                break;
            default:
                SetStats(user, 100, 23, 50, 0.1f, 0.25f, 14, 8);
                break;
        }
    }

    private void SetStats(GameObject user, int health, int shootdamage, int meleedamage, float spread, float cooldown, float forwardAcceleration, float sideAcceleration)
    {
        pvp = user.GetComponent<PvpManager>();
        playerController = user.GetComponent<PlayerController>();

        Debug.Log("Applied Stats");

        //pvp.health = health;
        //pvp.shootDamage = shootdamage;
        //pvp.meleeDamage = meleedamage;
        //pvp.spread = spread;
        //pvp.cooldownTime = cooldown;


        pvp.CallSetStats(health, shootdamage, meleedamage, spread, cooldown);

        //playerController.forwardAcceleration = forwardAcceleration;
        //playerController.strafeAcceleration = sideAcceleration;

        playerController.CallSetStats(forwardAcceleration, sideAcceleration);
    }

}
