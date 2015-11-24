using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeTrampoline : MonoBehaviour
{
    List<GameObject> collidedObjects = new List<GameObject>();
    public Material rewindMaterial, barrelMaterial;
    //public float expireTime;
    TimeMachinev2 myTimeMachine;
    float timer;

    void Start()
    {
        myTimeMachine = GameObject.FindGameObjectWithTag("TimeMachine").GetComponent<TimeMachinev2>();
    }

    void Update()
    {
        for (int i = 0; i < collidedObjects.Count; i++)
        {
            for (int j = 0; j < myTimeMachine.timelines.Count; j++)
            {
                if (myTimeMachine.timelines[j].id == collidedObjects[i].name)
                {
                    if (myTimeMachine.timelines[j].rewind)
                    {
                        collidedObjects[i].GetComponent<Renderer>().material = rewindMaterial;
                    }
                    else
                    {
                        collidedObjects[i].GetComponent<Renderer>().material = barrelMaterial;
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Phys"))
            collidedObjects.Add(collision.gameObject);

        for (int i = 0; i < collidedObjects.Count; i++)
        {
            for (int j = 0; j < myTimeMachine.timelines.Count; j++)
            {
                if (myTimeMachine.timelines[j].id == collidedObjects[i].name)
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
