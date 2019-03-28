using UnityEngine;

public class GenerateSailWaves : MonoBehaviour
{
    Vector3 waveSource = new Vector3(2.0f, 0.0f, 2.0f);
    public float waveFrequency = 0.4f;
    public float waveHeight = 0.2f;
    public float waveLength = 0.7f;
    Mesh meshPrefab;
    Vector3[] verts;
    Vector3[] oldVerts;

    private Mesh mesh;

    void Start()
    {
        Camera.main.depthTextureMode |= DepthTextureMode.Depth;
        MeshFilter mf = GetComponent<MeshFilter>();
        mesh = mf.mesh;

        oldVerts = new Vector3[mesh.vertices.Length];
        System.Array.Copy(mesh.vertices, oldVerts, mesh.vertices.Length);

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
            Vector3 v = oldVerts[i];
            //v.y = 0.0f;
            float dist = Vector3.Distance(v, waveSource);
            dist = (dist % waveLength) / waveLength;
            v.y += waveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f * waveFrequency
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