using UnityEngine;


abstract public class Sensor : MonoBehaviour {

    public bool detected;
    public Vector3 direction;

    abstract public void Enable();

	abstract public void Disable();
}
