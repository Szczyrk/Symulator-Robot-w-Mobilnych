using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class DeleteButtonMenu : MonoBehaviour
{
    public Button buttonDelete;
    void Start()
    {
        buttonDelete.onClick.AddListener(Delete);
    }

    // Update is called once per frame
    void Delete()
    {
        string PrefabFolderPath = "Assets" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar;
        string ModelFolderPath = "Assets" + Path.DirectorySeparatorChar + "Models" + Path.DirectorySeparatorChar;

        string EnivormentPrefab = PrefabFolderPath + "Environments"
            + Path.DirectorySeparatorChar + this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text + ".prefab";
        string EnivormentModels = ModelFolderPath + "Environments"
                + Path.DirectorySeparatorChar + this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
       
        if (Directory.Exists(EnivormentModels))
        {
            Directory.Delete(EnivormentModels, true);
            Debug.Log("Deleted: " + EnivormentModels);

        }
        if (File.Exists(EnivormentPrefab))
        {
            AssetDatabase.DeleteAsset(EnivormentPrefab);
            Debug.Log("Deleted: " + EnivormentPrefab);
        }

        string RobotPrefab = PrefabFolderPath + "Robots"
            + Path.DirectorySeparatorChar + this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text + ".prefab";
        string RobotModels = ModelFolderPath + "Robots"
                + Path.DirectorySeparatorChar + this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        if (Directory.Exists(RobotModels))
        {
            Directory.Delete(RobotModels, true);
            Debug.Log("Deleted: " + RobotModels);
        }
        if (File.Exists(RobotPrefab))
        {
            AssetDatabase.DeleteAsset(RobotPrefab);
            Debug.Log("Deleted: " + RobotPrefab);
        }
        Destroy(this.gameObject);
    }
}
