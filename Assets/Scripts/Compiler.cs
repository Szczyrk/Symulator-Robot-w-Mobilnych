using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Compiler : MonoBehaviour
{
   public TextMeshProUGUI textError;
    public InputField textCode;
    public static Compiler instance;
    List<string> patchsToDelete = new List<string>();

    void OnDestroy()
    {
        foreach (String s in patchsToDelete)
            if (File.Exists(s))
                File.Delete(s);
    }
    void Awake()
    {
        instance = this;
    }

    public void button1_Click()
    {
        Robot robot =   Simulation.robotSelected;
       robot.UnInitializationCode();
        CSharpCodeProvider codeProvider = new CSharpCodeProvider();
       robot.nameWithoutSpace = 
            codeProvider.CreateValidIdentifier(robot.nameWithoutSpace);
        ICodeCompiler icc = codeProvider.CreateCompiler();
        string Output =robot.nameWithoutSpace+ ".dll";
        string OutputCSCode =robot.code + ".txt";
        textError.text = "";
        CompilerParameters parameters = new CompilerParameters();
        parameters.GenerateExecutable = false;
        parameters.GenerateInMemory = true;
        parameters.ReferencedAssemblies.Add("System.dll");
        parameters.ReferencedAssemblies.Add(typeof(Simulation).Assembly.Location);
        parameters.ReferencedAssemblies.Add(typeof(Robot).Assembly.Location);
        parameters.ReferencedAssemblies.Add(typeof(MonoBehaviour).Assembly.Location);
        parameters.OutputAssembly = Output;
        System.IO.File.WriteAllText(OutputCSCode, textCode.text);
        string code = RenameVariables(textCode.text, robot);
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
           robot.initializationCode();
            int count = 1;
            string tempFileName = robot.nameWithoutSpace;
            patchsToDelete.Add(Output);
            while (File.Exists(Output))
            {
                tempFileName = string.Format("{0}_{1}", robot.code, count++);
                Output = tempFileName + ".dll";
            }
            robot.nameWithoutSpace = tempFileName;
            textError.faceColor = Color.blue;
            textError.text = "Success!";
        }
    }

    private string RenameVariables(string text, Robot robot)
    {
        if(!robot)
            return text;
        text = text.Replace("Code",robot.nameWithoutSpace);
        text = text.Replace("void Main()", "void Main(Robot robot)");
        for (int i =0; i <robot.motors.Length;i++)
            text = text.Replace(Simulation.robotSelected.motors[i].name, "robot.motors["+i+"].powerMotor");
        for (int i = 0; i <robot.sensors.Length; i++)
            text = text.Replace(Simulation.robotSelected.sensors[i].name, "robot.sensors[" + i + "].detected");
        return text;
            
    }
}
