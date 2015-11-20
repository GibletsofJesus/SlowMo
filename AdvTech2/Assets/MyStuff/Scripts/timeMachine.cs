using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class timeMachine : MonoBehaviour
{

    public GameObject[] boxes;
    public Text lengthText, UiRewindSpeed;
    public Image GuiImage;
    public Sprite play, pause, stop, fwd, back;
    public Slider SliderObject;
    public float updateTime = 0.01f;
    public float rewindDuration = 5.0f;

    int rewindSpeed=-1;

    List<Vector3> pos = new List<Vector3>();
    List<Quaternion> rot = new List<Quaternion>();
    List<Vector3> angVel = new List<Vector3>();
    List<Vector3> vel = new List<Vector3>();
    List<float> playbackTime = new List<float>();

    List<GameObject> physicsObjects = new List<GameObject>();
    List<GameObject> animatedObjects = new List<GameObject>();

    List<List<Vector3>> posArray = new List<List<Vector3>>();
    List<List<Quaternion>> rotArray = new List<List<Quaternion>>();
    List<List<Vector3>> angVelArray = new List<List<Vector3>>();
    List<List<Vector3>> velArray = new List<List<Vector3>>();
    List<List<float>> playbackTimeArray = new List<List<float>>();

    int scroll = 0;
    bool isPaused, rewind;
    float tick = 0;
    // Use this for initialization
    void Start()
    {
        //Start Couroutine
        StartCoroutine(record());

        //Add objects to lists
        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i].gameObject.CompareTag("Phys"))
            {
                physicsObjects.Add(boxes[i]);
            }
            else if (boxes[i].gameObject.CompareTag("Animated"))
            {
                animatedObjects.Add(boxes[i]);
            }
        }
    }

    IEnumerator record()
    {
        while (rewindSpeed == -1)
        {
            updateVariables();
            yield return new WaitForSeconds(updateTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region On spacebar;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SliderObject.interactable = true;
            SliderObject.maxValue = posArray.Count;
            SliderObject.value = posArray.Count-1;
            rewind = false;
            scroll = posArray.Count;
            isPaused = !isPaused;

            if (!isPaused)
            {
                GuiImage.sprite = play;
                StartCoroutine(record());
            }

            for (int i = 0; i < animatedObjects.Count; i++)
            {
                Animator anim = animatedObjects[i].GetComponent<Animator>();
                animatedObjects[i].GetComponent<Animator>().StopPlayback();
            }
        }
        #endregion

        #region Rewind function
        if (rewindSpeed > 0)
        {
            GuiImage.sprite = back;
            UiRewindSpeed.text = "x"+rewindSpeed;
            for (int l = 0; l < rewindSpeed; l++)
            {
                scroll--;
                if (scroll * updateTime > rewindDuration)
                {
                    if (scroll < posArray.Count - (rewindDuration / updateTime))
                    {
                        scroll++;
                    }
                }
            }
            if (scroll < 2)
                scroll = 2;

            for (int i = 0; i < physicsObjects.Count; i++)
            {
                physicsObjects[i].transform.position = posArray[scroll - 1][i];
                physicsObjects[i].transform.rotation = rotArray[scroll - 1][i];
                physicsObjects[i].GetComponent<Rigidbody>().angularVelocity = angVelArray[scroll - 1][i];
                physicsObjects[i].GetComponent<Rigidbody>().velocity = velArray[scroll - 1][i];
            }
            for (int i = 0; i < animatedObjects.Count; i++)
            {
                //animatedObjects[i].GetComponent<Animator>().playbackTime = playbackTimeArray[scroll - 1][i];
            }
            lengthText.text = "Current Tick:" + scroll + "\nTotal Ticks " + tick;
        }
        #endregion

        #region On 'r' button;

        if (Input.GetKeyDown(KeyCode.R))
        {
            GuiImage.sprite = back;
            for (int i = 0; i < animatedObjects.Count; i++)
            {
                Animator anim = animatedObjects[i].GetComponent<Animator>();
                float speed = anim.GetFloat("speed");
                speed = speed * -1.0f;
                anim.SetFloat("speed", speed);
            }
            rewind = !rewind;
            scroll = posArray.Count;
            if (!rewind)
            {
                GuiImage.sprite = play;
            }
        }
        #endregion
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (rewindSpeed == -1)
            {
                SliderObject.interactable = true;
                SliderObject.maxValue = posArray.Count;
                SliderObject.value = posArray.Count - 1;
                scroll = posArray.Count;
            }

            if (rewindSpeed > 0)
                rewindSpeed = rewindSpeed * 2;
            else
                rewindSpeed++;
            Debug.Log(rewindSpeed);
        }
    
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (rewindSpeed == 0)
            {
                rewindSpeed--;
                GuiImage.sprite = play;
                StartCoroutine(record());
            }
            else if(rewindSpeed > 1)
            {
                rewindSpeed = rewindSpeed / 2;
            }
            else
            {
                SliderObject.interactable = true;
                SliderObject.maxValue = posArray.Count;
                SliderObject.value = posArray.Count - 1;
                rewindSpeed--;
                if (rewindSpeed < -1)
                    rewindSpeed = -1;
            }
            Debug.Log(rewindSpeed);
        }

        #region If Paused
        if (rewindSpeed == 0)
        {
            StopCoroutine(record());
            GuiImage.sprite = pause;
            UiRewindSpeed.text = "";
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                physicsObjects[i].transform.position = posArray[scroll - 1][i];
                physicsObjects[i].transform.rotation = rotArray[scroll - 1][i];
                physicsObjects[i].GetComponent<Rigidbody>().angularVelocity = angVelArray[scroll - 1][i];
                physicsObjects[i].GetComponent<Rigidbody>().velocity = velArray[scroll - 1][i];
            }
            for (int i = 0; i < animatedObjects.Count; i++)
            {
                //animatedObjects[i].GetComponent<Animator>().playbackTime = playbackTimeArray[scroll - 1][i];
            }
        }
        else
        {
            SliderObject.interactable = false;
        }
        #endregion
    }

    void updateVariables()
    {
        pos.Clear();
        rot.Clear();
        angVel.Clear();
        vel.Clear();
        playbackTime.Clear();
        for (int i = 0; i < physicsObjects.Count; i++)
        {
            pos.Add(physicsObjects[i].transform.position);
            rot.Add(physicsObjects[i].transform.rotation);
            angVel.Add(physicsObjects[i].GetComponent<Rigidbody>().angularVelocity);
            vel.Add(physicsObjects[i].GetComponent<Rigidbody>().velocity);
        }
        for (int i = 0; i < animatedObjects.Count; i++)
        {
            playbackTime.Add(animatedObjects[i].GetComponent<Animator>().playbackTime);
        }

        posArray.Add(new List<Vector3>(pos));
        rotArray.Add(new List<Quaternion>(rot));
        angVelArray.Add(new List<Vector3>(angVel));
        velArray.Add(new List<Vector3>(vel));
        playbackTimeArray.Add(new List<float>(playbackTime));
        lengthText.text = "Current Tick:" + scroll + "\nTotal Ticks " + tick;
        //scroll++;
        tick++;

        //ticks * update time = time in seconds we've been recording
        if (tick*updateTime > rewindDuration)
        {

        }

    }

    public void setScroll(float newscroll)
    {
        scroll = (int)newscroll;
        lengthText.text = "Current Tick:" + scroll + "\nTotal Ticks " + tick;
    }
}