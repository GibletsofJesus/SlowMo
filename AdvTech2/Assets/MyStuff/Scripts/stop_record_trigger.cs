using UnityEngine;
using System.Collections;

public class stop_record_trigger : MonoBehaviour {

    bool hiThereFirstTimeHere= true;

	void OnTriggerEnter(Collider col)
    {
        if (hiThereFirstTimeHere)
        {
            Invoke("wooo", 6);
           
            hiThereFirstTimeHere = false;
        }
    }

    void wooo()
    {
        GameObject timemachin = GameObject.FindGameObjectWithTag("TimeMachine");
        timemachin.GetComponent<TimeMachinev2>().recording = false;
    }
}
