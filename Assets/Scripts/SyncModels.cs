using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum meshTypes { Car, FullChar, HalfChar };

public class SyncModels : MonoBehaviour
{
    [HideInInspector]

    public meshTypes meshType = meshTypes.Car;

    [HideInInspector] public GameObject FrontWheels;
    [HideInInspector] public GameObject BackWheels;

    private GameObject Manager;
    private MeshFilter meshFilter;
    private Mesh currentmesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        UpdateMesh();
    }
    private void Update()
    {
        UpdateMesh();
    }
    bool SetManager()
    {
        switch (meshType)
        {
            case meshTypes.Car:
                Manager = GameObject.Find("CarManager");
                break;
            default:
                Manager = GameObject.Find("CharManager");
                break;
        }
        if (Manager)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void UpdateMesh()
    {
        if (SetManager())
        {
            switch (meshType)
            {
                case meshTypes.Car:
                    Manager = GameObject.FindGameObjectWithTag("CarManager");
                    currentmesh = Manager.GetComponent<CarManager>().Cars[PlayerPrefs.GetInt("CURRENTCARID")];
                    if (FrontWheels != null && BackWheels != null)
                    {
                        FrontWheels.transform.localPosition = Manager.GetComponent<CarManager>().FrontWheelloc[PlayerPrefs.GetInt("CURRENTCARID")];
                        BackWheels.transform.localPosition = Manager.GetComponent<CarManager>().Backhweelloc[PlayerPrefs.GetInt("CURRENTCARID")];
                    }
                    break;
                case meshTypes.FullChar:
                    //currentmesh = Manager.GetComponent<CharacterManager>().characters[PlayerPrefs.GetInt("CURRENTCHARID", 0)].fullChar;
                    break;
                case meshTypes.HalfChar:
                    //currentmesh = Manager.GetComponent<CharacterManager>().characters[PlayerPrefs.GetInt("CURRENTCHARID", 0)].halfChar;
                    break;
                default:
                    break;
            }
            meshFilter.mesh = currentmesh;
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(SyncModels))]
public class SyncModelsEditor : Editor
{
    public GameObject source;
    public meshTypes op;

    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var Script = (SyncModels)target;

        // draw checkbox for the bool
        Script.meshType = (meshTypes)EditorGUILayout.EnumPopup("My type", Script.meshType);

        //Script.meshType = EditorGUILayout.Toggle("Start Temp", Script.meshType);
        if (Script.meshType == meshTypes.Car) // if bool is true, show other fields
        {
            Script.FrontWheels = (GameObject)EditorGUILayout.ObjectField("MyObj", source, typeof(GameObject), true);
            Script.BackWheels = (GameObject)EditorGUILayout.ObjectField("MyObj", source, typeof(GameObject), true);
        }
    }
}
#endif
