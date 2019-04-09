using System.Collections.Generic;
using UnityEngine;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;


public class GenerateRock : MonoBehaviour
{
    [HideInInspector]
    public GameObject body;
    Mesh mesh;
    Vector3[] vertices;
    public Material[] rockMaterials;

    [Header("Shape & Size")]

    //How many levels are within the shape
    [VectorLabels("Min", "Max")]
    public Vector2Int shape = new Vector2Int(2, 4);

    [VectorLabels("Min", "Max")]
    public Vector2 _x = new Vector2(0.25f, 1.0f);
    [VectorLabels("Min", "Max")]
    public Vector2 _y = new Vector2(0.25f, 1.0f);
    [VectorLabels("Min", "Max")]
    public Vector2 _z = new Vector2(0.25f, 1.0f);

    [Header("Noise")]

    //The amplification of the noise
    [VectorLabels("Min", "Max")]
    public Vector2 amplification = new Vector2(1.0f, 5.0f);

    //The frequency of the noise, the scale of the noise (how frequent there are hill / valleys)
    [VectorLabels("Min", "Max")]
    public Vector2 frequency = new Vector2(0.3f, 1.0f);

    //The lacunarity of the noise, basically, the amount of holes in it
    float lacu = 1f;

    //The number of octaves (the number of noise maps added together)
    int octaves = 1;

    //The persistance of the noise (the difference in value between each octave)
    float persist = 1f;

    public void Generate()
    {
        DestroyObject();

        GenerateMesh();

        GenerateNoise();

        SetMesh();

        Resize();
    }

    void GenerateMesh()
    {
        body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        if (body)
        {
            this.name = "Rock";
            body.transform.parent = transform;
            body.name = "body";
            body.transform.localPosition = Vector3.zero;
            DestroyImmediate(body.GetComponent<SphereCollider>());
        }
        else
        {
            Debug.LogError("Failed to created body on " + this.name + " object.");
            return;
        }

        int newShape = Random.Range(shape.x, shape.y + 1);

        mesh = GenerateIcoSphere.Create(newShape, 1.0f);
        vertices = mesh.vertices;
    }

    void GenerateNoise()
    {
        float modifier;

        float newFrequency = Random.Range(frequency.x, frequency.y);
        float newAmplification = Random.Range(amplification.x, amplification.y);

        var noise = new LibNoise.Generator.Billow(newFrequency, lacu, persist, octaves, Random.Range(0, 0xffffff), QualityMode.High);

        for (int i = 0; i < vertices.Length; i++)
        {
            modifier = (float)noise.GetValue(vertices[i].x, vertices[i].y, vertices[i].z);
            modifier = ((modifier - 0.5f) / newAmplification) + 0.99f;
            vertices[i] = Vector3.Scale(vertices[i], (Vector3.one * modifier));
        }
    }

    void SetMesh()
    {
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        body.GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    void Resize()
    {
        float x = Random.Range(_x.x, _x.y);
        float y = Random.Range(_y.x, _y.x);
        float z = Random.Range(_z.x, _z.x);

        body.transform.localScale = new Vector3(x, y, z);

        int mat = Random.Range(0, rockMaterials.Length);

        if(rockMaterials[mat])
            body.GetComponent<MeshRenderer>().material = rockMaterials[mat];
        else
            Debug.LogError("Material " + mat +  " is not assigned on " + this.name + " object.");

        body.AddComponent<MeshCollider>().convex = true;
    }

    public void DestroyObject()
    {
        List<GameObject> objectsToDelete = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            objectsToDelete.Add(transform.GetChild(i).gameObject);
        }

        foreach (GameObject item in objectsToDelete)
            DestroyImmediate(item);

        this.name = "SpawnRock";
    }
}
