using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TextVariablesRobot : MonoBehaviour
{

    Robot robot;

    private TMP_Text textTMPStart;
    private InputField textCode;

    private int lineStart;
    private int lineCode;
    float updateCode = 0.2f;


    void Start()
    {
        robot = transform.GetComponent<Robot>();
        if (GameObject.Find("Canvas/InputField_Imag/InputField/Text Start"))
        {
            textTMPStart = GameObject.Find("Canvas/InputField_Imag/InputField/Text Start").GetComponent<TMP_Text>();
            UpdateVariables();
        }
        if (GameObject.Find("Canvas/InputField_Imag/InputField/InputFieldCode"))
        {
            textCode = GameObject.Find("Canvas/InputField_Imag/InputField/InputFieldCode").GetComponent<InputField>();
           UpdateCode();
        }
    }

    public void UpdateCode()
    {
        if (textCode == null)
            textCode = GameObject.Find("Canvas/InputField_Imag/InputField/InputFieldCode").GetComponent<InputField>();
        string path = "Assets/Resources/ExampleCode.txt";
        if (File.Exists(robot.nameWithoutSpace + ".txt"))
        {
            path = robot.nameWithoutSpace + ".txt";
        }
        
        textCode.text = File.ReadAllText(path);
    }

    public void UpdateVariables()
    {
        if(textTMPStart == null)
            textTMPStart = GameObject.Find("Canvas/InputField_Imag/InputField/Text Start").GetComponent<TMP_Text>();
        textTMPStart.text = robot.name + "\n";
        textTMPStart.text += "//Sensors:  \n";
        foreach (Sensor sensor in robot.sensors)
            textTMPStart.text += "bool "+sensor.name + "\n";
        textTMPStart.text += "\n";
        textTMPStart.text += "//Motors:  \n";
        foreach (MotorToWheel motor in robot.motors)
            textTMPStart.text += "float "+motor.name + "\n";
    }

}
