using UnityEngine;
using System.Collections;

public class spin : MonoBehaviour {

    public float moveSpeed;
    public float juiceBonus;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(new Vector3(0,50,0) * Time.deltaTime * moveSpeed);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<FPSmovement>().rewindJuice += juiceBonus;
            GetComponent<AudioSource>().Play();
            Destroy(gameObject.GetComponent<MeshRenderer>());
            Destroy(gameObject.GetComponent<BoxCollider>());
            Destroy(gameObject, 2);
        }
    }
}
