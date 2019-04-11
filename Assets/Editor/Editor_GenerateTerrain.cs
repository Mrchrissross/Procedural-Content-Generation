using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateTerrain))]
public class Editor_GenerateTerrain : Editor
{
    float seaLevelMin = 10.0f;
    float seaLevelMax = 40.0f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenerateTerrain myScript = (GenerateTerrain)target;

        myScript.seaLevel = Float_CheckBoundary(myScript.seaLevel, seaLevelMin, seaLevelMax);

        if (GUILayout.Button("Generate"))
            myScript.Generate();

        if (GUILayout.Button("Destroy Object"))
            myScript.DestroyObject();
    }

    Vector2 Float_CheckBoundary(Vector2 variable, float min, float max)
    {
        if (variable.x < min)
            variable.x = min;

        if (variable.y < min)
            variable.y = min;

        if (variable.y > max)
            variable.y = max;

        if (variable.x > variable.y)
            variable.x = variable.y;

        return variable;
    }
}