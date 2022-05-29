using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Mathf;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector2[] uvs;

    Vector3[] vertices;
    int[] triangles;

    public ChunkManager manager;

    public bool updateMesh = false;

    public int xOffset = 0;
    public int zOffset = 0;
    float Scale;
    float GroundLevel;
    float LakeLevel;
    float Seed;
    int Dim;
    float Size;

#if UNITY_EDITOR
    private void FixedUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            CreateShape();
            UpdateMesh();
            print("MeshRecreated");
        }
    }
#endif

    public void MeshInitializer()
    {
        manager = GetComponentInParent<ChunkManager>();
        Dim = manager.Dimentions;
        Dim = manager.Dimentions;
        Seed = manager.Seed;
        GroundLevel = manager.GroundLevel;
        LakeLevel = manager.LakeLevel;
        Scale = manager.Scale;
        Size = manager.Size;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(Dim + 1) * (Dim + 1)];

        for (int i = 0, z = 0; z <= Dim; z++)
        {
            for (int x = 0; x <= Dim; x++)
            {
                float y;
                y = Mathf.PerlinNoise((x / (float)Dim * Scale) + Seed + (float)xOffset, (z / (float)Dim * Scale) + Seed+ (float)zOffset) * Size;
                if (y < GroundLevel * Size && y > LakeLevel * Size)
                {
                    y = PerlinNoise((x+transform.position.x)/.55f, (z + transform.position.z) / .55f)/2 + GroundLevel * Size;
                }
                else if (y < LakeLevel * Size)
                {
                    y = y + (GroundLevel - LakeLevel) * Size;
                }

                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[Dim * Dim * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < Dim; z++)
        {
            for (int x = 0; x < Dim; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + Dim + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + Dim + 1;
                triangles[tris + 5] = vert + Dim + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
        uvs = new Vector2[vertices.Length];
        for (int i = 0, z = 0; z <= Dim; z++)
        {
            for (int x = 0; x <= Dim; x++)
            {
                uvs[i] = new Vector2((float)x / Dim, (float)z / Dim);
                i++;
            }
        }
    }
    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        SetCollider();
    }
    void SetCollider()
    {
        MeshCollider meshc = gameObject.GetComponent<MeshCollider>();
        meshc.sharedMesh = null;
        meshc.sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
    }
}
