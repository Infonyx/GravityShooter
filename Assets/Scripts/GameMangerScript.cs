using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMangerScript : Photon.PunBehaviour {

    public GameObject playerPrefab;

    public override void OnJoinedRoom()
    {
        if (PlayerController.LocalPlayerInstance == null)
        {
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
    }

}
