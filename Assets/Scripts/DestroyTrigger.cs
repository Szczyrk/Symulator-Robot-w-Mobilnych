using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrigger : MonoBehaviour
{
    Robot last;
    private void OnTriggerEnter(Collider other)
    {
        if (last.gameObject != other.GetComponentInParent<Robot>().gameObject)
        {
            last = other.GetComponentInParent<Robot>();
            Debug.Log("Destroy " + last.gameObject);
            Destroy(last.gameObject);
            Simulation.robots.Remove(last);
        }
   }

}
