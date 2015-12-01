using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TimeMachinev2 : MonoBehaviour {
    
    public float updateTime = 0.01f;
    public float rewindDuration = 5.0f;
    public Text recordingText;
    public Text stickyText;

    public bool resumeWhenFinishedRewinding;

    [HideInInspector]
    public int scroll = 0;
    bool recording = true;
    public int rewindSpeed = 2;

    public bool sticky; //Toggle sticky bombs

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
        //audio
        public float timesample;
        public bool isPlaying;

        //physics
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
        if (Input.GetKeyDown(KeyCode.B))
        {
            sticky = !sticky;
            if (sticky)
            {
                stickyText.text = "Enabled";
                stickyText.color = Color.green;
            }
            else
            {
                stickyText.text = "Disabled";
                stickyText.color = Color.red;
            }
        }

        recordingText.enabled = recording;

        #region reset function
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Reset all objects, delete rewind data
            for (int i = 0; i < timelines.Count; i++)
            {
                //Set colour
                rewindableObjects[i].transform.position = timelines[i].timeData[2].position;
                rewindableObjects[i].transform.rotation = timelines[i].timeData[2].rotation;

                Rigidbody rigidComp = rewindableObjects[i].GetComponent<Rigidbody>();
                if (rigidComp != null)
                {
                    rigidComp.angularVelocity = timelines[i].timeData[2].AngularVelocity;
                    rigidComp.velocity = timelines[i].timeData[2].velocity;
                }

                AudioSource audioComp = rewindableObjects[i].GetComponent<AudioSource>();
                if (audioComp != null)
                {
                    audioComp.pitch = 1 * Time.timeScale;
                    if (!audioComp.isPlaying && timelines[i].scroller > 2)
                    {
                        if (timelines[i].timeData[2].isPlaying)
                        {
                            audioComp.time = timelines[i].timeData[2].timesample;
                            audioComp.Play();
                        }
                    }
                }
                recording = true;
                timelines[i].scroller = 0;
                rewindableObjects[i].GetComponent<Renderer>().material.color = Color.red;
                timelines[i].rewind = false;
                timelines[i].timeData.Clear();
            }
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.F))
        {
            recording = !recording;
            if (recording)
                StartCoroutine(record());
        }

        rewinder();

    }

    void updateVariables(GameObject input)
    {
        rewindData sample;

        #region audio
        AudioSource audioComp = input.GetComponent<AudioSource>();
        if (audioComp != null)
        {
            sample.isPlaying = audioComp.isPlaying;
            sample.timesample = audioComp.time;
        }
        else
        {
            sample.isPlaying = false;
            sample.timesample = 0;
        }
        #endregion

        #region physics
        Rigidbody rigidComp = input.GetComponent<Rigidbody>();
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
        #endregion

        sample.position = input.transform.position;
        sample.rotation = input.transform.rotation;

        for (int i = 0; i < timelines.Count; i++)
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
                //Set colour
                rewindableObjects[i].GetComponent<Renderer>().material.color = Color.cyan;

                //Rewind a little
                for (int l = 0; l < rewindSpeed; l++)
                {
                    timelines[i].scroller--;
                    if (timelines[i].scroller * updateTime > rewindDuration)
                    {
                        timelines[i].rewind = false;
                        if (timelines[i].scroller < timelines[1].timeData.Count - (rewindDuration / updateTime))
                        {
                            timelines[i].rewind = false;
                            //timelines[i].scroller++;
                        }
                    }
                }
                if (timelines[i].scroller < 2)
                {
                    timelines[i].scroller = 2;
                    if (resumeWhenFinishedRewinding)
                        timelines[i].rewind = false;
                }
                #region Set values
                rewindableObjects[i].transform.position = timelines[i].timeData[timelines[i].scroller - 1].position;
                rewindableObjects[i].transform.rotation = timelines[i].timeData[timelines[i].scroller - 1].rotation;

                Rigidbody rigidComp = rewindableObjects[i].GetComponent<Rigidbody>();
                if (rigidComp != null)
                {
                    rigidComp.angularVelocity = timelines[i].timeData[timelines[i].scroller - 1].AngularVelocity;
                    rigidComp.velocity = timelines[i].timeData[timelines[i].scroller - 1].velocity;
                }

                AudioSource audioComp = rewindableObjects[i].GetComponent<AudioSource>();
                if (audioComp != null)
                {
                    audioComp.pitch = rewindSpeed * -1 * Time.timeScale;
                    if (!audioComp.isPlaying && timelines[i].scroller > 2)
                    {
                        if (timelines[i].timeData[timelines[i].scroller-1].isPlaying)
                        {
                            audioComp.time = timelines[i].timeData[timelines[i].scroller - 1].timesample;
                            audioComp.Play();
                        }
                    }
                }
                #endregion
            }
            else
            {
                rewindableObjects[i].GetComponent<Renderer>().material.color = Color.red;
                AudioSource normalAudio = rewindableObjects[i].GetComponent<AudioSource>();
                if (normalAudio != null)
                {
                    normalAudio.pitch = 1 * Time.timeScale;
                }
            }
        }
    }
    
}