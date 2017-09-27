using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.PunBehaviour {

    string _gameVersion = "1";
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    public byte maxPlayers = 10;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Awake () {
        PhotonNetwork.logLevel = Loglevel;

        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;

        Debug.Log("Test3");
    }


    public void Connect()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinRandomRoom();
        } else
        {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }

        Debug.Log("Test2");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayers }, null);
        Debug.Log("Test");
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");

            PhotonNetwork.LoadLevel(1);
        }
    }
}
