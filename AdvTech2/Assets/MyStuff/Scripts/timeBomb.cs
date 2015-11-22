using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class timeBomb : MonoBehaviour
{
    bool hasCollided = false;
    Vector3 hitPosition;
    List<GameObject> collidedObjects = new List<GameObject>();
    public Color highLightColour;
    public float expireTime;
    //public GameObject timeMachine;
    TimeMachinev2 myTimeMachine;
    float timer;

    void Start()
    {
        myTimeMachine = GameObject.FindGameObjectWithTag("TimeMachine").GetComponent<TimeMachinev2>();
    }

    void Update()
    {
        timer += 1.0f * Time.deltaTime;
        if (timer > expireTime)
        {
            Destroy(gameObject);
            for (int i = 0; i < collidedObjects.Count; i++)
            {
                for (int j = 0; j < myTimeMachine.timelines.Count; j++)
                {
                    if (myTimeMachine.timelines[i].id == collidedObjects[i].name)
                        myTimeMachine.timelines[i].rewind = true;
                }
            }
        }

            if (hasCollided)
        {
            transform.position = hitPosition;
        }

        for (int i = 0; i < collidedObjects.Count; i++)
        {
            for(int j=0; j < myTimeMachine.timelines.Count; j++)
            {
                if (myTimeMachine.timelines[j].id == collidedObjects[i].name)
                    myTimeMachine.timelines[j].rewind = true;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Phys"))
        {
            collidedObjects.Add(collision.gameObject);
            collision.gameObject.GetComponent<Renderer>().material.color = highLightColour;
            myTimeMachine.scroll = myTimeMachine.timelines[1].timeData.Count;
        }

        gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha", 0);
        hasCollided = true;
        transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        if (hitPosition == Vector3.zero)
            hitPosition = transform.position;
        
    }
}
