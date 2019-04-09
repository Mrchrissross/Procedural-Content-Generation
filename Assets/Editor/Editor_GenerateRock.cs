using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateRock))]
public class Editor_GenerateRock : Editor
{
    //Ranges
    int shapeMin = 2;
    int shapeMax = 8;
    float freqMin = 0.05f;
    float freqMax = 1.5f;
    float ampMin = 0.25f;
    float ampMax = 7.5f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenerateRock myScript = (GenerateRock)target;

        myScript.shape = Int_CheckBoundary(myScript.shape, shapeMin, shapeMax);
        myScript.frequency = Float_CheckBoundary(myScript.frequency, freqMin, freqMax);
        myScript.amplification = Float_CheckBoundary(myScript.amplification, ampMin, ampMax);

        #region Sizes

        if (myScript._x.x > myScript._x.y)
            myScript._x.x = myScript._x.y;

        if (myScript._y.x > myScript._y.y)
            myScript._y.x = myScript._y.y;

        if (myScript._z.x > myScript._z.y)
            myScript._z.x = myScript._z.y;

        #endregion

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