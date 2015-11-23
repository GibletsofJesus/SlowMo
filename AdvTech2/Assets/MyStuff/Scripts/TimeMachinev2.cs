using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TimeMachinev2 : MonoBehaviour {
    
    public float updateTime = 0.01f;
    public float rewindDuration = 5.0f;
    public Text recordingText;

    [HideInInspector]
    public int scroll = 0;
    bool recording = true;
    int rewindSpeed = -1;
    public int rewindSpeed2 = 2;
    
    List<GameObject> rewindableObjects = new List<GameObject>();
    public List<timeline> timelines = new List<timeline>();
    
    // Use this for initialization
    void Start()
    {
        //Start Couroutine
        StartCoroutine(record());

        //Add objects to lists
        rewindableObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Phys"));
        for (int i = 0; i < rewindableObjects.Count; i++)
        {
            //make a new timeline
            timeline thisOne = new timeline();
            thisOne.id = rewindableObjects[i].name;
            timelines.Add(thisOne);
        }
    }

    IEnumerator record()
    {
        while (recording)
        {
            for (int i = 0; i < rewindableObjects.Count; i++)
            {
                updateVariables(rewindableObjects[i]);
            }
            scroll++;
            yield return new WaitForSeconds(updateTime);
        }
    }

    public struct rewindData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 AngularVelocity;
        public Vector3 velocity;
    }

    public class timeline
    {
        public List<rewindData> timeData = new List<rewindData>();
        public string id;
        public bool rewind;
        public int scroller = 0;
    }

    // Update is called once per frame
    void Update()
    {
        recordingText.enabled = recording;

        if (Input.GetKeyDown(KeyCode.F))
            recording = !recording;
            
        rewinder();
    }

    void updateVariables(GameObject input)
    {
        rewindData sample;
        sample.position = input.transform.position;
        sample.rotation = input.transform.rotation;
        sample.AngularVelocity = input.GetComponent<Rigidbody>().angularVelocity;
        sample.velocity = input.GetComponent<Rigidbody>().velocity;
        for (int i=0; i < timelines.Count; i++)
        {
            if (timelines[i].id == input.name)
                timelines[i].timeData.Add(sample);
        }
    }

    void rewinder()
    {
        for (int i = 0; i < timelines.Count; i++)
        {
            if (timelines[i].rewind)
            {
                for (int l = 0; l < rewindSpeed2; l++)
                {
                    timelines[i].scroller--;
                    if (timelines[i].scroller * updateTime > rewindDuration)
                    {
                        if (timelines[i].scroller < timelines[1].timeData.Count - (rewindDuration / updateTime))
                        {
                            timelines[i].rewind = false;
                            //timelines[i].scroller++;
                        }
                    }
                }
                if (timelines[i].scroller < 2)
                    timelines[i].scroller = 2;
                rewindableObjects[i].transform.position = timelines[i].timeData[timelines[i].scroller - 1].position;
                rewindableObjects[i].transform.rotation = timelines[i].timeData[timelines[i].scroller - 1].rotation;
                rewindableObjects[i].GetComponent<Rigidbody>().angularVelocity = timelines[i].timeData[timelines[i].scroller - 1].AngularVelocity;
                rewindableObjects[i].GetComponent<Rigidbody>().velocity = timelines[i].timeData[timelines[i].scroller - 1].velocity;
            }
        }
    }
    
}