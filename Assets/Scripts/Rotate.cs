using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis { AxisX, AxisY, AxisZ}

public class Rotate : MonoBehaviour {


    public bool Rotating { get; set; }

    [SerializeField]
    private Axis axis;
    [SerializeField]
    private float degreesPerSecond;

    private Transform myTransform;
    private Vector3 rotationVector;

    private void Awake()
    {
        Rotating = true;

        //Default z Axis
        switch (axis)
        {
            case Axis.AxisX:
                rotationVector = new Vector3(degreesPerSecond, 0, 0);
                break;
            case Axis.AxisY:
                rotationVector = new Vector3(0, degreesPerSecond, 0);
                break;
            case Axis.AxisZ:
                rotationVector = new Vector3(0, 0, degreesPerSecond);
                break;
            default:
                rotationVector = new Vector3(0, 0, degreesPerSecond);
                break;

        }
        
    }

    // Update is called once per frame
    void Update () {

        transform.Rotate(rotationVector * Time.deltaTime);
        
    }
}
