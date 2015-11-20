using UnityEngine;
using System.Collections;

public class ramMovement : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.Q))
            transform.Translate(Vector3.forward * Time.deltaTime * 10);

        if (Input.GetKey(KeyCode.E))
            transform.Translate(Vector3.back * Time.deltaTime * 10);

    }
}
