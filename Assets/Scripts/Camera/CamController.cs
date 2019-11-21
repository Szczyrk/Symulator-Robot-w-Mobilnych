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

	public static ReadOnlyCollection<ViewMode> viewModeList {
		get { return _modes.AsReadOnly(); }
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
	

	public static void RemoveAreaOfInterest(IObjectDimensions area) {
		if (_areas.Count > 1) _areas.Remove(area);
		if (_area >= _areas.Count) _area = 0;
	}

	public static void ClearAreaList() {
		_areas.Clear();
		_area = 0;
	}

	public static void SetAreaOfInterest(int index) {
		// ignore out of range indexes
		if (index < 0 || index >= _areas.Count) {
			Debug.LogWarning("SetAreaOfInterest index param out of range. Ignoring.");
			return;
		}
		_area = index;
	}
	

	public static void SetAreaOfInterest(IObjectDimensions obj) {
		int index = _areas.IndexOf(obj);
		if (index >= 0) _area = index;
	}

	public static void CycleAreaOfInterest() {
		_area = (++_area) % _areas.Count;
	}
	
	
	
	public static void AddViewMode(ViewMode mode) {
		if (!_modes.Contains(mode)) _modes.Add(mode);
	}
	
	public static void RemoveViewMode(ViewMode mode) {
		if (mode != ViewMode.FreeMovement) _modes.Remove(mode);
		if (_mode > _modes.Count) {
			SetViewMode(0);
		}
	}

	public static void ClearViewModeList() {
		_modes.Clear();
		_modes.Add(ViewMode.Orbit);
		SetViewMode(0);
	}
	
	public static void SetViewMode(ViewMode mode) {
		int index = _modes.IndexOf(mode);
		SetViewMode(index);
	}
	
	public static void SetViewMode(int index) {
		// ignore out of range indexes
		if (index < 0 || index >= _modes.Count) {
			Debug.LogWarning("SetViewMode index param out of range. Ignoring.");
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
	

	public static void RandomViewMode() {
		SetViewMode(Random.Range(0, _modes.Count-1));
	}
	


	public float mouseSensitivity;		
	

	public float freeMoveSpeed;		
	

	public float freeMoveSpeedShiftMult;
	

	
	
	private float _birdseyeDist = 0f;				// vertical distance offset for camera in birdseye mode
	private float _3rdPersonDist = 0.5f;			// distance offset for camera in orbit mode
	private Vector3 _3rdPersonDir = Vector3.one;	// direction offset for camera in orbit mode
	private Vector3 _1stPersonDir = Vector3.one;	// camera direction when in free movement mode
	

	// called when the instance enters the scene
	void Awake() {
		if (Instance == null) {	
			Instance = this;
		}
		else {
			Destroy(this);
		}
		_camera = GetComponent<Camera>();
	}
	
	// called on the first frame for this instance 
	void Start() {
		SetViewMode(0);
	}
	
	// called every frame for this instance
	void Update() {
		
		// set camera size on screen
		GetComponent<Camera>().rect = new Rect(0, 0, 1f, 1f);
		

		if ( GetComponent<Camera>().pixelRect.Contains(Input.mousePosition) ) {

			_birdseyeDist -= Input.GetAxis("Mouse ScrollWheel") * 1.2f;
			_3rdPersonDist -= Input.GetAxis("Mouse ScrollWheel") * 1.2f;
			_3rdPersonDist = Mathf.Min(_3rdPersonDist, 20f);
			_3rdPersonDist = Mathf.Max(_3rdPersonDist, 0.15f);
			
			if (Input.GetMouseButton(1)) {
				float x = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
				float y = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
				Quaternion rotation = Quaternion.AngleAxis(-y, Vector3.up);
				_3rdPersonDir = rotation * _3rdPersonDir;
				rotation = Quaternion.AngleAxis(x, _camera.transform.right);
				_3rdPersonDir = rotation * _3rdPersonDir;
				
				rotation = Quaternion.AngleAxis(y, Vector3.up);
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
		
		float size = Mathf.Max(area.bounds.size.x/2f, area.bounds.size.z/2f);
		size += _birdseyeDist;
		size = Mathf.Max(size, 0.5f);
		_camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, size, Time.unscaledDeltaTime * 4f);
		Vector3 targetPosition = area.bounds.center + Vector3.up * 10f;
		

		_camera.transform.position = Vector3.Slerp(
			_camera.transform.position, 
			targetPosition, 
			Time.unscaledDeltaTime * 4f
			);
		

		Quaternion targetRotation = Quaternion.LookRotation(Vector3.down);
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

        if (Simulation.robotSelected == null)
            targetPosition = area.bounds.center;
        else
            targetPosition = Simulation.robotSelected.bounds.center;
        Ray ray = new Ray(area.bounds.center, _3rdPersonDir);
		RaycastHit hit;
			targetPosition = area.bounds.center + _3rdPersonDir * _3rdPersonDist;
		_camera.transform.position = Vector3.Slerp(
			_camera.transform.position, 
			targetPosition, 
			Time.unscaledDeltaTime * 4f
			);
		

		Quaternion targetRotation = Quaternion.LookRotation(area.bounds.center - targetPosition);
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
