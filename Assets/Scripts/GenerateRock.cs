using System.Collections.Generic;
using UnityEngine;

using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;


public class GenerateRock : MonoBehaviour
{
    GameObject body;
    Mesh mesh;
    Vector3[] vertices;
    public Material[] rockMaterials;

    [Range(1, 8)]
    public int shapeMin = 2;
    [Range(1, 8)]
    public int shapeMax = 4;

    //The amplification of the noise
    [Range(0.25f, 1.75f)]
    public float lowestAmplification = 1.0f;
    [Range(2.5f, 7.5f)]
    public float highestAmplification = 5.0f;

    //The frequency of the noise, the scale of the noise (how frequent there are hill / valleys)
    [Range(0.05f, 0.5f)]
    public float lowestFrequency = 0.3f;
    [Range(0.5f, 1.5f)]
    public float highestFrequency = 1.0f;

    public float amplification;
    public float frequency;
    public int shape;

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

        shape = Random.Range(shapeMin, shapeMax + 1);

        mesh = GenerateIcoSphere.Create(shape, 1.0f);
        vertices = mesh.vertices;
    }

    void GenerateNoise()
    {
        float modifier;

        frequency = Random.Range(0.3f, 1.0f);
        amplification = Random.Range(1.0f, 5.0f);

        var noise = new LibNoise.Generator.Billow(frequency, lacu, persist, octaves, Random.Range(0, 0xffffff), QualityMode.High);
        //var noise = new LibNoise.Generator.RidgedMultifractal(frequency, lacu, octaves, Random.Range(0, 0xffffff), QualityMode.High);
        //var noise = new LibNoise.Generator.Voronoi(frequency, 0.5, Random.Range(0, 0xfffffff), true);
        //var noise = new LibNoise.Generator.Perlin(frequency, lacu, persist, octaves, Random.Range(0, 0xffffff), QualityMode.High);

        for (int i = 0; i < vertices.Length; i++)
        {
            modifier = (float)noise.GetValue(vertices[i].x, vertices[i].y, vertices[i].z);
            modifier = ((modifier - 0.5f) / amplification) + 0.99f;
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
        float x = Random.Range(0.25f, 1.0f);
        float y = Random.Range(0.25f, 1.0f);
        float z = Random.Range(0.25f, 1.0f);

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
