using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateJapanTower))]
public class Editor_GenerateJapanTower : Editor
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