using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTrigger : MonoBehaviour
{
    Robot last = null;
    private void OnTriggerEnter(Collider other)
    {
        if(last == null)
            last = other.GetComponentInParent<Robot>();
        else
        if (last.gameObject != other.GetComponentInParent<Robot>().gameObject)
        {
            last = other.GetComponentInParent<Robot>();
            Debug.Log("Stop " + last.gameObject.name);
            last.transform.GetComponent<Rigidbody>().isKinematic = true;
        }
   }

}
