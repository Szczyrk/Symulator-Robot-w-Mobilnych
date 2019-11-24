using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System;

public class IndividualEdit : MonoBehaviour
{
    static IndividualEdit instance;
    static List<GameObject> gameobjectsActive = new List<GameObject>();
    public Material material;
    static Material materialStatic;
    private static Material oldMaterial;
    static MeshRenderer oldGameObject = null;
    static MeshRenderer newGameObject;
    static private Button buttonMotor;
    static private Button buttonSensorParam;
    static GameObject objectEdited;
    static GameObject listElemnetsRobot;
    //static GameObject panelMotor;
    Button accept;
    MotorController motorEdit;
    TextMeshProUGUI text_Torque;
    TextMeshProUGUI text_RPM;
    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
    public static GameObject editPanel;
    public static GameObject inputCode;
    public static GameObject simulationControler;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        buttonMotor = GameObject.Find("Canvas/PanelMain/PanelCenter/Panel/buttonMotor").GetComponent<Button>();
        buttonSensorParam = GameObject.Find("Canvas/PanelMain/PanelCenter/Panel/buttonSensorParam").GetComponent<Button>();
        editPanel = GameObject.Find("Canvas/PanelMain/LeftPanel/LeftPanel_Edit");
        listElemnetsRobot= GameObject.Find("Canvas/PanelMain/RightPanel/Panel/Scroll View_Elements robot");
        inputCode =  GameObject.Find("Canvas/PanelMain/LeftPanel/LeftPanel_InputField");
        simulationControler = GameObject.Find("Canvas/PanelMain/RightPanel/Panel/Panel_Simulation");
        buttonMotor.gameObject.SetActive(false);
        buttonSensorParam.gameObject.SetActive(false);
        buttonSensorParam.onClick.AddListener(AddSensorParam);
        buttonMotor.onClick.AddListener(AddMotor);
        materialStatic = material;
        listElemnetsRobot.SetActive(false);
       // EditPanel.SetActive(false);
        /*
        panelMotor = GameObject.Find("Canvas/PanelMotor");
        accept = panelMotor.GetComponentInChildren<Button>();
        accept.onClick.AddListener(() => motorUpdate(panelMotor));
        text_Torque = panelMotor.transform.Find("InputField (TMP)/Text Area/Text_RPM").gameObject.GetComponent<TextMeshProUGUI>();
        text_RPM = panelMotor.transform.Find("InputField (TMP) (1)/Text Area/Text_Torque").gameObject.GetComponent<TextMeshProUGUI>();
        panelMotor.SetActive(false);
        */
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Simulation.state == Simulation.State.edit)
        {
           // Debug.Log("Mouse is down");

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity);
            if (hit)
            {
                newGameObject = hitInfo.collider.transform.GetComponent<MeshRenderer>();
                if (newGameObject != oldGameObject)
                {
                    if (newGameObject != null)
                    {
                        if (oldGameObject)
                            oldGameObject.sharedMaterial = oldMaterial;
                        oldGameObject = newGameObject;
                        Debug.Log("Hit " + hitInfo.collider.name + " " + newGameObject);
                        oldMaterial = newGameObject.sharedMaterial;
                        newGameObject.sharedMaterial = material;
                    }
                }
            }
            else
            {
                if (oldGameObject)
                    oldGameObject.sharedMaterial = oldMaterial;
                oldGameObject = null;
                Debug.Log("No hit!");
                buttonMotor.gameObject.SetActive(false);
                buttonSensorParam.gameObject.SetActive(false);
            }
        }
        if(newGameObject)
        {
            buttonMotor.gameObject.SetActive(true);
            buttonSensorParam.gameObject.SetActive(true);
        }
    }

    public static void StartIndividualEdit(GameObject objectEdit)
    {
        inputCode.SetActive(false);
        editPanel.SetActive(true);
        listElemnetsRobot.SetActive(true);
        AddElementsToList.Start_UI(objectEdit);

        foreach (GameObject objectInScene in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (objectInScene.GetComponent<Camera>() || objectInScene.GetComponent<Light>() || objectInScene.GetComponent<Canvas>() 
                || objectInScene.name == "EventSystem" || objectInScene.name == "StopBox" || objectInScene.GetComponent<UI_SimulaitonControl>()
                || objectEdit == objectInScene)
                continue;
            if (objectInScene.active)
            {
                gameobjectsActive.Add(objectInScene);
                objectInScene.SetActive(false);
            }
        }
        objectEdit.transform.rotation = Quaternion.Euler(0, 90, 0);
        objectEdit.GetComponent<Rigidbody>().isKinematic = true;
        Time.timeScale = 1f;
        instance.StartCoroutine("Coroutine");
        simulationControler.SetActive(false);
        objectEdited = objectEdit;
        AutomaticEditPanel_UI.Start_UI();
        CamController.SetViewMode(CamController.ViewMode.Orbit);
        // EditPanel.SetActive(true);
    }

    private IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0f;
    }
    public static void BackToSimulation()
    {
        foreach(GameObject gameObject in gameobjectsActive)
        {
            if(gameObject != null)
                gameObject.SetActive(true);
        }
        inputCode.SetActive(true);
        editPanel.SetActive(false);
        listElemnetsRobot.SetActive(false);
        simulationControler.SetActive(true);
        buttonMotor.gameObject.SetActive(false);
        buttonSensorParam.gameObject.SetActive(false);

        objectEdited.GetComponent<Rigidbody>().isKinematic = false;
    }
    void AddMotor()
    {
        if(newGameObject)
        {
            motorEdit = newGameObject.gameObject.AddComponent<MotorController>();
            motorEdit.CreatMotor(newGameObject);
            newGameObject.GetComponent<MeshCollider>().enabled = false;


            Simulation.robotSelected.UpdateEquipment();
            AutomaticEditPanel_UI.Start_UI();
        }
    }


    void AddSensorParam()
    {
        if (newGameObject)
        {
            DistanceSensorController sensor = newGameObject.gameObject.AddComponent<DistanceSensorController>();
            sensor.CreatStartLight();
            Simulation.robotSelected.UpdateEquipment();
            AutomaticEditPanel_UI.Start_UI();
        }
    }

    public static void ResetSelect()
    {
        if (oldGameObject)
            oldGameObject.sharedMaterial = oldMaterial;
    }

    public static void SelectObject(GameObject select)
    {
        newGameObject = select.GetComponent<MeshRenderer>();
            if (oldGameObject)
                oldGameObject.sharedMaterial = oldMaterial;
            oldGameObject = newGameObject;
            oldMaterial = newGameObject.sharedMaterial;
            newGameObject.sharedMaterial = materialStatic;
    }
}
