using UnityEngine;
using System.Collections;

public class ParamSensor : Sensor {


	public float FOV = 20f;

	public float maxRange = 20f;
	
	public float updateDt = 0.2f;

    public Transform startLight;
	
	void Start()
    {
        if (startLight == null)
        {
            startLight = new GameObject().transform;
            startLight.name = "Start Light";
            startLight.SetParent(this.transform);
            startLight.localPosition = new Vector3(-0.00000001f,0,0);

        }
    }
	public bool scanning {
		get; private set;
	}
	
	
	
	public override void Enable () {
		scanning = true;
		StartCoroutine(UpdateData());
	}

	public override void Disable () {
		scanning = false;
	}
	

	private IEnumerator UpdateData() {
		
		while(scanning) {
			float proximity = maxRange;
            for (float a = -FOV; a < FOV; a += 2f)
            {
                Quaternion rotation = Quaternion.Euler(new Vector3(0,a,0));
				Vector3 direction = rotation * startLight.forward;
				float check = Cast (direction);
				if (check < proximity) proximity = check;
			}
			direction = startLight.forward * proximity;
			if (proximity < maxRange * 0.75f) detected = true;
			else detected = false;
			yield return new WaitForSeconds(updateDt);
		}
	
	}
	
	private float Cast(Vector3 direction) {
		Ray ray = new Ray(startLight.position, direction);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, maxRange)) {
			return hit.distance;
		}
		else  {
			return maxRange;
		}
	}
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(Simulation.state == Simulation.State.edit)
            Gizmos.DrawRay(startLight.position, startLight.position + Quaternion.Euler(new Vector3(maxRange/Mathf.Sqrt(2), 0, maxRange / Mathf.Sqrt(2)))* startLight.forward);
    }
}
