using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Draw : MonoBehaviour {


	private class LineData {
		public Vector3 start;
		public Vector3 end;
		public Color color;
		public LineData(Vector3 from, Vector3 to, Color c) {
			start = from;
			end = to;
			color = c;
		}
	}
    private class Circle
    {
        public Vector3 start;
        public Color color;
        public float radius;
        public Circle(Vector3 from,  Color c, float r)
        {
            start = from;
            color = c;
            radius = r;
        }
    }

    public static Draw Instance;


	
	private Material glLineMaterial;
	private List<LineData> gl_lines = new List<LineData>();
    private List<Circle> gl_circle = new List<Circle>();


    void Awake() {
		if (Instance == null){
			Instance = this;
		}
		else {	
			Destroy(this);
		}

		glLineMaterial = Resources.Load<Material>("Materials/Line");
	}
	


	public void Line(Vector3 start, Vector3 end, Color color, Space relativeTo = Space.World) {
        foreach(Robot robot in Simulation.robots)
		if (relativeTo == Space.Self && robot != null) {
			start = robot.transform.TransformPoint(start);
			end = robot.transform.TransformPoint(end);
		}
		gl_lines.Add(new LineData(start, end, color));
	}
    public void AddCircle(MotorController wheel)
    {
        if (wheel != null)
        {
            gl_circle.Add(
                new Circle(wheel.transform.position + wheel.centerWheel, 
                Color.green, wheel.wheelCollider.radius * wheel.transform.lossyScale.x));
        }

    }
    public void Bearing(Vector3 origin, Vector3 direction, Color color) {
		Vector3 end = origin + direction;
		gl_lines.Add(new LineData(origin, end, color));
	}
	
	private void GLLine(LineData line) {
		GL.Color(line.color);
		GL.Vertex(line.start);
		GL.Vertex(line.end);
	}

	void OnPostRender() {
        if (Simulation.state == Simulation.State.edit)
        {
            GL.PushMatrix();
            glLineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            foreach (LineData line in gl_lines)
            {
                GLLine(line);
            }
            foreach (Circle circle in gl_circle)
            {
                GLCICRLE(circle);
            }
            GL.End();
           GL.PopMatrix();
        }else
        {
            GL.PushMatrix();
            glLineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            foreach (LineData line in gl_lines)
            {
                GLLine(line);
            }
            GL.End();
            GL.PopMatrix();
            gl_lines.Clear();
        }
    }
    void GLCICRLE(Circle circle)
    {
        GL.Color(circle.color);
        float degRad = Mathf.PI / 180;
        for (float theta = 0.0f; theta < (2 * Mathf.PI); theta += 0.01f)
        {
            Vector3 ci = (new Vector3(Mathf.Cos(theta) * circle.radius + circle.start.x, Mathf.Sin(theta) * circle.radius + circle.start.y, circle.start.z));
            GL.Vertex3(ci.x, ci.y, ci.z);
        }
    }
    public void Clear()
    {
        gl_lines.Clear();
        gl_circle.Clear();
    }
    public void ClearLine()
    {
        gl_lines.Clear();
    }
    public void ClearCircle()
    {
        gl_circle.Clear();
    }
}
