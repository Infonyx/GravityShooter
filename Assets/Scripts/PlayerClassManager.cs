using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassManager : MonoBehaviour {

	public int playerClass = 0;

	void Start () {
        if (GameObject.FindGameObjectsWithTag("PlayerManager").Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
	}

    public void setClass(int setClass)
    {
        playerClass = setClass;
        Debug.Log(playerClass);
    }
	
	
}
