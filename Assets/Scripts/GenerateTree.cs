using System.Collections.Generic;
using UnityEngine;
using LibNoise;

public class GenerateTree : MonoBehaviour
{
    [System.Serializable]
    public class TowerParts
    {
        public GameObject Empty;
        public GameObject Trunk;
        public Material leafMaterial;
    };

    public TowerParts parts;

    //How many levels are within the shape
    [VectorLabels("Min", "Max")]
    public Vector2Int shape = new Vector2Int(2, 4);

    //The amplification of the noise
    [VectorLabels("Min", "Max")]
    public Vector2 amplification = new Vector2(1.0f, 5.0f);

    //The frequency of the noise, the scale of the noise (how frequent there are hill / valleys)
    [VectorLabels("Min", "Max")]
    public Vector2 frequency = new Vector2(0.3f, 1.0f);

    bool ExtraTrunk;

    GameObject body;
    Mesh mesh;
    Vector3[] vertices;

    private GameObject tempEmpty;
    private GameObject tempTrunk;

    private List<GameObject> bones = new List<GameObject>();
    private List<GameObject> spawnPoints = new List<GameObject>();

    public void Generate()
    {
        DestroyObject();

        GenerateBase();

        ExtraTrunk = true;

        for(int i = 0; i < 3; i++)
            GenerateBase();

        ExtraTrunk = false;
    }

    void GenerateBase()
    {
        GenerateTrunk();

        SortChildren();

        ModifyTrunk();

        GenerateLeaves();
    }

    void GenerateTrunk()
    {
        tempEmpty = Instantiate(parts.Empty, this.transform);
        if (tempEmpty)
        {
            this.name = "Tree";
            tempEmpty.name = "Body";
            tempEmpty.transform.localPosition = Vector3.zero;
        }
        else
            Debug.LogError("The empty prefab has not been set on " + this.name + " object.");

        tempTrunk = Instantiate(parts.Trunk, tempEmpty.transform);
        if (tempTrunk)
            tempTrunk.name = "Trunk";
        else
            Debug.LogError("The trunk prefab has not been set on " + this.name + " object.");
    }

    void SortChildren()
    {
        foreach (Transform child in tempTrunk.GetComponentsInChildren<Transform>(true))
        {
            bones.Add(child.gameObject);
        }

        for (int i = 0; i < bones.Count; i++)
        {
            if (bones[i].name == "Trunk")
                bones.Remove(bones[i]);

            if (bones[i].name == "Bone")
                bones.Remove(bones[i]);

            if (bones[i].name == "BranchSpawn")
            {
                spawnPoints.Add(bones[i]);
                bones.Remove(bones[i]);
            }
        }
    }

    void ModifyTrunk()
    {
        float   trunkHeight = Random.Range(0.5f, 0.8f),
                trunkWidth = Random.Range(0.7f, 1.5f);

        tempTrunk.transform.localScale = new Vector3(trunkWidth, trunkWidth, trunkHeight);

        float   offsetY = Random.Range(-0.15f, 0.15f),
                offsetZ = Random.Range(-0.15f, 0.15f);

        foreach (GameObject bone in bones)
        {
            bone.transform.localPosition = new Vector3(bone.transform.localPosition.x, offsetY, offsetZ);
        }
    }

    void GenerateLeaves()
    {
        GenerateLeafMesh();

        LeafResize();
    }

    void GenerateLeafMesh()
    {
        body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        if (body)
        {
            body.name = "Leaf";
            DestroyImmediate(body.GetComponent<SphereCollider>());
        }
        else
        {
            Debug.LogError("Failed to created body on " + this.name + " object.");
            return;
        }

        int newShape = Random.Range(shape.x, shape.y);

        mesh = GenerateIcoSphere.Create(newShape, 1.0f);
        vertices = mesh.vertices;

        GenerateLeafNoise();

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        body.GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    void GenerateLeafNoise()
    {
        float   modifier,
                lacu = 1.5f;

        int     octaves = 1;

        float newFrequency = Random.Range(frequency.x, frequency.y);
        float newAmplification = Random.Range(amplification.x, amplification.y);

        var noise = new LibNoise.Generator.RidgedMultifractal(newFrequency, lacu, octaves, Random.Range(0, 0xffffff), QualityMode.High);

        for (int i = 0; i < vertices.Length; i++)
        {
            modifier = (float)noise.GetValue(vertices[i].x, vertices[i].y, vertices[i].z);
            modifier = ((modifier - 0.5f) / newAmplification) + 0.99f;
            vertices[i] = Vector3.Scale(vertices[i], (Vector3.one * modifier));
        }
    }

    void LeafResize()
    {
        float x = Random.Range(1.25f, 2.0f);
        float y = Random.Range(1.25f, 2.0f);
        float z = Random.Range(1.25f, 2.0f);

        body.transform.localScale = new Vector3(x, y, z);
        body.transform.parent = spawnPoints[spawnPoints.Count - 1].transform;
        body.transform.localPosition = Vector3.zero;

        if (parts.leafMaterial)
            body.GetComponent<MeshRenderer>().material = parts.leafMaterial;
        else
            Debug.LogError("Leaf material is not assigned on " + this.name + " object.");

        if(!ExtraTrunk)
        {
            tempTrunk.AddComponent<BoxCollider>();
            body.AddComponent<BoxCollider>();
        }
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

        this.name = "SpawnTree";

        bones.Clear();
        spawnPoints.Clear();
    }
}
