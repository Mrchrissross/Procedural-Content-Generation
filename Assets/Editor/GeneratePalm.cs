// This is required to allow the buttons to appear on the Japan tower Generator.

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratePalmTree))]
public class GeneratePalm : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GeneratePalmTree myScript = (GeneratePalmTree)target;
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