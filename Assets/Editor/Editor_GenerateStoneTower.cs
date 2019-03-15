using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateStoneTower))]
public class Editor_GenerateStoneTower : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateStoneTower myScript = (GenerateStoneTower)target;
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