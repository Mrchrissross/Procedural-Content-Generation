using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratePalmTree))]
public class Editor_GeneratePalm : Editor
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