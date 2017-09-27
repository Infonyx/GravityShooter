using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkLerp : Photon.PunBehaviour {

    Vector3 truePosition;
    Quaternion trueRotation;
    public float lerpSpeed = 5;
	
	void Update () {
		if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, truePosition, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, trueRotation, lerpSpeed * Time.deltaTime);
        }
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        } else
        {
            truePosition = (Vector3)stream.ReceiveNext();
            trueRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
