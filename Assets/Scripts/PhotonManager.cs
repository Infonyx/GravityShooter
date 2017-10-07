using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.PunBehaviour {

    string _gameVersion = "1";
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    public byte maxPlayers = 20;
    public GameObject connectingPanel;
    public GameObject controlPanel;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        connectingPanel.SetActive(false);
        controlPanel.SetActive(true);
    }

    void Awake () {
        PhotonNetwork.logLevel = Loglevel;

        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings(_gameVersion);
    }


    public void Connect()
    {
        connectingPanel.SetActive(true);
        controlPanel.SetActive(false);
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinRandomRoom();
        } else
        {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
            connectingPanel.SetActive(false);
            controlPanel.SetActive(true);
        }
        

    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        connectingPanel.SetActive(false);
        controlPanel.SetActive(true);
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayers }, null);
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinedRoom()
    {
        
        if (PhotonNetwork.room.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        connectingPanel.SetActive(false);
        controlPanel.SetActive(true);
    }
}
