using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateJapanTower))]
public class Editor_GenerateJapanTower : Editor
{
    int minLevels = 1;
    int maxLevels = 10;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenerateJapanTower myScript = (GenerateJapanTower)target;

        myScript.levels = Int_CheckBoundary(myScript.levels, minLevels, maxLevels);

        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }

        if (GUILayout.Button("Destroy Object"))
        {
            myScript.DestroyObject();
        }
    }

    Vector2Int Int_CheckBoundary(Vector2Int variable, int min, int max)
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