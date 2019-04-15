using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateWorld))]
public class Editor_GenerateWorld : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenerateWorld myScript = (GenerateWorld)target;

        if (!myScript.generateJapaneseTower)
            myScript.generateHouses = false;

        //if (myScript.generateHouses && !myScript.generateJapaneseTower)
        //    myScript.generateJapaneseTower = true;
    }
}