using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class timeBomb : MonoBehaviour
{
    bool hasCollided = false;
    Vector3 hitPosition;
    List<GameObject> collidedObjects = new List<GameObject>();
    public float expireTime = 5;
    TimeMachinev2 myTimeMachine;
    float timer;
    bool scaleMe;
    float startScale, addScale = 0.0f;

    void Start()
    {
        myTimeMachine = GameObject.FindGameObjectWithTag("TimeMachine").GetComponent<TimeMachinev2>();
    }

    void Update()
    {
        timer += 1.0f * Time.deltaTime;
        if (timer > expireTime)
        {
            for (int i = 0; i < collidedObjects.Count; i++)
            {
                for (int j = 0; j < myTimeMachine.timelines.Count; j++)
                {
                    if (myTimeMachine.timelines[j].id == collidedObjects[i].name)
                        myTimeMachine.timelines[j].rewind = false;
                }
            }
            Destroy(gameObject);
        }

        if (hasCollided)
        {
            if (!myTimeMachine.sticky)
            {
                for (int i = 0; i < collidedObjects.Count; i++)
                {
                    Physics.IgnoreCollision(collidedObjects[i].GetComponent<Collider>(), GetComponent<Collider>());
                }
                if (transform.position.y < 1)
                    transform.Translate(0, 1, 0);
            }
            else
            {
                transform.position = hitPosition;
            }

        }

        if (scaleMe && transform.localScale.x < 2.5)
        {
            addScale += 0.1f;
            transform.localScale = new Vector3(startScale + addScale, startScale+ addScale, startScale + addScale);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Phys"))
        {
            collidedObjects.Add(collision.gameObject);
        }

        if (!scaleMe)
        {
            scaleMe = true;
            startScale = transform.localScale.x;
        }

        gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha", 0);
        hasCollided = true;

        if (hitPosition == Vector3.zero)
            hitPosition = transform.position;

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
