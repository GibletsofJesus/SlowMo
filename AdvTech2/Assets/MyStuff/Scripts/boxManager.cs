using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class boxManager : MonoBehaviour {

    public GameObject[] boxes;
    List<Vector3> pos = new List<Vector3>();
    List<Quaternion> rot = new List<Quaternion>();

    List<List<Vector3>> posArray = new List<List<Vector3>>();
    List<List<Quaternion>> rotArray = new List<List<Quaternion>>();

    int scroll = 0;
    bool pause;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            /*pos.Clear();
            rot.Clear();
            for (int i = 0; i < boxes.Length; i++)
            {
                pos.Add(boxes[i].transform.position);
                rot.Add(boxes[i].transform.rotation);
            }*/
            scroll = posArray.Count;
            pause = !pause;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            scroll++;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            scroll--;
        }

        if (pause)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].transform.position = posArray[scroll-1][i];
                //boxes[i].transform.position = pos[i];
                boxes[i].transform.rotation = rotArray[scroll-1][i];
                //boxes[i].transform.rotation = rot[i];
            }
        }
        else
        {
            updateVariables();
        }
        //Add some code here so we're not calling this every frame.
    }

    void updateVariables()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            pos.Add(boxes[i].transform.position);
            rot.Add(boxes[i].transform.rotation);
        }

        if (posArray.Count < 1000)
        {
            posArray.Add(pos);
            rotArray.Add(rot);
        }

        //Grab variables for game objects
        //Store in array


    }
}
