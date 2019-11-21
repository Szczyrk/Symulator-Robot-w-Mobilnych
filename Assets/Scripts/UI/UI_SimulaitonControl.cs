using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_SimulaitonControl : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Toggle toggle;

    public void Pause()
    {
        Simulation.Pause();
    }
    public void Run()
    {
        foreach (Robot robot in Simulation.robots)
        {

                robot.initializationCode();
        }
        Simulation.Run();
    }
    public void Reset()
    {
        foreach(Robot robot in Simulation.robots)
        {
            robot.Reset();
            robot.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void Birdseye()
    {
        CamController.SetViewMode(CamController.ViewMode.Birdseye);
    }
    public void FreeMovment()
    {
        CamController.SetViewMode(CamController.ViewMode.FreeMovement);
    }
    public void Orbit()
    {
        CamController.SetViewMode(CamController.ViewMode.Orbit);
    }

    public void SaveRobot()
    {
        PrefabUtility.SaveAsPrefabAsset(Simulation.robotSelected.gameObject, "Assets" + Path.DirectorySeparatorChar 
            + "Resources" + Path.DirectorySeparatorChar + "Robots" + Path.DirectorySeparatorChar 
            + Simulation.robotSelected.name + ".prefab");
    }

    public void DropDownCamera()
    {
        if(dropdown)
            switch(dropdown.value)
            {
                case 0:
                    Birdseye();
                    break;
                case 1:
                    FreeMovment();
                    break;
                case 2:
                    Orbit();
                    break;
            }
    }

    public void ToggleDrawDebug()
    {
        if(!toggle.isOn)
        {
           foreach(Robot robot in Simulation.robots)
                foreach (ParamSensor sensor in robot.sensors)
                {
                    sensor.drawDebug = false;
                }
        }
        else
        {
            foreach (Robot robot in Simulation.robots)
                foreach (ParamSensor sensor in robot.sensors)
                {
                    sensor.drawDebug = true;
                }
        }
    }
}
