using UnityEngine;
using System.Collections;

public class Refill_box : MonoBehaviour
{
    public float FillPerSecond = 250.0f;
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (col.gameObject.GetComponent<FPSmovement>().rewindJuice < col.gameObject.GetComponent<FPSmovement>().slider.maxValue)
            {
                col.gameObject.GetComponent<FPSmovement>().rewindJuice += FillPerSecond;
            }
        }
    }
}
