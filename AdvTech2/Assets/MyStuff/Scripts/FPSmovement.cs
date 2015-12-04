using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FPSmovement : MonoBehaviour {

    public float mouseSensitivity = 4;
    public float moveSpeed = 0.1f;
    public Camera cam;
    GameObject projectile;
    TimeMachinev2 myTimeMachine;
    List<rewindData> playerData = new List<rewindData>();
    int scroll=2;

    public float rewindJuice;

    public Color hudStartColor, hudEndColour;
    public Slider slider;
    public Image sliderFill;

    // Use this for initialization
    void Start()
    {
        rewindJuice = slider.maxValue;
        Cursor.visible = false;
        projectile = Resources.Load("Sphere") as GameObject;
        myTimeMachine = GameObject.FindGameObjectWithTag("TimeMachine").GetComponent<TimeMachinev2>();
    }
	
	// Update is called once per frame
	void Update()
    {
        updatePlayerData();
        float rotX = CrossPlatformInputManager.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = CrossPlatformInputManager.GetAxis("Mouse Y") * mouseSensitivity;

        float posX=0, posZ=0;

        #region movement

        if (Input.GetKey(KeyCode.W))
        {
            posZ += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            posZ -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            posX -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            posX += 1;
        }
        
        #region jetpack
        if (Input.GetKey(KeyCode.Space))
        {
            //if (gameObject.GetComponent<Rigidbody>().velocity.y < 0.01)            
            //gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 35, 0), ForceMode.Impulse);

            gameObject.transform.Translate(0, 0.1f, 0);
            gameObject.GetComponent<Rigidbody>().useGravity = false;

        }
        #endregion
        else
            gameObject.GetComponent<Rigidbody>().useGravity = true;

        gameObject.transform.Translate(posX*moveSpeed, 0, posZ*moveSpeed);
        gameObject.transform.localRotation *= Quaternion.Euler(0f, rotX, 0f);
        cam.transform.rotation *= Quaternion.Euler(-rotY, 0f, 0f);
        #endregion

        #region right click rewind
        if (Input.GetMouseButtonDown(1))
        {
            if (rewindJuice > 10)
            {
                rewindJuice -= 10;
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity))
                {
                    if (hit.collider.tag == "Phys")
                    {
                        if (myTimeMachine != null)
                        {
                            for (int j = 0; j < myTimeMachine.timelines.Count; j++)
                            {
                                if (myTimeMachine.timelines[j].id == hit.transform.gameObject.name)
                                {
                                    if (!myTimeMachine.timelines[j].rewind)
                                    {
                                        myTimeMachine.timelines[j].rewind = true;
                                        myTimeMachine.timelines[j].scroller = myTimeMachine.scroll - 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Time.timeScale = 0.25f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Time.timeScale = 1.0f;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.visible = false;

        if (Input.GetMouseButtonDown(0))
        {
            if (rewindJuice > 50)
            {
                rewindJuice -= 50;
                GameObject shootme = Instantiate(projectile) as GameObject;
                shootme.transform.position = transform.position + cam.transform.forward * 5;
                shootme.GetComponent<Rigidbody>().velocity = cam.transform.forward * 25;
            }
        }

        #region player rewind

        if (Input.GetKeyDown(KeyCode.V))
        {
            scroll = 0;
        }

        if (Input.GetKey(KeyCode.V))
        {
            if (rewindJuice > 0)
            {
                scroll++;
                scroll++;
                rewindJuice--;

                if (scroll > playerData.Count)
                {
                    scroll = playerData.Count;
                    rewindJuice++;
                }
                transform.position = playerData[playerData.Count - scroll].position;
                GetComponent<Rigidbody>().angularVelocity = playerData[playerData.Count - scroll].AngularVelocity;
                GetComponent<Rigidbody>().velocity = playerData[playerData.Count - scroll].velocity;
            }
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            if (scroll > 2)
                playerData.RemoveRange(playerData.Count - scroll, scroll - 1);
        }
        if (rewindJuice > slider.maxValue)
            rewindJuice = slider.maxValue;

        slider.value = rewindJuice;
        sliderFill.color = Color.Lerp(hudEndColour, hudStartColor, rewindJuice / slider.maxValue);

        #endregion
    }

    public struct rewindData
    {
        public Vector3 position;
        public Vector3 AngularVelocity;
        public Vector3 velocity;
    }

    void updatePlayerData()
    {
        rewindData sample;
        Rigidbody rigidComp = GetComponent<Rigidbody>();
        if (rigidComp != null)
        {
            sample.AngularVelocity = rigidComp.angularVelocity;
            sample.velocity = rigidComp.velocity;
        }
        else
        {
            sample.AngularVelocity = Vector3.zero;
            sample.velocity = Vector3.zero;
        }

        sample.position = transform.position;

        playerData.Add(sample);
    }

    public void lockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
