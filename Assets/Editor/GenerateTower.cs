using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateJapanTower))]
public class GenerateTower : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateJapanTower myScript = (GenerateJapanTower)target;
        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }
    }
}