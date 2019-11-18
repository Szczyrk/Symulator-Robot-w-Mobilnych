using Microsoft.CSharp;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor.Compilation;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Compiler : MonoBehaviour
{
   public TextMeshProUGUI textError;
    public InputField textCode;

    public void button1_Click()
    {
        CSharpCodeProvider codeProvider = new CSharpCodeProvider();
        Simulation.robotSelected.nameWithoutSpace = 
            codeProvider.CreateValidIdentifier(Simulation.robotSelected.nameWithoutSpace);
        ICodeCompiler icc = codeProvider.CreateCompiler();
        string Output = Simulation.robotSelected.nameWithoutSpace + ".dll";
        string OutputCSCode = Simulation.robotSelected.nameWithoutSpace + ".txt";

        textError.text = "";
        System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
        parameters.GenerateExecutable = false;
        parameters.GenerateInMemory = true;
        parameters.ReferencedAssemblies.Add("System.dll");
        parameters.ReferencedAssemblies.Add(typeof(Simulation).Assembly.Location);
        parameters.ReferencedAssemblies.Add(typeof(Robot).Assembly.Location);
        parameters.ReferencedAssemblies.Add(typeof(MonoBehaviour).Assembly.Location);
        parameters.OutputAssembly = Output;
        System.IO.File.WriteAllText(OutputCSCode, textCode.text);
        string code = RenameVariables(textCode.text);
        CompilerResults results = icc.CompileAssemblyFromSource(parameters, code);
        if (results.Errors.Count > 0)
        {
            textError.faceColor = Color.red;
            foreach (CompilerError CompErr in results.Errors)
            {
                textError.text = textError.text +
                            "Line number " + CompErr.Line +
                            ", Error Number: " + CompErr.ErrorNumber +
                            ", '" + CompErr.ErrorText + ";" +
                             "\n" + " \n";
            }
        }
        else
        {
            textError.faceColor = Color.blue;
            textError.text = "Success!";
        }
    }

    private string RenameVariables(string text)
    {
        if(!Simulation.robotSelected)
            return text;
        text = text.Replace("Code", Simulation.robotSelected.nameWithoutSpace);
        text = text.Replace("void Main()", "void Main(Robot robot)");
        for (int i =0; i < Simulation.robotSelected.motors.Length;i++)
            text = text.Replace(Simulation.robotSelected.motors[i].name, "robot.motors["+i+"].powerMotor");
        for (int i = 0; i < Simulation.robotSelected.sensors.Length; i++)
            text = text.Replace(Simulation.robotSelected.sensors[i].name, "robot.sensors[" + i + "].detected");
        return text;
            
    }
}
