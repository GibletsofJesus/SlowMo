using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class boxManager : MonoBehaviour
{

    public GameObject[] boxes;
    public Text lengthText;
    public Image GuiImage;
    public Sprite play, pause, stop, fwd, back;
    public Slider SliderObject;
    public float updateTime = 0.01f;

    List<Vector3> pos = new List<Vector3>();
    List<Quaternion> rot = new List<Quaternion>();
    List<GameObject> physicsObjects = new List<GameObject>();
    List<GameObject> animatedObjects = new List<GameObject>();

    List<List<Vector3>> posArray = new List<List<Vector3>>();
    List<List<Quaternion>> rotArray = new List<List<Quaternion>>();

    int scroll = 0;
    bool isPaused, rewind;
    float tick = 0;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(record());
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
        while (!isPaused)
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
            //lengthText.text = ""+(tick * updateTime);
            rewind = false;
            scroll = posArray.Count;
            isPaused = !isPaused;

            //phys reset goes here
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                Rigidbody test = physicsObjects[i].GetComponent<Rigidbody>();
                test.velocity = Vector3.zero;
            }
        }
        #endregion

        #region rewind function
        if (rewind)
        {
            GuiImage.sprite = back;
            scroll--;
            if (scroll < 2)
                scroll = 2;
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                physicsObjects[i].transform.position = posArray[scroll-1][i];
                physicsObjects[i].transform.rotation = rotArray[scroll-1][i];
            }
        }
        #endregion

        #region Scrub through function
        if (isPaused)
        {
            GuiImage.sprite = pause;
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                physicsObjects[i].transform.position = posArray[scroll-1][i];
                physicsObjects[i].transform.rotation = rotArray[scroll-1][i];
            }
        }
        else
        {
            SliderObject.interactable = false;
            GuiImage.sprite = play;
        }
        #endregion

        #region On 'r' button;

        if (Input.GetKeyDown(KeyCode.R))
        {

            for (int i = 0; i < animatedObjects.Count; i++)
            {
                Animator anim = animatedObjects[i].GetComponent<Animator>();
                float speed = anim.GetFloat("speed");
                speed = speed * -1.0f;
                anim.SetFloat("speed", speed);
            }
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                Rigidbody test = physicsObjects[i].GetComponent<Rigidbody>();
                test.velocity = Vector3.zero;
            }
            rewind = !rewind;
            isPaused = rewind;
            scroll = posArray.Count;
            if (!rewind)
            {
                StartCoroutine(record());
            }
        }

        #endregion
    }

    void updateVariables()
    {
        pos.Clear();
        rot.Clear();
        for (int i = 0; i < physicsObjects.Count; i++)
        {
            pos.Add(physicsObjects[i].transform.position);
            rot.Add(physicsObjects[i].transform.rotation);
        }
        for (int i = 0; i < animatedObjects.Count; i++)
        {
        }

        posArray.Add(new List<Vector3>(pos));
        rotArray.Add(new List<Quaternion>(rot));
        tick++;
    }

    public void setScroll(float newscroll)
    {
        scroll = (int)newscroll;
    }
}