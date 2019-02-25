// This is required to allow the buttons to appear on the Japan tower Generator.

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateJapanTower))]
public class Editor_GenerateTower : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateJapanTower myScript = (GenerateJapanTower)target;
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