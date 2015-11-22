using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeMachinev2 : MonoBehaviour {
    
    public float updateTime = 0.01f;
    public float rewindDuration = 5.0f;

    int rewindSpeed = -1;
    
    List<GameObject> rewindableObjects = new List<GameObject>();
    public List<timeline> timelines = new List<timeline>();
    
    public int scroll = 0;
    float tick = 0;
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
        while (rewindSpeed == -1)
        {
            for (int i = 0; i < rewindableObjects.Count; i++)
            {
                updateVariables(rewindableObjects[i]);
            }
            yield return new WaitForSeconds(updateTime);
        }
    }

    public struct rewindData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 AngularVelocity;
        public Vector3 velocity;
        //public float timeTick;
    }

    public class timeline
    {
        public List<rewindData> timeData = new List<rewindData>();
        public string id;
        public bool rewind;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F))
            rewindSpeed = 1;
            
        rewinder();
        tick++;
        Debug.Log(scroll);
    }

    void updateVariables(GameObject input)
    {
        rewindData sample;
        sample.position = input.transform.position;
        sample.rotation = input.transform.rotation;
        sample.AngularVelocity = input.GetComponent<Rigidbody>().angularVelocity;
        sample.velocity = input.GetComponent<Rigidbody>().velocity;
        //sample.timeTick = tick;
        for (int i=0; i < timelines.Count; i++)
        {
            if (timelines[i].id == input.name)
                timelines[i].timeData.Add(sample);
        }
    }

    void rewinder()
    {
        for (int l = 0; l < rewindSpeed; l++)
        {
            scroll--;
            if (scroll * updateTime > rewindDuration)
            {
                if (scroll < timelines[1].timeData.Count- (rewindDuration / updateTime))
                {
                    scroll++;
                }
            }
        }
        if (scroll < 2)
            scroll = 2;

        for (int i = 0; i < timelines.Count; i++)
        {
            if (timelines[i].rewind)
            {
                rewindableObjects[i].transform.position = timelines[i].timeData[scroll - 1].position;
                rewindableObjects[i].transform.rotation = timelines[i].timeData[scroll - 1].rotation;
                rewindableObjects[i].GetComponent<Rigidbody>().angularVelocity = timelines[i].timeData[scroll - 1].AngularVelocity;
                rewindableObjects[i].GetComponent<Rigidbody>().velocity = timelines[i].timeData[scroll - 1].velocity;
            }
        }
    }
    
}