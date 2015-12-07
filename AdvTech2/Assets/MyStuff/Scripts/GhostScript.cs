using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GhostScript : MonoBehaviour {

    List<Vector3> posList = new List<Vector3>();
    List<Quaternion> rotList = new List<Quaternion>();
    bool play;
    int scroller=0;

    public void sup(List<Vector3> pos, List<Quaternion> rot)
    {
        posList = pos;
        rotList = rot;
        play = true;
    }

    void Update()
    {
        if (play)
        {
            scroller++;
            if (scroller == posList.Count)
            {
                scroller = posList.Count - 1;
            }
            transform.position = posList[scroller];
            transform.rotation = rotList[scroller];
        }
    }
}
