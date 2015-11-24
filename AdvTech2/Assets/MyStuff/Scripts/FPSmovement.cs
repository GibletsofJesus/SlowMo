using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class FPSmovement : MonoBehaviour {

    public float mouseSensitivity = 4;
    public float moveSpeed = 0.1f;
    public Camera cam;
    GameObject projectile;
    // Use this for initialization
    void Start ()
    {
        projectile = Resources.Load("Sphere") as GameObject;
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
        
        if (Input.GetKey(KeyCode.LeftShift))
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject shootme = Instantiate(projectile) as GameObject;
            shootme.transform.position = transform.position + cam.transform.forward*5;
            shootme.GetComponent<Rigidbody>().velocity = cam.transform.forward * 25;
        }

    }
}
