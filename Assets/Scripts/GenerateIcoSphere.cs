﻿using UnityEngine;

public static class GenerateIcoSphere {


    public static Mesh Create(int n, float radius)
    {
        int nn = n * 4;
        int vertexNum = (nn * nn / 16) * 24;
        Vector3[] vertices = new Vector3[vertexNum];
        int[] triangles = new int[vertexNum];
        Vector2[] uv = new Vector2[vertexNum];

        Quaternion[] init_vectors = new Quaternion[24];
        // 0
        init_vectors[0] = new Quaternion(0, 1, 0, 0);   //the triangle vertical to (1,1,1)
        init_vectors[1] = new Quaternion(0, 0, 1, 0);
        init_vectors[2] = new Quaternion(1, 0, 0, 0);
        // 1
        init_vectors[3] = new Quaternion(0, -1, 0, 0);  //to (1,-1,1)
        init_vectors[4] = new Quaternion(1, 0, 0, 0);
        init_vectors[5] = new Quaternion(0, 0, 1, 0);
        // 2
        init_vectors[6] = new Quaternion(0, 1, 0, 0);   //to (-1,1,1)
        init_vectors[7] = new Quaternion(-1, 0, 0, 0);
        init_vectors[8] = new Quaternion(0, 0, 1, 0);
        // 3
        init_vectors[9] = new Quaternion(0, -1, 0, 0);  //to (-1,-1,1)
        init_vectors[10] = new Quaternion(0, 0, 1, 0);
        init_vectors[11] = new Quaternion(-1, 0, 0, 0);
        // 4
        init_vectors[12] = new Quaternion(0, 1, 0, 0);  //to (1,1,-1)
        init_vectors[13] = new Quaternion(1, 0, 0, 0);
        init_vectors[14] = new Quaternion(0, 0, -1, 0);
        // 5
        init_vectors[15] = new Quaternion(0, 1, 0, 0); //to (-1,1,-1)
        init_vectors[16] = new Quaternion(0, 0, -1, 0);
        init_vectors[17] = new Quaternion(-1, 0, 0, 0);
        // 6
        init_vectors[18] = new Quaternion(0, -1, 0, 0); //to (-1,-1,-1)
        init_vectors[19] = new Quaternion(-1, 0, 0, 0);
        init_vectors[20] = new Quaternion(0, 0, -1, 0);
        // 7
        init_vectors[21] = new Quaternion(0, -1, 0, 0);  //to (1,-1,-1)
        init_vectors[22] = new Quaternion(0, 0, -1, 0);
        init_vectors[23] = new Quaternion(1, 0, 0, 0);
        
        int j = 0;  //index on vectors[]

        for (int i = 0; i < 24; i += 3)
        {
            for (int p = 0; p < n; p++)
            {   
                //edge index 1
                Quaternion edge_p1 = Quaternion.Lerp(init_vectors[i], init_vectors[i + 2], (float)p / n);
                Quaternion edge_p2 = Quaternion.Lerp(init_vectors[i + 1], init_vectors[i + 2], (float)p / n);
                Quaternion edge_p3 = Quaternion.Lerp(init_vectors[i], init_vectors[i + 2], (float)(p + 1) / n);
                Quaternion edge_p4 = Quaternion.Lerp(init_vectors[i + 1], init_vectors[i + 2], (float)(p + 1) / n);

                for (int q = 0; q < (n - p); q++)
                {   
                    //edge index 2
                    Quaternion a = Quaternion.Lerp(edge_p1, edge_p2, (float)q / (n - p));
                    Quaternion b = Quaternion.Lerp(edge_p1, edge_p2, (float)(q + 1) / (n - p));
                    Quaternion c, d;
                    if(edge_p3 == edge_p4)
                    {
                        c = edge_p3;
                        d = edge_p3;
                    }else
                    {
                        c = Quaternion.Lerp(edge_p3, edge_p4, (float)q / (n - p - 1));
                        d = Quaternion.Lerp(edge_p3, edge_p4, (float)(q + 1) / (n - p - 1));
                    }

                    triangles[j] = j;
                    vertices[j++] = new Vector3(a.x, a.y, a.z);
                    triangles[j] = j;
                    vertices[j++] = new Vector3(b.x, b.y, b.z);
                    triangles[j] = j;
                    vertices[j++] = new Vector3(c.x, c.y, c.z);
                    if (q < n - p - 1)
                    {
                        triangles[j] = j;
                        vertices[j++] = new Vector3(c.x, c.y, c.z);
                        triangles[j] = j;
                        vertices[j++] = new Vector3(b.x, b.y, b.z);
                        triangles[j] = j;
                        vertices[j++] = new Vector3(d.x, d.y, d.z);
                    }
                }
            }
        }
        Mesh mesh = new Mesh();
        mesh.name = "IcoSphere";

        CreateUV(n, vertices, uv);
        for (int i = 0; i < vertexNum; i++)
        {
            vertices[i] *= radius;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        CreateTangents(mesh);

        return mesh;
    }

    static void CreateUV(int n, Vector3[] vertices, Vector2[] uv)
    {
        int tri = n * n;        // devided triangle count (1,4,9...)
        int uvLimit = tri * 6;  // range of wrap UV.x 

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];

            Vector2 textureCoordinates;
            if((v.x == 0f)&&(i < uvLimit))
            {
                textureCoordinates.x = 1f;
            }
            else
            {
                textureCoordinates.x = Mathf.Atan2(v.x, v.z) / (-2f * Mathf.PI);
            }

            if (textureCoordinates.x < 0f)
            {
                textureCoordinates.x += 1f;
            }

            textureCoordinates.y = Mathf.Asin(v.y) / Mathf.PI + 0.5f;
            uv[i] = textureCoordinates;
        }

        int tt = tri * 3;
        uv[0 * tt + 0].x = 0.875f;
        uv[1 * tt + 0].x = 0.875f;
        uv[2 * tt + 0].x = 0.125f;
        uv[3 * tt + 0].x = 0.125f;
        uv[4 * tt + 0].x = 0.625f;
        uv[5 * tt + 0].x = 0.375f;
        uv[6 * tt + 0].x = 0.375f;
        uv[7 * tt + 0].x = 0.625f;

    }

    static void CreateTangents(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Vector3[] normals = mesh.normals;

        int triangleCount = triangles.Length;
        int vertexCount = vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

        for (int i = 0; i < triangleCount; i += 3)
        {
            int i1 = triangles[i + 0];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = uv[i1];
            Vector2 w2 = uv[i2];
            Vector2 w3 = uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }


        for (int i = 0; i < vertexCount; ++i)
        {
            Vector3 n = normals[i];
            Vector3 t = tan1[i];

            Vector3.OrthoNormalize(ref n, ref t);
            tangents[i].x = t.x;
            tangents[i].y = t.y;
            tangents[i].z = t.z;

            tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
    }
}
