using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Water : MonoBehaviour {
    public Shader waterShader;

    public int quadRes = 10;

    private Material waterMaterial;
    private Mesh mesh;
    private Vector3[] vertices;

    private void CreateWaterPlane() {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Water";

        float halfRes = quadRes * 0.5f;
        vertices = new Vector3[(quadRes + 1) * (quadRes + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, x = 0; x <= quadRes; ++x) {
            for (int z = 0; z <= quadRes; ++z, ++i) {
                vertices[i] = new Vector3(x - halfRes, 0, z - halfRes);
                uv[i] = new Vector2((float)x / quadRes, (float)z / quadRes);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[quadRes * quadRes * 6];

        for (int ti = 0, vi = 0, x = 0; x < quadRes; ++vi, ++x) {
            for (int z = 0; z < quadRes; ti += 6, ++vi, ++z) {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + 1;
                triangles[ti + 2] = vi + quadRes + 2;
                triangles[ti + 3] = vi;
                triangles[ti + 4] = vi + quadRes + 2;
                triangles[ti + 5] = vi + quadRes + 1;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void CreateMaterial() {
        if (waterShader == null) return;
        if (waterMaterial != null) return;

        waterMaterial = new Material(waterShader);
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        renderer.material = waterMaterial;
    }

    void OnEnable() {
        CreateWaterPlane();
        CreateMaterial();
    }

    void Update() {
        
    }

    void OnDisable() {
        if (waterMaterial != null) {
            Destroy(waterMaterial);
            waterMaterial = null;
        }

        if (mesh != null) {
            Destroy(mesh);
            mesh = null;
            vertices = null;
        }
    }

    private void OnDrawGizmos() {
        if (vertices == null) return;

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; ++i)
            Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
    }
}
