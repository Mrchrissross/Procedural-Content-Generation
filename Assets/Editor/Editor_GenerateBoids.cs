using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateBoids))]
public class Editor_GenerateBoids : Editor
{
    int selection = 1;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenerateBoids myScript = (GenerateBoids)target;

        if (myScript.bird && selection != 1)
        {
            myScript.ship = false;
            myScript.fish = false;
            selection = 1;
        }

        else if (myScript.ship && selection != 2)
        {
            myScript.bird = false;
            myScript.fish = false;
            selection = 2;
        }

        else if (myScript.fish && selection != 3)
        {
            myScript.bird = false;
            myScript.ship = false;
            selection = 3;
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