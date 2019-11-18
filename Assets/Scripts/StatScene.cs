using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Simulation.Starting();
        Simulation.StartSimulation();

    }

}
