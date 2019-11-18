using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawCircle : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 5)]
    public float xradius = 5;
    [Range(0, 5)]
    public float yradius = 5;
    static LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
    }
    public void Start_Draw(float radius)
    {

        
        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        CreatePoints(radius);
    }

    void CreatePoints(float radius)
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(0, x, z));

            angle += (360f / segments);
        }
    }
}

