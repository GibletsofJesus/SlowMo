using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class FPSmovement : MonoBehaviour {

    public float mouseSensitivity = 4;
    public float moveSpeed = 0.1f;
    public Camera cam;
    GameObject projectile;
    TimeMachinev2 myTimeMachine;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        projectile = Resources.Load("Sphere") as GameObject;
        myTimeMachine = GameObject.FindGameObjectWithTag("TimeMachine").GetComponent<TimeMachinev2>();
    }
	
	// Update is called once per frame
	void Update()
    {
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
        
        if (Input.GetKey(KeyCode.Space))
        {
            gameObject.transform.Translate(0, 0.1f, 0);
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        else
            gameObject.GetComponent<Rigidbody>().useGravity = true;

        gameObject.transform.Translate(posX*moveSpeed, 0, posZ*moveSpeed);
        gameObject.transform.localRotation *= Quaternion.Euler(0f, rotX, 0f);
        cam.transform.rotation *= Quaternion.Euler(-rotY, 0f, 0f);
        #endregion

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Phys")
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
            GameObject shootme = Instantiate(projectile) as GameObject;
            shootme.transform.position = transform.position + cam.transform.forward*5;
            shootme.GetComponent<Rigidbody>().velocity = cam.transform.forward * 25;
        }

    }

    public void lockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
