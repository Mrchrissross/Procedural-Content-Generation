using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratePirateShip))]
public class Editor_GeneratePirateShip : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GeneratePirateShip myScript = (GeneratePirateShip)target;
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