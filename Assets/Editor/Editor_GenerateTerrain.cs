using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateTerrain))]
public class Editor_GenerateTerrain : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateTerrain myScript = (GenerateTerrain)target;
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