using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class hoverButton : MonoBehaviour
{
    bool ishover = false;
    RectTransform parent, button;
    public int leftOrRight = 1;
    TextMeshProUGUI textMeshPro;


    void Start()
    {
        parent = transform.parent.GetComponent<RectTransform>();
        button = transform.GetComponent<RectTransform>();
        textMeshPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void Click()
    {
        if(!ishover)
        {
            parent.localPosition += new Vector3((parent.rect.width + 10)* leftOrRight, 0,0);
            button.localPosition += new Vector3(-(button.rect.width + 10) * leftOrRight, 0, 0);
            ishover = true;
            textMeshPro.text = textMeshPro.text != "->"? "->": "<-";
        }
        else
        {
            parent.localPosition += new Vector3(-(parent.rect.width + 10) * leftOrRight, 0, 0);
            button.localPosition += new Vector3((button.rect.width +10) * leftOrRight, 0, 0);
            ishover = false;
            textMeshPro.text = textMeshPro.text != "->" ? "->" : "<-";
        }
    }
}
