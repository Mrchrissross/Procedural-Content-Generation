using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateHouse))]
public class Editor_GenerateHouse : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateHouse myScript = (GenerateHouse)target;
        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }

        if (GUILayout.Button("Destroy Object"))
        {
            myScript.DestroyObject();
        }
    }
}