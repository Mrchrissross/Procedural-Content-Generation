using UnityEngine;

public class GenerateWaves : MonoBehaviour
{
    Vector3 waveSource = new Vector3(2.0f, 0.0f, 2.0f);
    public float waveFrequency = 0.53f;
    public float waveHeight = 0.48f;
    public float waveLength = 0.71f;
    Mesh meshPrefab;
    Vector3[] verts;

    private Mesh mesh;

    void Start()
    {
        Camera.main.depthTextureMode |= DepthTextureMode.Depth;
        MeshFilter mf = GetComponent<MeshFilter>();
        mesh = mf.mesh;
        verts = mesh.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        CalcWave();
    }

    void CalcWave()
    {
        var uvs = mesh.uv;

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];
            v.y = 0.0f;
            float dist = Vector3.Distance(v, waveSource);
            dist = (dist % waveLength) / waveLength;
            v.y = waveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f * waveFrequency
            + (Mathf.PI * 2.0f * dist));
            verts[i] = v;
        }

        mesh.uv = uvs;
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.MarkDynamic();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}