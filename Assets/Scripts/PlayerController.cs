using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Photon.MonoBehaviour {

    Rigidbody rigi;
    [HideInInspector] public float forwardAcceleration = 14;
    [HideInInspector] public float strafeAcceleration = 8;
    private float walkingSpeed = 400f;
    private Vector3 walkingChange = new Vector3(0,0,0);
    private float jumpSpeed = 15f;
    Camera cam;
    private float fov = 100f;
    private float fastfov = 30f;
    private float fovspeed = 0.05f;
    private GameObject playerSelf;

    private Vector3 oldpos;
    public GameObject nearestPlanet;
    private float xSensivity = 2f;
    public float ySensivity = 2f;
    private float turnrot = 0;
    private float autoTurnRot = 0;
    private float autoTurnRotSpeed = 0.1f;
    public float rotSensivity = 1f;

    private float onPlanetAutoRotSpeed = 1f;

    public float speedLimit = 30;
    public float speedLimitSettingSpeed = 0.28f;
    public float deacceleratorSpeed = 0.9f;

    private float dashSpeed = 40;
    private float dashCooldown = 4;
    private float dashVelocityDecrease = 0.2f;
    private float dashBarChangeSpeed = 0.1f;
    private float leftDashDelay = 0;
    private float rightDashDelay = 0;
    private Slider leftBar;
    private Slider rightBar;


    public float sniperZoom = 3.5f;

    private float maxStandingVelocity = 4;
    private float directionInterpolation = 0.028f;

    private bool lastAttracted = false;
    private bool inJump = false;

    private CameraControlls cameraControlls;

    public static GameObject LocalPlayerInstance;

    private void Awake()
    {
        if (photonView.isMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
        }
        //DontDestroyOnLoad(this.gameObject);
        
        /*if (GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerClassManager>().playerClass == 2)
        {
            GameObject.FindGameObjectWithTag("PhantomPanel").SetActive(true);
            leftBar = GameObject.FindGameObjectWithTag("PhantomLeftBar").GetComponent<Slider>();
            rightBar = GameObject.FindGameObjectWithTag("PhantomRightBar").GetComponent<Slider>();
        }*/
        
    }


    public void CallSetStats(float nforwardSpeed, float nstrafeSpeed)
    {
        photonView.RPC("SetStats", PhotonTargets.All, nforwardSpeed, nstrafeSpeed);
    }

    [PunRPC]
    public void SetStats(float nforwardSpeed, float nstrafeSpeed)
    {
        forwardAcceleration = nforwardSpeed;
        strafeAcceleration = nstrafeSpeed;
    }

    void Start() {
        if (!photonView.isMine && PhotonNetwork.connected) { 
            Destroy(transform.GetChild(0).gameObject);
            //Destroy(gameObject.GetComponent<PlayerController>());
        }
       
        playerSelf = gameObject;
        rigi = GetComponent<Rigidbody>();
        rigi.freezeRotation = true;
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        oldpos = Input.mousePosition;

        cameraControlls = transform.GetChild(0).gameObject.GetComponent<CameraControlls>();

        if (GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerClassManager>().playerClass == 2)
        {
            leftBar = GameObject.Find("LeftBar").GetComponent<Slider>();
            rightBar = GameObject.Find("RightBar").GetComponent<Slider>();
        } else
        {
            GameObject.Find("Phantom").SetActive(false);
        }
    } //Setup 

    private void FixedUpdate()
    {
        if (photonView.isMine && PhotonNetwork.connected)
        {

            FindNearestPlanet();

            if (!standingOnPlanet(nearestPlanet) || Input.GetKeyDown(KeyCode.LeftShift))
            {           
                jetBoost();             
            }
            

            if (Input.GetKey(KeyCode.LeftControl))
            {
                rigi.velocity = rigi.velocity * Mathf.Pow(deacceleratorSpeed, 144 / 50);
            }

            if (Input.GetMouseButton(1))
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov / sniperZoom, fovspeed); //Fov change
            } else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Clamp(fov + rigi.velocity.magnitude, fov, fastfov + fov), fovspeed); //Fov change
            }
        }
    }

    void Update() //Movement + FOV
    {
        if (photonView.isMine && PhotonNetwork.connected)
        {
            if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            FindNearestPlanet();

            if (standingOnPlanet(nearestPlanet))
            {
                walk(nearestPlanet);
            }

            rotate();

            if (GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerClassManager>().playerClass == 2)
            {
                PhantomUpdate();
                PhantomMovement();
            }
        }
        //Debug.Log("Velocity: " + rigi.velocity.magnitude + photonView.owner);

    }

    private void jetBoost()
    {
        if (Input.GetKey(KeyCode.W) && rigi.velocity.magnitude < speedLimit)
        {
            rigi.AddRelativeForce(new Vector3(0, 0, forwardAcceleration), ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.W) && rigi.velocity.magnitude > 0)
        {
            rigi.velocity = Vector3.Slerp(rigi.velocity.normalized, transform.forward, directionInterpolation) * rigi.velocity.magnitude;
        }
        if (Input.GetKey(KeyCode.A) && rigi.velocity.magnitude < speedLimit)
        {
            rigi.AddRelativeForce(new Vector3(-strafeAcceleration, 0, 0), ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.D) && rigi.velocity.magnitude < speedLimit)
        {
            rigi.AddRelativeForce(new Vector3(strafeAcceleration, 0, 0), ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.S) && rigi.velocity.magnitude < speedLimit)
        {
            rigi.AddRelativeForce(new Vector3(0, 0, -strafeAcceleration), ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.Space) && rigi.velocity.magnitude < speedLimit)
        {
            rigi.AddRelativeForce(new Vector3(0, strafeAcceleration, 0), ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.C) && rigi.velocity.magnitude < speedLimit)
        {
            rigi.AddRelativeForce(new Vector3(0, -strafeAcceleration, 0), ForceMode.Acceleration);
        }
        
    } //InSpace-Movement

    private void PhantomUpdate()
    {
        leftBar.value = Mathf.Lerp(leftBar.value,1-leftDashDelay/dashCooldown, dashBarChangeSpeed);
        rightBar.value =Mathf.Lerp(rightBar.value, 1-rightDashDelay/dashCooldown, dashBarChangeSpeed);
    }
    private void PhantomMovement()
    {
        if (leftDashDelay > 0)
        {
            leftDashDelay -= Time.deltaTime;
        }
        if (rightDashDelay > 0)
        {
            rightDashDelay -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.A) && leftDashDelay <= 0)
            {
                rigi.velocity = rigi.velocity * dashVelocityDecrease;
                rigi.AddRelativeForce(new Vector3(-dashSpeed, 0, 0), ForceMode.VelocityChange);
                leftDashDelay = dashCooldown;
            }
            if (Input.GetKeyDown(KeyCode.D) && rightDashDelay <= 0)
            {
                rigi.velocity = rigi.velocity * dashVelocityDecrease;
                rigi.AddRelativeForce(new Vector3(dashSpeed, 0, 0), ForceMode.VelocityChange);
                rightDashDelay = dashCooldown;
            }
        }
    }

    private void walk(GameObject nearestPlanet)
    {

        if (Input.GetKey(KeyCode.W))
        {
            transform.RotateAround(nearestPlanet.transform.position, transform.right, Time.deltaTime * walkingSpeed / (Vector3.Distance(transform.position, nearestPlanet.transform.position)));
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.RotateAround(nearestPlanet.transform.position, transform.forward, Time.deltaTime * walkingSpeed / (Vector3.Distance(transform.position, nearestPlanet.transform.position)));
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.RotateAround(nearestPlanet.transform.position, transform.forward, Time.deltaTime * -walkingSpeed / (Vector3.Distance(transform.position, nearestPlanet.transform.position)));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.RotateAround(nearestPlanet.transform.position, transform.right, Time.deltaTime * -walkingSpeed / (Vector3.Distance(transform.position, nearestPlanet.transform.position)));
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigi.AddForce((transform.position - nearestPlanet.transform.position).normalized * jumpSpeed, ForceMode.VelocityChange);
        }
    } //Planet-Movement

    private void rotate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            turnrot = rotSensivity;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            turnrot = -rotSensivity;
        }

        FindNearestPlanet();

        if (nearestPlanet != null)
        {
            if (standingOnPlanet(nearestPlanet))
            {
                autoTurnRot = Vector3.Dot(transform.right, (nearestPlanet.transform.position - transform.position));
            }
            else if (Vector3.Dot(transform.forward, Vector3.Cross(transform.up, (transform.position - nearestPlanet.transform.position))) < 0 && rigi.velocity.magnitude > 16 && (nearestPlanet.transform.position - transform.position).magnitude < nearestPlanet.GetComponent<PlanetManager>().size / 2 + nearestPlanet.GetComponent<PlanetManager>().orbitRadius)
            {
                autoTurnRot = Vector3.Dot(transform.up, (transform.position - nearestPlanet.transform.position));
            }
            else if (Vector3.Dot(transform.forward, Vector3.Cross(transform.up, (transform.position - nearestPlanet.transform.position))) > 0 && rigi.velocity.magnitude > 16 && (nearestPlanet.transform.position - transform.position).magnitude < nearestPlanet.GetComponent<PlanetManager>().size / 2 + 3)
            {
                autoTurnRot = Vector3.Dot(-transform.up, (transform.position - nearestPlanet.transform.position));
            }
        }
        if (nearestPlanet != null && photonView.isMine)
        {
            if ((nearestPlanet.transform.position - transform.position).magnitude <= nearestPlanet.GetComponent<PlanetManager>().size / 2 + nearestPlanet.GetComponent<PlanetManager>().standingRadius && (worldVelocity(gameObject, nearestPlanet) - nearestPlanet.GetComponent<PlanetOrbit>().orbitVelocity).magnitude <= maxStandingVelocity && !Input.GetKey(KeyCode.LeftShift))
            {
                transform.Rotate(new Vector3(Vector3.Dot(-transform.forward, (nearestPlanet.transform.position - transform.position).normalized) * onPlanetAutoRotSpeed, (Input.GetAxis("Mouse X") * xSensivity), turnrot + autoTurnRot * autoTurnRotSpeed));
                cameraControlls.rotateCamera();
            } else
            {
                transform.Rotate(new Vector3((-Input.GetAxis("Mouse Y") * ySensivity) + cameraControlls.resetCamera(), (Input.GetAxis("Mouse X") * xSensivity), turnrot + autoTurnRot * autoTurnRotSpeed));
                
            }
        } else if (photonView.isMine)
        {
            transform.Rotate(new Vector3((-Input.GetAxis("Mouse Y") * ySensivity) + cameraControlls.resetCamera(), (Input.GetAxis("Mouse X") * xSensivity), turnrot + autoTurnRot * autoTurnRotSpeed));
            cameraControlls.resetCamera();
        }
        
        turnrot = 0;
        autoTurnRot = 0;
    }

    public Vector3 worldVelocity(GameObject player,GameObject planet)
    {
        return player.GetComponent<Rigidbody>().velocity + planet.GetComponent<PlanetOrbit>().orbitVelocity;
    }

    public bool standingOnPlanet(GameObject planet)
    {
        if (planet != null)
            return (planet.transform.position - playerSelf.transform.position).magnitude <= planet.GetComponent<PlanetManager>().size / 2 + planet.GetComponent<PlanetManager>().standingRadius && (worldVelocity(playerSelf, planet) - planet.GetComponent<PlanetOrbit>().orbitVelocity).magnitude <= maxStandingVelocity && !Input.GetKey(KeyCode.LeftShift);
        else return false;
    }

    public bool isTheMasterClient()
    {
        return PhotonNetwork.isMasterClient;
    }

    private void FindNearestPlanet()
    {
        foreach (GameObject planet in GameObject.FindGameObjectsWithTag("Planet"))
        {
            if (nearestPlanet == null) nearestPlanet = planet;
            else if ((nearestPlanet.transform.position - transform.position).magnitude > (planet.transform.position - transform.position).magnitude)
            {
                nearestPlanet = planet;
            }
        }
    }

    /*
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(GetComponent<Rigidbody>().position);
            
        } else
        {
            GetComponent<Rigidbody>() = stream.ReceiveNext();
        }
            
    } */
}
