using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using System.Threading;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TextVariablesRobot))]
public class Robot : MonoBehaviour, IObjectDimensions {


	public bool 		manualControl;
	public bool 		moveEnabled;
	public float		stopDistance;	
	public Transform 	destination;	
	public Vector3 		centerOfMassOffset;
    public string nameWithoutSpace;
    public string code;

    Rigidbody rigidbody;
    public MotorController[] 	motors;
    public Sensor[] sensors;
	private Vector3 	_size;
    private MethodInfo[] methods;
    Thread program;
    public TextVariablesRobot textVariables;
    internal bool reload;
    private Vector3 startPostion;
    private Quaternion startRotation;

    void OnDestroy()
    {
        if(program != null)
            program.Abort();
    }

    public Vector3 position {
		get{ return transform.position; }
		set{ transform.position = value; }
	}
	

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
        
        if (rigidbody != null)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void UpdateEquipment()
    {
        motors = GetComponentsInChildren<MotorController>();
        sensors = GetComponentsInChildren<Sensor>();
    }


    private void Start()
    {
        startPostion = transform.position;
        startRotation = transform.rotation;
        motors = GetComponentsInChildren<MotorController>();
        sensors = GetComponentsInChildren<Sensor>();
        rigidbody = GetComponent<Rigidbody>();
        if(!Simulation.robots.Contains(this))
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
        code = nameWithoutSpace;
    }

    public void initializationCode()
    {
        if (File.Exists(nameWithoutSpace + ".dll"))
        {
            var DLL = Assembly.LoadFile(nameWithoutSpace + ".dll");
            Type type = DLL.GetType(nameWithoutSpace);
            methods = type.GetMethods();
        }
        else
        {
            Stop();
            StopSensor();
            Debug.Log("No code for " + name);
        }

    }

    public void UnInitializationCode()
    {
        methods = null;
    }

    public void StartingRobot()
    {
        if (program == null)
        {
            if (!File.Exists(code+".txt"))
            {
                Debug.Log("Robot " + this.name + " has't code. Please create it!");
            }
            else
                Compiler.instance.button1_Click();
        }
    }

    public void StartRobot()
    {
        if (methods != null)
        {
            program = new Thread(() =>
            {
                methods[0].Invoke(this, new object[] { this });
            });
            StartSensor();
            program.Start();
        }
        else
            Debug.Log("Robot " + name +" hasn't got program!");
    }

    public void Stop()
    {
        if(program!=null)
            program.Abort();
        for (int i = 0; i < motors.Length; i++)
        {
            motors[i].powerMotor = 0;
        }
        StopSensor();
    }

    public void StartSensor()
    {
        foreach (Sensor sensor in sensors)
            sensor.Enable();
    }

    public void StopSensor()
    {
        foreach (Sensor s in sensors)
        {
            s.Disable();
        }
    }

	
	private void Awake() {


		Bounds b = new Bounds(Vector3.zero, Vector3.zero);
		foreach(Renderer r in GetComponentsInChildren<Renderer>())
			b.Encapsulate(r.bounds);
		_size = b.size;

		GetComponent<Rigidbody>().centerOfMass += centerOfMassOffset;
	}


	private void Update() {

        if (manualControl) {
			float stright = Input.GetAxis("X");
			float right = Input.GetAxis("C");
			float left = Input.GetAxis("Z");
            Code(stright, right, left);


        }
		else if (Simulation.isRunning){
            foreach (MotorController motor in motors)
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

    private void Code( float stright, float right, float left)
    {
        foreach(MotorController motor in motors)
            {
            if (stright == 1)
                motor.powerMotor = 20f;
            if(right == 1)
                motors[0].powerMotor = -20f;
            else if(left == 1)
                motors[1].powerMotor = -20f;
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
