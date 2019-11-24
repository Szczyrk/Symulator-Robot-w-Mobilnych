using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddElementsToList : MonoBehaviour
{
    static GameObject listPanel;
    static Transform button;
    void Start()
    {
        listPanel = GameObject.Find("Canvas/PanelMain/RightPanel/Panel/Scroll View_Elements robot/Viewport/Content");
        button = this.transform.GetChild(0);
        button.gameObject.SetActive(false);
    }

   public static void Start_UI(GameObject robot)   
    {
        if (button == null)
            button = GameObject.Find("Canvas/PanelMain/RightPanel/Panel/Scroll View_Elements robot/Viewport/Content/Button").transform;
        if(listPanel == null)
            listPanel = GameObject.Find("Canvas/PanelMain/RightPanel/Panel/Scroll View_Elements robot/Viewport/Content");
        button.gameObject.SetActive(true);
        foreach (Transform child in listPanel.transform)
        {
            if(child != button)
                GameObject.Destroy(child.gameObject);
        }
        var elements = robot.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in elements)
        {
            AddButtonSelect(listPanel, meshRenderer.gameObject);
        }
        button.gameObject.SetActive(false);
    }
    static void AddButtonSelect(GameObject parent, GameObject elemet)
    {
        Transform newButton = button.transform.Spawn();
        newButton.SetParent(parent.transform);
        Button buttonCLick = newButton.GetComponent<Button>();
        buttonCLick.onClick.AddListener(() => ButtonActionSelect(elemet));
        newButton.GetComponentInChildren<TMP_Text>().text = elemet.name;
    }
    static void ButtonActionSelect(GameObject select)
    {
        IndividualEdit.ResetSelect();
        IndividualEdit.SelectObject(select);
    }

}
