using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FileMenager : MonoBehaviour
{
    string path;
    public static string objectER;

    static void  AddModel3D(Context context)
    {
       GameObject enivorment = Resources.Load<GameObject>(context.modelFilePath);
        Transform env = enivorment.transform.Spawn();
       env.position = enivorment.transform.position;
    }

    #region Internal
    private static int count = 0;

    private class Context
    {
        public string modelFilePath;
        public string modelFileName;
        public string modelName;

        public string tempFolderPath;

        public Context(string _modelFilePath)
        {
            Initialize(_modelFilePath);
        }

        private void Initialize(string _modelFilePath)
        {
            count++;

            modelFilePath = _modelFilePath;
            modelFileName = Path.GetFileName(modelFilePath);

            modelName = Path.GetFileNameWithoutExtension(modelFilePath);

            tempFolderPath = Path.GetTempPath() + Path.DirectorySeparatorChar + "tmpImportModel" + count;

            if (Directory.Exists(tempFolderPath))
                Directory.Delete(tempFolderPath, true);
            Directory.CreateDirectory(tempFolderPath);
        }

    }
    private static void DoTempCopy(Context ctx)
    {
        File.Copy(ctx.modelFilePath, ctx.tempFolderPath + Path.DirectorySeparatorChar + ctx.modelFileName);

    }

    private static void DeletePreviousAssets(Context ctx, string assetFolderPath)
    {
        foreach (string filePath in Directory.GetFiles(assetFolderPath))
        {
            // Only delete existing assets. This preserves any later added assets to a model folder
            if (File.Exists(ctx.tempFolderPath + Path.DirectorySeparatorChar + Path.GetFileName(filePath)))
                AssetDatabase.DeleteAsset(filePath);
        }
    }

    private static void ImportModels(Context ctx)
    {
        string ModelFolderPath = "Assets" + Path.DirectorySeparatorChar + "Models" + Path.DirectorySeparatorChar + objectER 
            + Path.DirectorySeparatorChar + ctx.modelName;
        if (!Directory.Exists(ModelFolderPath))
            Directory.CreateDirectory(ModelFolderPath);
        else
            DeletePreviousAssets(ctx, ModelFolderPath);

        foreach (string filePath in Directory.GetFiles(ctx.tempFolderPath))
        {
            string fileName = Path.GetFileName(filePath);

            File.Copy(filePath, ModelFolderPath + Path.DirectorySeparatorChar + fileName);
        }
        foreach (string filePath in Directory.GetFiles(ModelFolderPath))
        {
            if (!File.Exists(filePath + ".meta"))
                if (Path.GetExtension(filePath) != ".meta")
                {
                    AssetDatabase.ImportAsset(filePath);

                    ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(filePath);
                    if (!modelImporter.isReadable)
                    {
                        modelImporter.isReadable = true;
                        modelImporter.SaveAndReimport();
                    }
                }
        }
    }

    private static void DeleteTempFolder(Context ctx)
    {
        Directory.Delete(ctx.tempFolderPath, true);
    }

    private static void internalModelImport(Context ctx)
    {
        DoTempCopy(ctx);
        ImportModels(ctx);
        DeleteTempFolder(ctx);
        ConvertFBXtoPrefab(ctx);
    }

    private static void ConvertFBXtoPrefab(Context ctx)
    {
        string PrefabFolderPath  = "Assets" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar +
            objectER;
        string ModelFolderPath = "Assets" + Path.DirectorySeparatorChar + "Models" + Path.DirectorySeparatorChar + objectER
            + Path.DirectorySeparatorChar + ctx.modelName;

        if (!Directory.Exists(PrefabFolderPath))
            Directory.CreateDirectory(PrefabFolderPath);
        foreach (string filePath in Directory.GetFiles(ModelFolderPath))
        {
            if (Path.GetExtension(filePath) == ".meta" || Path.GetExtension(filePath) == ".prefab")
                continue;
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            var objToPrefab = AssetDatabase.LoadAssetAtPath(filePath,typeof(GameObject));
            string filePathWithoutExtension = Path.GetDirectoryName(filePath)+ fileName;
            int i = 1;
            string fileNameTMP = fileName;
            while (File.Exists(filePathWithoutExtension + ".prefab"))
                fileName = fileNameTMP + "_" + i++;
            var instanceRoot = (GameObject)PrefabUtility.InstantiatePrefab(objToPrefab);
            AddCollider(instanceRoot);
            if (objectER == "Environments")
                AddComponentFromEnvironments(instanceRoot);
            else
                AddComponentFromRobots(instanceRoot);
            foreach (Camera camera in instanceRoot.GetComponentsInChildren<Camera>())
                camera.enabled = false;
            //var variantRoot = PrefabUtility.SaveAsPrefabAssetAndConnect(instanceRoot, PrefabFolderPath + Path.DirectorySeparatorChar + fileName + ".prefab", InteractionMode.UserAction);
            PrefabUtility.SaveAsPrefabAsset(instanceRoot, PrefabFolderPath + Path.DirectorySeparatorChar + fileName + ".prefab");

           Destroy(instanceRoot);


        }
       foreach (string filePath in Directory.GetFiles(PrefabFolderPath))
        {
            if (!File.Exists(filePath + ".meta"))
                if (Path.GetExtension(filePath) != ".meta")
                    AssetDatabase.ImportAsset(filePath);
        }
    }

    private static void AddCollider(GameObject variantRoot)
    {
        Apply(variantRoot.transform);
    }
    static void Apply(Transform t)
    {
        if (t.GetComponent<MeshRenderer>())
        {
            MeshCollider meshCollider= t.gameObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
        }

        // Recurse
        foreach (Transform child in t)
            Apply(child);
    }

    private static void AddComponentFromRobots(GameObject variantRoot)
    {
        variantRoot.AddComponent<Robot>();
     
    }

    private static void AddComponentFromEnvironments(GameObject variantRoot)
    {
         variantRoot.AddComponent<Environment>();
       
    }
    #endregion

    public static void ModelImport(string modelFilePath)
    {
        Context ctx = new Context(modelFilePath);
        internalModelImport(ctx);
    }
    public static void ModelImport(string[] modelFilePaths)
    {
        foreach (string modelFilePath in modelFilePaths)
        {
            ModelImport(modelFilePath);
        }
    }

    public static  void ModelImportBatchGUI()
    {
        string modelFilePath;
        bool done = false;

        string path = EditorUtility.OpenFilePanel("Select model file...", "", "FBX");
        modelFilePath=path;

        ModelImport(modelFilePath);
    }
}
