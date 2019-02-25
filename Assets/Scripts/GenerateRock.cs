using UnityEngine;

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

    float   scale = 10,
            frequency = 0.65f,
            amplification = 3.1f;
                    

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

        int level = Random.Range(shapeMin, shapeMax + 1);

        mesh = GenerateIcoSphere.Create(level, 1.0f);
        vertices = mesh.vertices;
    }

    void GenerateNoise()
    {
        float modifier = 1;

        scale = Random.Range(2.5f, 10.0f);
        frequency = Random.Range(0.05f, 1.0f);
        amplification = Random.Range(1.0f, 5.0f);

        for (int i = 0; i < vertices.Length; i++)
        {
            modifier = Simplex.Noise.CalcPixel3D((int)(vertices[i].x * scale), (int)(vertices[i].y * scale), (int)(vertices[i].z * scale), frequency);
            modifier /= 256;

            float truefudge = .99f + ((modifier - .5f) / amplification);

            vertices[i] = Vector3.Scale(vertices[i], (Vector3.one * truefudge));
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
        DestroyImmediate(body);

        this.name = "SpawnRock";
    }
}
