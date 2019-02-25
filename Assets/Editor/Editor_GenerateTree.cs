// This is required to allow the buttons to appear on the Japan tower Generator.

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateTree))]
public class Editor_GenerateTree : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateTree myScript = (GenerateTree)target;
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