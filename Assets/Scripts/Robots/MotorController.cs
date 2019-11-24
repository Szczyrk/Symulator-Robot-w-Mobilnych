using UnityEngine;
using System.Collections;
using System;
using TMPro;
[RequireComponent(typeof(WheelCollider))]
public class MotorController : MonoBehaviour {

    public bool drawDebug = false;
    public float maxWheelRpm = 57.1f;
	public float maxWheelTorque = 1.8f;
	public float Kp = 0.5f;
	
	public WheelCollider wheelCollider;
    public float powerMotor;
    private float torque;
    public Vector3 centerWheel;

    private void UpdateWheelPose()
    {
        Vector3 _pos = transform.position;
        Quaternion _quat = transform.rotation;

        wheelCollider.GetWorldPose(out _pos, out _quat);

        transform.position = _pos;
        transform.rotation = _quat;
    }


    void Awake() {
        wheelCollider = transform.GetComponent<WheelCollider>();
    }


  public void Run()
    {
        torque = (powerMotor * maxWheelRpm - wheelCollider.rpm) * Kp;
        wheelCollider.motorTorque = Mathf.Clamp(torque, -maxWheelTorque, maxWheelTorque);

        if(drawDebug)
        Debug.DrawRay(wheelCollider.transform.position, transform.forward * powerMotor * 0.1f, Color.magenta);
    }

    public void CreatMotor(MeshRenderer newGameObject)
    {
        wheelCollider = transform.GetComponent<WheelCollider>();
        JointSpring suspensionSpring = wheelCollider.suspensionSpring;
        suspensionSpring.spring = 0;
        suspensionSpring.damper = 0;
        suspensionSpring.targetPosition = 0;
        wheelCollider.suspensionSpring = suspensionSpring;
        wheelCollider.mass = 8;
        wheelCollider.wheelDampingRate = 0.025f;
        wheelCollider.suspensionDistance = 0;
        wheelCollider.radius = (newGameObject.bounds.size.x + 0.002f) / 2 / newGameObject.transform.lossyScale.x;
        centerWheel = Rotation((newGameObject.transform.position - newGameObject.bounds.center),newGameObject) / newGameObject.transform.lossyScale.x;
        wheelCollider.center = centerWheel;
    }

    private Vector3 Rotation(Vector3 vector3, MeshRenderer newGameObject)
    {
        Matrix4x4 M = Matrix4x4.TRS(Vector3.one, newGameObject.transform.rotation, Vector3.one);
        return M.inverse * vector3 * -1;
    }
}
