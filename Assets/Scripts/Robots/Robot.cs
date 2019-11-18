using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TextVariablesRobot))]
public class Robot : MonoBehaviour, IObjectDimensions {


	public bool 		manualControl;
	public bool 		moveEnabled;
	public float		stopDistance;	
	public Transform 	destination;	
	public Vector3 		centerOfMassOffset;
    public string nameWithoutSpace;

    Rigidbody rigidbody;
    public MotorToWheel[] 	motors;
    public Sensor[] sensors;
	private Vector3 	_size;
    private MethodInfo[] methods;
    public TextVariablesRobot textVariables;
    internal bool reload;
    public Vector3 startPostion;
    public Quaternion startRotation;

    public Vector3 position {
		get{ return transform.position; }
		set{ transform.position = value; }
	}
	

	public Transform cameraMount { get; private set; }

	public Bounds bounds {
		get {
			return new Bounds(transform.position, _size);
		}
	}
	
	public bool atDestination {
		get {
			if (!destination) return false;
			float distance = Vector3.Distance(transform.position, destination.position);
			if (distance < stopDistance) return true;
			return false;
		}
	}

	
	public void Reset() {
        transform.position = startPostion;
        transform.rotation = startRotation;
        foreach (Sensor s in sensors) {
			s.Disable();
		}
        
        if (rigidbody != null)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void UpdateEquipment()
    {
        motors = GetComponentsInChildren<MotorToWheel>();
        sensors = GetComponentsInChildren<Sensor>();
    }


    private void Start()
    {
        startPostion = transform.position;
        startRotation = transform.rotation;
        motors = GetComponentsInChildren<MotorToWheel>();
        sensors = GetComponentsInChildren<Sensor>();
        rigidbody = GetComponent<Rigidbody>();
        Simulation.robots.Add(this);
        textVariables = GetComponent<TextVariablesRobot>();
        nameWithoutSpace = this.name.Replace(" ", "_");
        nameWithoutSpace = nameWithoutSpace.Replace("(", "");
        nameWithoutSpace = nameWithoutSpace.Replace(")", "");
        nameWithoutSpace = nameWithoutSpace.Replace(".", "");
        nameWithoutSpace = nameWithoutSpace.Replace(",", "");
        nameWithoutSpace = nameWithoutSpace.Replace("/", "");
        nameWithoutSpace = nameWithoutSpace.Replace("{", "");
        nameWithoutSpace = nameWithoutSpace.Replace("}", "");
        nameWithoutSpace = "Robot_" + nameWithoutSpace;
    }

    public void initializationCode()
    {
        var DLL = Assembly.LoadFrom(nameWithoutSpace+".dll");
        Type type = DLL.GetType(nameWithoutSpace);
        methods = type.GetMethods();
        Assembly.UnsafeLoadFrom(nameWithoutSpace + ".dll");

    }
    public void StartSensor()
    {
        foreach (Sensor sensor in sensors)
            sensor.Enable();
    }

	
	private void Awake() {

		cameraMount = transform.Find("Main Camera");
		if (!cameraMount) {
			cameraMount = transform;
			Debug.LogWarning("CameraMount object not found on Robot.");
		} 

		Bounds b = new Bounds(Vector3.zero, Vector3.zero);
		foreach(Renderer r in GetComponentsInChildren<Renderer>())
			b.Encapsulate(r.bounds);
		_size = b.size;

		GetComponent<Rigidbody>().centerOfMass += centerOfMassOffset;
	}


	private void Update() {

        if (manualControl) {
			float x = Input.GetAxis("X");
			float y = Input.GetAxis("Y");
			float z = Input.GetAxis("Z");	

		}
		else if (Simulation.isRunning){

            //Code();
            methods[0].Invoke(this,new object[] { this});

            foreach (MotorToWheel motor in motors)
            {
                motor.Run();
            }
        }
        if(reload)
        {
            AutomaticEditPanel_UI.Start_UI();
            reload = false;
        }
    }

    private void Code()
    {
        Robot robot = this;
        if (robot.sensors[0].detected && robot.sensors[1].detected)
        {
            robot.motors[0].powerMotor = 20;
            robot.motors[1].powerMotor = 20;
        }
        else
        {
            robot.motors[0].powerMotor = 20;
            robot.motors[1].powerMotor = -20;
        }
    }
    public void UnLoadCode()
    {
        Assembly.UnsafeLoadFrom(nameWithoutSpace + ".dll");
    }
    private void OnDrawGizmos() {

		Gizmos.color = Color.Lerp(Color.yellow, Color.clear, 0.25f);
		if (Application.isPlaying) {
			Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass, 0.05f);
		} else {
			Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass + centerOfMassOffset, 0.05f);
		}
		
	}
	
    internal void ShowVariables()
    {
        textVariables.UpdateVariables();
        textVariables.UpdateCode();
    }
}
