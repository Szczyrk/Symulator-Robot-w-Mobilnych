using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatScene : MonoBehaviour
{
    public Button buttonEdit;
    // Start is called before the first frame update
    void Start()
    {

        Simulation.Starting();
        Simulation.StartSimulation();
        if (buttonEdit == null)
        {
            buttonEdit = GameObject.Find("Canvas/Panel/buttonEdit").GetComponent<Button>();
            buttonEdit.onClick.AddListener(EditStart);
        }
        else
            buttonEdit.onClick.AddListener(EditStart);
    }
    private void EditStart()
    {
        if (Simulation.robotSelected != null)
            if (Simulation.state != Simulation.State.edit)
            {
                Simulation.state = Simulation.State.edit;
                IndividualEdit.StartIndividualEdit(Simulation.robotSelected.gameObject);
            }
            else
            {
                Simulation.state = Simulation.State.starting;
                IndividualEdit.BackToSimulation();
                Simulation.robotSelected.ShowVariables();
            }
    }
}
