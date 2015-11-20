using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class FPSmovement : MonoBehaviour {

    public float mouseSensitivity = 4;
    public float moveSpeed = 0.1f;
    public Camera cam;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update()
    {
        float rotX = CrossPlatformInputManager.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = CrossPlatformInputManager.GetAxis("Mouse Y") * mouseSensitivity;

        float posX=0, posZ=0;

        if (Input.GetKey(KeyCode.W))
        {
            posX += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            posX -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            posZ += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            posZ -= 1;
        }
        gameObject.transform.Translate(posX*moveSpeed, 0, posZ*moveSpeed);
        gameObject.transform.localRotation *= Quaternion.Euler(0f, rotX, 0f);
        cam.transform.localRotation *= Quaternion.Euler(-rotY, 0f, 0f);
    }
}
