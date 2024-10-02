using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SceneUnderstanding : MonoBehaviour
{
    public string ExportScene(bool writeFile=false)
    {
        string sceneJson = SceneToJsonExporter.SerializeSceneToJson(true);
        string path = Application.dataPath + "/SceneJson/YourScene.json"; // Specify the path here
        if (writeFile) File.WriteAllText(path, sceneJson);
        Debug.Log("Scene exported to JSON at: " + path);

    #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh(); // Refresh the AssetDatabase to show the new file in Unity Editor
#endif

        return sceneJson;
    }

}



//Create a Data Structure for Serialization
public class GameObjectInfo
{
    public string name;
    //public string parentName;
    //public string position;
    //public string rotation; 
    public List<string> components = new List<string>(); // component it has been attached
    //public List<GameObjectInfo> children = new List<GameObjectInfo>(); // get children info

    // Constructor
    //public GameObjectInfo(string name, Vector3 positionV, Vector3 rotationV, List<string> components)
    //{
    //    this.name = name;
    //    //this.parentName = parentName;
    //    this.position = $"{positionV.x},{positionV.y},{positionV.z}";
    //    this.rotation = $"{rotationV.x},{rotationV.y},{rotationV.z}";
    //    this.components = components;
    //}

    public GameObjectInfo(string name, List<string> components)
    {
        this.name = name;
        //this.parentName = parentName;
        this.components = components;
    }
}


//Traverse the Scene Hierarchy
public static class SceneToJsonExporter
{
    public static List<GameObjectInfo> GetSceneObjects()
    {
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        List<GameObjectInfo> objectsInfo = new List<GameObjectInfo>();

        foreach (GameObject obj in rootObjects)
        {
            GameObjectInfo info = ProcessGameObject(obj);
            if (info != null)
            { // Make sure to only add if the GameObject wasn't excluded
                objectsInfo.Add(info);
            }
        }

        //objectsInfo include all the scene objects
        return objectsInfo;
    }

    //Filter game objects and record information, for individual game object
    private static GameObjectInfo ProcessGameObject(GameObject obj)
    {

        // Check if the GameObject should be excluded based on its tag
        if (obj.CompareTag("Ignore"))
        {
            return null; // Return null or handle as appropriate for your logic
        }

        Vector3 position = obj.transform.position; // Get position
        Vector3 rotation = obj.transform.eulerAngles; // Get rotation as Euler angles
        List<string> components = new List<string>(); // Get components

        foreach (Component component in obj.GetComponents<Component>())
        {
            components.Add(component.GetType().ToString());
        }

        //GameObjectInfo info = new GameObjectInfo(obj.name, parent?.name, position, rotation, components);
        GameObjectInfo info = new GameObjectInfo(obj.name, components);
        return info;
    }

    //Serialize to JSON
    public static string SerializeSceneToJson(bool clean=false)
    {
        //List<GameObjectInfo> sceneObjects = GetSceneObjects();
        //return JsonUtility.ToJson(new { objects = sceneObjects }, true);

        List<GameObjectInfo> sceneObjects = GetSceneObjects();
        if (sceneObjects.Count == 0)
        {
            Debug.LogError("No scene objects found to serialize.");
        }
        else
        {
            Debug.Log("Serializing " + sceneObjects.Count + " objects.");
        }
        //return JsonUtility.ToJson(new { objects = sceneObjects }, true);
        string result = JsonConvert.SerializeObject(new { objects = sceneObjects }, Formatting.Indented);
        if (clean) result = result.Replace(" ", "").Replace("\"", "").Replace("\n", "").Replace("\r", "").Replace(System.Environment.NewLine, "");
        return result;
    }
}




