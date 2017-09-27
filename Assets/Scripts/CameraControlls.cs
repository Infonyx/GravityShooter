using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlls : MonoBehaviour {

    private float rotationAmount = 0;
    private float reRotate;
    private float reRotationSpeed = 0.01f;
    private float oldRot;


	public void rotateCamera()
    {
        rotationAmount += -Input.GetAxis("Mouse Y") * transform.GetComponentInParent<PlayerController>().ySensivity;
        rotationAmount = Mathf.Clamp(rotationAmount, -89, 89);
        transform.localEulerAngles = new Vector3(rotationAmount,0,0);
        //transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * transform.GetComponentInParent<PlayerController>().ySensivity,0,0));
    }

    public float resetCamera()
    {
        oldRot = rotationAmount;
        reRotate = Mathf.Lerp(rotationAmount, 0, reRotationSpeed);
        rotationAmount = rotationAmount - (oldRot - reRotate);

        transform.localEulerAngles= new Vector3(rotationAmount, 0, 0);
        ////Debug.Log(oldRot - reRotate + "current rot: "+ rotationAmount);
        return oldRot - reRotate;
    }
}
