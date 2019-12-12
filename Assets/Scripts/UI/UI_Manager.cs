using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public Button button;
    Transform parent;
    List<Button> gameObjects;
    public float offsetW;
    public float offsetH;
    delegate GameObject LoadObj(string name);
    LoadObj loadObj;
    public GameObject imagePanel;
    Vector3 sizeCalculated;
    List<GameObject> objectsToDestroyWhenButton = new List<GameObject>();
    public Button add;
    string actionName = null;


    void Start()
    {
        foreach (Transform render in button.transform.GetChild(0))
        {
            Renderer tmpR = render.GetComponent<Renderer>();
            if (tmpR != null)
            {
                Vector3 tmp = tmpR.bounds.size;
                if (sizeCalculated.magnitude < tmp.magnitude)
                    sizeCalculated = tmp;
            }
        }
        add.onClick.AddListener(() => ButtonAdd());
        add.gameObject.SetActive(false);
        button.gameObject.SetActive(false);
        parent = button.GetComponentInParent<Transform>();
        imagePanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void chooseEnivorment()
    {
        actionName = "Environments";
        imagePanel.SetActive(true);
        EnvLoader.SearchForEnvironments();
        loadObj = EnvLoader.LoadEnvironment;
        ListToCreatChoose(EnvLoader.environmentsFound);
    }
    public void chooseRobot()
    {
        actionName = "Robots";
        imagePanel.SetActive(true);
        RobotLoader.SearchForRobots();
        loadObj = RobotLoader.LoadRobotGameObject;
        ListToCreatChoose(RobotLoader.robotsFound);
    }
    public void chooseSave()
    {

    }

    void ListToCreatChoose(List<GameObject> list)
    {
        int maxCountElementInWidth = Mathf.RoundToInt((Screen.width - 150) / button.gameObject.GetComponent<RectTransform>().rect.width);
        int countElementInWidth = 0;
        int countElementInHeight = 0;

        foreach (GameObject gameObject in list)
        {
            if (maxCountElementInWidth == countElementInWidth)
            {
                countElementInWidth = 0;
                countElementInHeight++;
            }

            Vector3 position = button.transform.position + (new Vector3(offsetW * countElementInWidth++, offsetH * countElementInHeight, 0));
            Quaternion rotation = button.transform.rotation;
            Transform newButton = button.transform.Spawn(position, rotation);
            newButton.SetParent(parent);
            //SwapPrefabs(newButton.GetChild(0).gameObject, loadObj(gameObject.name));
            newButton.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(gameObject));
            newButton.localScale = button.transform.localScale;
            newButton.GetComponentInChildren<TMP_Text>().text = gameObject.name;

            newButton.gameObject.SetActive(true);
            objectsToDestroyWhenButton.Add(newButton.gameObject);
        }
        if (maxCountElementInWidth == countElementInWidth)
        {
            countElementInWidth = 0;
            countElementInHeight++;
        }
        add.transform.position = button.transform.position + (new Vector3(offsetW * countElementInWidth, offsetH * countElementInHeight, 0));
        add.gameObject.SetActive(true);
    }

    void SwapPrefabs(GameObject oldGameObject, GameObject GameObjectFromList)
    {
        
        Quaternion rotation = oldGameObject.transform.rotation;
        Vector3 position = oldGameObject.transform.position;

        Transform newGameObject = GameObjectFromList.transform.Spawn(position, rotation);
        if (oldGameObject.transform.parent != null)
        {
            newGameObject.transform.SetParent(oldGameObject.transform.parent);
        }

        DestroyImmediate(oldGameObject);
        DestroyImmediate(GameObjectFromList);
    }

    void ButtonAdd()
    {
        FileMenager.objectER = actionName;
        FileMenager.ModelImportBatchGUI();
        CloseAfterClick();
    }

    void ButtonClicked(GameObject gameObject)
    {
        if (gameObject.GetComponent<Robot>())
        {
            Simulation.namesRobotInSimulation.Add(gameObject.name);
        }
        else
        {
            Simulation.environment = gameObject;
        }
        CloseAfterClick();
    }

    public void CloseAfterClick()
    {
        imagePanel.SetActive(false);
        foreach (GameObject destroy in objectsToDestroyWhenButton)
        {
            DestroyImmediate(destroy);
        }
        objectsToDestroyWhenButton.Clear();
        add.gameObject.SetActive(false);
    }

    public void LoadSceneSimulation()
    {
        LevelLoader.Instance.LoadLevel(1);
        Simulation.robots.Clear();
        
    }
}