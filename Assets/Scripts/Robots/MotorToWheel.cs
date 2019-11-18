using UnityEngine;
using System.Collections;
using System;
using TMPro;
[RequireComponent(typeof(WheelCollider))]
public class MotorToWheel : MonoBehaviour {

    public bool drawDebug = false;
    public float maxWheelRpm = 57f;
	public float maxWheelTorque = 1.8f;
	public float Kp = 0.5f;
	
	public WheelCollider wheelCollider;
    public float powerMotor;
    private float torque;

    private void UpdateWheelPose()
    {
        Vector3 _pos = transform.position;
        Quaternion _quat = transform.rotation;

        wheelCollider.GetWorldPose(out _pos, out _quat);

        transform.position = _pos;
        transform.rotation = _quat;
    }


    void Start() {
        wheelCollider = transform.GetComponent<WheelCollider>();
        JointSpring suspensionSpring = wheelCollider.suspensionSpring;
        suspensionSpring.spring = 0;
        suspensionSpring.damper = 0;
        suspensionSpring.targetPosition = 0;
        wheelCollider.suspensionSpring = suspensionSpring;
        wheelCollider.mass = 8;
        wheelCollider.wheelDampingRate = 0.025f;
        wheelCollider.suspensionDistance = 0;
    }


  public void Run()
    {
        //UpdateWheelPose();
        torque = (powerMotor * maxWheelRpm - wheelCollider.rpm) * Kp;
        wheelCollider.motorTorque = Mathf.Clamp(torque, -maxWheelTorque, maxWheelTorque);

        if(drawDebug)
        Debug.DrawRay(wheelCollider.transform.position, transform.forward * powerMotor * 0.1f, Color.magenta);
    }

}
