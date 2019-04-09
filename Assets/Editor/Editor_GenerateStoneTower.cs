using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateStoneTower))]
public class Editor_GenerateStoneTower : Editor
{
    float sizeMin = 2.5f;
    float sizeMax = 5.5f;
    float heightMin = 0.5f;
    float heightMax = 2.5f;
    float orbScMin = 0.5f;
    float orbScMax = 2.0f;
    int rCountMin = 4;
    int rCountMax = 15;
    float rRotMin = 0.5f;
    float rRotMax = 7.0f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenerateStoneTower myScript = (GenerateStoneTower)target;

        myScript.size = Float_CheckBoundary(myScript.size, sizeMin, sizeMax);
        myScript.height = Float_CheckBoundary(myScript.height, heightMin, heightMax);
        myScript.orbScale = Float_CheckBoundary(myScript.orbScale, orbScMin, orbScMax);
        myScript.rockCount = Int_CheckBoundary(myScript.rockCount, rCountMin, rCountMax);
        myScript.rockRotation = Float_CheckBoundary(myScript.rockRotation, rRotMin, rRotMax);

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