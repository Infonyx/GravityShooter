using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameDisplayer : Photon.PunBehaviour {

    public GameObject textField;
    private GameObject canvas;
    public Text myName;
    private Camera cam;
    private RaycastHit hit;

    private float displacement = Screen.height / 2;
    private float fontSize = 100;
    private float maxDistance = 150;
    private int minSize = 14;

	void Awake () {
        if (!photonView.isMine)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            canvas = GameObject.FindGameObjectWithTag("Canvas");
            myName = Instantiate(textField, canvas.transform).GetComponent<Text>();
            myName.text = gameObject.GetPhotonView().owner.NickName;
        } else
        {
            Destroy(GetComponent<PlayerNameDisplayer>());
        }
	}

    public void DestroyName()
    {
        Destroy(myName);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);

        foreach (GameObject displayer in GameObject.FindGameObjectsWithTag("NameDisplayer"))
        {
            Destroy(displayer);
        }        
        
        if (!photonView.isMine)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            canvas = GameObject.FindGameObjectWithTag("Canvas");
            myName = Instantiate(textField, canvas.transform).GetComponent<Text>();
            myName.text = gameObject.GetPhotonView().owner.NickName;
        }
    }

    void Update () {
        if (Vector3.Dot(cam.gameObject.transform.forward, transform.position - cam.gameObject.transform.position) > 0 && Vector3.Distance(transform.position, cam.transform.position) < maxDistance)
        {
            Physics.Raycast(cam.transform.position, transform.position - cam.transform.position, out hit);
            if (hit.transform.gameObject == gameObject)
            {
                myName.enabled = true;
                myName.transform.position = cam.WorldToScreenPoint(transform.position) + new Vector3(0, displacement / Vector3.Distance(transform.position, cam.transform.position), 0);
                myName.fontSize = (int)Mathf.Clamp((fontSize / Vector3.Distance(transform.position, cam.transform.position)), minSize, fontSize);
            } else
            {
                myName.enabled = false;
            }
            
        } else
        {
            myName.enabled = false;
        }
        
	}
}
