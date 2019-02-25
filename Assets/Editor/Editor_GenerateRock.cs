// This is required to allow the buttons to appear on the Japan tower Generator.

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateRock))]
public class Editor_GenerateRock : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateRock myScript = (GenerateRock)target;
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