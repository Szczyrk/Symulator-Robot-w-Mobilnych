using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


[RequireComponent(typeof(Camera))]
public class CamController : MonoBehaviour {


	public enum ViewMode {
        Orbit,

        Birdseye,
		
		FreeMovement
	}
	
	
	private class StubObservable : IObjectDimensions {
		public StubObservable() {
			bounds = new Bounds();
		}
		public string name {
			get { return Strings.projectTitle; }
		}
		
		public Bounds bounds {
			get; private set;
		}
	}
	

	public static CamController Instance {
		get; private set;
	}
	

	public static ViewMode viewMode {
		get {
			return _modes[_mode];
		}
	}
	


	public static IObjectDimensions area {
		get { 
			if (_areas.Count > 0)
				return _areas[_area]; 
			else 
				return _stub;
		}
	}

	private static List<ViewMode>		_modes;		
	private static List<IObjectDimensions>	_areas;		
	private static IObjectDimensions			_stub;		
	private static int _mode; 				
	private static int _area; 					
	private static Camera _camera;						

	static CamController() {
		_modes = new List<ViewMode>();
        _modes.Add(ViewMode.Orbit);
        _modes.Add(ViewMode.Birdseye);
        _modes.Add(ViewMode.FreeMovement);
        _areas = new List<IObjectDimensions>();
		_stub = new StubObservable();
	}


	public static void AddAreaOfInterest(IObjectDimensions area) {
		if (!_areas.Contains(area)) _areas.Add(area);
	}
	
	
	public static void AddViewMode(ViewMode mode) {
		if (!_modes.Contains(mode)) _modes.Add(mode);
	}

	
	public static void SetViewMode(ViewMode mode) {
		int index = _modes.IndexOf(mode);
		SetViewMode(index);
	}
	
	public static void SetViewMode(int index) {
		if (index < 0 || index >= _modes.Count) {
			Debug.LogWarning("SetViewMode index param out of range.");
			return;
		}
		_mode = index;
		_camera.transform.parent = null;
		switch (_modes[_mode]) {
		case ViewMode.Orbit:
		case ViewMode.FreeMovement:
			_camera.orthographic = false;
			_camera.fieldOfView = 60f;
			break;
		default:
		case ViewMode.Birdseye:
			_camera.GetComponent<Camera>().orthographic = true;
			_camera.transform.parent = null;
			break;
		}
	}

	


	public float mouseSensitivity;		
	

	public float freeMoveSpeed;		
	

	public float freeMoveSpeedShiftMult;
	
	private float birdseyeScroll = 0f;
	private float orbitScroll = 0.5f;
	private Vector3 _1stPersonDir = Vector3.one;
	

	void Awake() {
		if (Instance == null) {	
			Instance = this;
		}
		else {
			Destroy(this);
		}
		_camera = GetComponent<Camera>();
	}
	
	void Start() {
		SetViewMode(0);
	}
	

	void Update() {
		
		GetComponent<Camera>().rect = new Rect(0, 0, 1f, 1f);
		

		if ( GetComponent<Camera>().pixelRect.Contains(Input.mousePosition) ) {

			birdseyeScroll -= Input.GetAxis("Mouse ScrollWheel") * 1.2f;
			orbitScroll -= Input.GetAxis("Mouse ScrollWheel") * 1.2f;
			orbitScroll = Mathf.Min(orbitScroll, 20f);
			orbitScroll = Mathf.Max(orbitScroll, 0.15f);
			
			if (Input.GetMouseButton(1)) {
				float x = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
				float y = Input.GetAxisRaw("Mouse X") * mouseSensitivity;

                Quaternion rotation = Quaternion.AngleAxis(y, Vector3.up);
				_1stPersonDir = rotation * _1stPersonDir;
				rotation = Quaternion.AngleAxis(-x, _camera.transform.right);
				_1stPersonDir = rotation * _1stPersonDir;
			}
		}


		switch(viewMode) {
		case ViewMode.Birdseye:
			BirdseyeUpdate();
			break;
		case ViewMode.FreeMovement:
			FreeMovementUpdate();
			break;
		case ViewMode.Orbit:
			OrbitUpdate();  
			break;
		default:
			break;
		}
	}
	

	void BirdseyeUpdate() {

        Vector3 targetPosition;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.down);
        float size;
        if (Simulation.robotSelected == null)
        {
            targetPosition = area.bounds.center + Vector3.up * 10f;
            size = Mathf.Max(area.bounds.size.x / 2f, area.bounds.size.z / 2f);
        }
        else
        {
            targetPosition = Simulation.robotSelected.bounds.center + Vector3.up * 10f;
            size = Mathf.Max(Simulation.robotSelected.bounds.size.x / 2f, Simulation.robotSelected.bounds.size.z / 2f);
        }
      
		size += birdseyeScroll;
		size = Mathf.Max(size, 0.5f);
		_camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, size, Time.unscaledDeltaTime * 4f);
		
		

		_camera.transform.position = Vector3.Slerp(
			_camera.transform.position, 
			targetPosition, 
			Time.unscaledDeltaTime * 4f
			);

		_camera.transform.rotation = Quaternion.Slerp(
			_camera.transform.rotation, 
			targetRotation, 
			Time.unscaledDeltaTime * 8f
			);
	}
	

	void FreeMovementUpdate() {
		transform.rotation = Quaternion.LookRotation(_1stPersonDir);
		float b = Input.GetKey(KeyCode.LeftShift) ? freeMoveSpeedShiftMult : 1f;
		float y = Input.GetAxisRaw("Z") * freeMoveSpeed * b * Time.unscaledDeltaTime;
		float x = Input.GetAxisRaw("X") * freeMoveSpeed * b * Time.unscaledDeltaTime;
		Vector3 pos = transform.position;
		pos += transform.forward * y;
		pos += transform.right * x;
		transform.position = pos;
	}
	
	void OrbitUpdate() {
        Vector3 targetPosition;
        Quaternion targetRotation;
        if (Simulation.robotSelected == null)
        {
            targetPosition = area.bounds.center + _1stPersonDir * orbitScroll;
            targetRotation = Quaternion.LookRotation(area.bounds.center - targetPosition);
        }
        else
        {
            targetPosition = Simulation.robotSelected.bounds.center + _1stPersonDir * orbitScroll;
            targetRotation = Quaternion.LookRotation(Simulation.robotSelected.bounds.center - targetPosition);
        }
       
		_camera.transform.position = Vector3.Slerp(
			_camera.transform.position, 
			targetPosition, 
			Time.unscaledDeltaTime * 4f
			);
		

		
		_camera.transform.rotation = Quaternion.Slerp(
			_camera.transform.rotation, 
			targetRotation, 
			Time.unscaledDeltaTime * 8f
			);
	}

    public static void LookOnObject(Transform transform)
    {
        _camera.transform.LookAt(transform);
    }
	
}
