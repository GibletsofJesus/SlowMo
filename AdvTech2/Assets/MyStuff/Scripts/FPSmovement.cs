using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class FPSmovement : MonoBehaviour {

    public float mouseSensitivity = 4;
    public float moveSpeed = 0.1f;
    public Camera cam;
    GameObject projectile;
    TimeMachinev2 myTimeMachine;
    public bool allowJetpack;
    List<rewindData> playerData = new List<rewindData>();
    List<ghostData> SpookyGhostData = new List<ghostData>();
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
        
        gameObject.transform.Translate(posX*moveSpeed, 0, posZ*moveSpeed);
        gameObject.transform.localRotation *= Quaternion.Euler(0f, rotX, 0f);
        cam.transform.rotation *= Quaternion.Euler(-rotY, 0f, 0f);
        #endregion

        #region jetpack
        if (Input.GetKey(KeyCode.Space))
        {
            //if (gameObject.GetComponent<Rigidbody>().velocity.y < 0.01)            
            //gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 35, 0), ForceMode.Impulse);

            if (allowJetpack)
            {
                gameObject.transform.Translate(0, 0.1f, 0);
                gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
        }
        else
            gameObject.GetComponent<Rigidbody>().useGravity = true;
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

        #region shitty slowmo
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Time.timeScale = 0.25f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Time.timeScale = 1.0f;
        }
        #endregion

        #region mouse things
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
        #endregion

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
        #endregion

        if (rewindJuice > slider.maxValue)
            rewindJuice = slider.maxValue;

        slider.value = rewindJuice;
        sliderFill.color = Color.Lerp(hudEndColour, hudStartColor, rewindJuice / slider.maxValue);

    }

    #region spookey ghost things

    public void saveToFile()
    {
        var time = System.DateTime.Now;
        using (StreamWriter sw = new StreamWriter("Ghost "+time.Year + "-" + time.Month + "-" + time.Day + " " + time.Hour + time.Minute+".ghost"))
        {
            for (int i = 0; i < SpookyGhostData.Count; i++)
            {
                sw.Write(SpookyGhostData[i].position);
                sw.WriteLine(" R " + SpookyGhostData[i].rotation);
            }
        }
        
       /* var filesToRead = Directory.GetFiles("Ghost*.ghost");
        Debug.Log(filesToRead);*/
    }

    public void textReader()
    {
        using (StreamReader sr = new StreamReader("Ghost"))
        {
            List<Vector3> pos = new List<Vector3>();
            List<Quaternion> rot = new List<Quaternion>();
            while (!sr.EndOfStream)
            {
                ghostData boo;
                string line = sr.ReadLine();
                line = line.Replace("(", "");
                line = line.Replace(")", "");
                line = line.Replace(" ", "");
                string[] splitString = line.Split('R');

                string[] mo = splitString[0].Split(',');
                var x = float.Parse(mo[0]);
                var y = float.Parse(mo[1]);
                var z = float.Parse(mo[2]);
                pos.Add(new Vector3(x,y,z));

                string[] bo = splitString[1].Split(',');
                var r1 = float.Parse(bo[0]);
                var r2 = float.Parse(bo[1]);
                var r3 = float.Parse(bo[2]);
                var r4 = float.Parse(bo[3]);
                rot.Add(new Quaternion(r1,r2,r3,r4));
            }
            GameObject MrSpooks = Instantiate(Resources.Load("ghost") as GameObject) as GameObject;
            MrSpooks.GetComponent<GhostScript>().sup(pos, rot);
        }
    }

    #endregion

    public struct ghostData
    {
        public Vector3 position;
        public Quaternion rotation;
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
        ghostData boo;//!

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

        boo.position = transform.position;
        boo.rotation = transform.rotation;
        SpookyGhostData.Add(boo);
    }

    public void lockMouse()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
