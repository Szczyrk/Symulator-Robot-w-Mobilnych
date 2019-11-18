using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class Simulation : MonoBehaviour {

    private static Button buttonEdit;
    public enum State {
        menu,
		starting,
		simulating,
		stopped,
        edit,
		end
	}

	private class Observer : IObjectDimensions {
		public Observer() {}
		
		public string name {
			get { return "Simulation"; }
		}
		
		public Bounds bounds {
			get {
				// encapsulate robot and destination
				if (isRunning) {
					Bounds b = new Bounds();
					b.Encapsulate(robotSelected.position);
					return b;
				} 
				// use Simulation.Instance.Bounds
				else {
					return Simulation.Instance.bounds;
				}
			}
		}
	}


	static Simulation() {
		testArea = new Observer();
	}


	public static Simulation Instance;
	

	public static State state {get; private set;}

	public static bool exhibitionMode;  
	

	public static int simulationNumber {
		get; private set;
	}
	

	public static int testNumber {
		get; private set;
	}
	

	public static Robot robotSelected {
		get { return _robotSelected; }
		private set {
		//	if(_robotSelected) _robotSelected.transform.Recycle();
			_robotSelected = value;
		}
	}
    public static List<Robot> robots 
    {
        get { return _robots; }
        private set
        {
            _robots = value;
        }
    }
    public static List<string> namesRobotInSimulation;

    public static GameObject environment {
		get {
			return _environment.gameObject;
		}
		set {
			if (_environment) _environment.transform.Recycle();
			_environment = value.GetComponent<Environment>();
			SetBounds();
		}
	}
	
	

	public static IObjectDimensions testArea {
		get; private set;
	}
	
	public static bool isRunning {
		get { return state == State.simulating; }
	}

	public static bool isStopped {
		get { return state == State.stopped; }
	}

	public static bool isFinished {
		get { return state == State.end; }
	}

	

	public static bool paused {
		get { return _paused; }
		set {
			_paused = value;
			if (_paused) Time.timeScale = 0f;
			else Time.timeScale = timeScale;
		}
	}


	public static float time {
		get {
			if (isRunning) _stopTime = Time.time;
			return _stopTime - _startTime;
		}
	}

	public static float timeScale {
		get { return _timeScale; }
		set {
			_timeScale = value;
			if (!paused) {
				Time.timeScale = value;
			}
		}
	}

	private static float _startTime;
	private static float _stopTime;
	
	private static Robot _robotSelected;
    private static List<Robot> _robots = new List<Robot>();
    private static Environment _environment;
	private static bool _paused;
	private static float _timeScale = 1f;


	public static void Enter() {
		CamController.AddViewMode(CamController.ViewMode.Birdseye);
		CamController.AddViewMode(CamController.ViewMode.FreeMovement);
		CamController.AddViewMode(CamController.ViewMode.Orbit);
		CamController.AddAreaOfInterest(testArea);
	}
	

	public static void Run() {
		Debug.Log("Simulation run.");
		if (state == State.stopped || state == State.starting) {
			Time.timeScale = _timeScale;
			state = State.simulating;
            foreach (Robot robot in robots)
            {
                robot.StartSensor();
            }
		}
	}

    public static void Starting()
    {
        Debug.Log("Simulation starting.");
        if (state == State.stopped || state == State.menu)
        {
            state = State.starting;
        }
    }


    public static void Pause() {
		Debug.Log("Simulation Pause.");
		if (state == State.simulating || state == State.starting) {
			Time.timeScale = 0f;
			state = State.stopped;
		}
	}
	
	
	

	public static Vector3 RandomInBounds(Bounds b) {
		Vector3 v = new Vector3();
		v.x = UnityEngine.Random.Range(b.min.x, b.max.x);
		v.y = b.max.y;
		v.z = UnityEngine.Random.Range(b.min.z, b.max.z);
		RaycastHit hit;
		if (Physics.Raycast(v, Vector3.down, out hit)) {
			v = hit.point + hit.normal* 0.25f;
			Debug.DrawRay(v, Vector3.down, Color.white, 5f);
		}
		return v;
	}
	

	
	private static void SetBounds() {
		Bounds b = new Bounds();
		foreach(Renderer r in environment.GetComponentsInChildren<Renderer>())
			b.Encapsulate(r.bounds);
			
		Instance.bounds = b;
	}

	public Bounds bounds {
		get; private set;
	}
	
	

	void Awake() {
		if (Instance) {
			Destroy(this.gameObject);
		}
		else {
			Instance = this;
		}
	}
	

	void Start()
    {
        state = State.menu;
        if (GameObject.Find("buttonEdit"))
        {
            buttonEdit = GameObject.Find("buttonEdit").GetComponent<Button>();
            state = State.starting;
            buttonEdit.onClick.AddListener(EditStart);
        }
        robots = new List<Robot>();
     /*   foreach (Robot robot in GameObject.FindObjectsOfType<Robot>())
        {
            robots.Add(robot);
        }*/
        namesRobotInSimulation = new List<string>();
        DontDestroyOnLoad(this);

    }

    public static void StartSimulation()
    {
        robots.Clear();
        foreach(string robot in namesRobotInSimulation)
        {
          robots.Add(RobotLoader.LoadRobotGameObject(robot).GetComponent<Robot>());

        }
        if(_environment != null)
            environment.transform.Spawn();
        if (buttonEdit == null)
        {
            buttonEdit = GameObject.Find("Canvas/Panel/buttonEdit").GetComponent<Button>();
            buttonEdit.onClick.AddListener(EditStart);
        }
        if(robots.Count == 0)
            robotSelected = robots[0];
    }

    private static void EditStart()
    {
        if(robotSelected != null)
            if (state != State.edit)
            {
                state = State.edit;
                IndividualEdit.StartIndividualEdit(robotSelected.gameObject);
            }
            else
            {
                state = State.starting;
                IndividualEdit.BackToSimulation();
                robotSelected.ShowVariables();
            }
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && state != State.edit)
        {

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity);
            if (hit)
            {
                if (hitInfo.transform.GetComponentInParent<Robot>())
                {
                    Robot robot = hitInfo.transform.GetComponentInParent<Robot>();
                    if(robot != robotSelected)
                    {
                        robotSelected = robot;
                        Debug.Log("Robot selected: " +robotSelected.name);
                        robotSelected.ShowVariables();
                    }
                }
            }
        }
        if (isRunning) {
           
            // check for conditions to end the test
            /*	if (robot.atDestination ) {
                    Debug.Log("Simulation: nav objective complete!");
                    NextTest();
                }
                 if (robot.isStuck && settings.continueOnRobotIsStuck) {
                    Debug.LogWarning("Simulation: Robot appears to be stuck! Skipping test.");
                    NextTest();
                }
                else if (settings.maximumTestTime > 0 && time > settings.maximumTestTime) {
                    Debug.LogWarning("Simulation: Max test time exceeded! Skipping test.");
                    NextTest();
                }*/

        }
        if (state == State.starting)
        {
           // this.gameObject.AddComponent<IndividualEdit>();
            if (CheckRobots())
                Pause();
        } 
    }


    bool CheckRobots()
    {
        foreach(Robot robot in _robots)
        {
            if (robot.motors.Length == 0)
            {
                Pause();
                state = State.edit;
                Simulation.robotSelected = robot;
                IndividualEdit.StartIndividualEdit(robot.gameObject);
                robotSelected = robot;
                return false;
            }
        }
        return true;
    }

    void OnDrawGizmos() {
		Gizmos.DrawWireCube(bounds.center, bounds.size);
	}
	
}
