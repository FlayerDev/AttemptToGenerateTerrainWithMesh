using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector2[] uvs;

    Vector3[] vertices;
    int[] triangles;

    public bool updateMesh = false;
    public bool updateConst = false;

    [Range(.1f, .9f)] public float GroundLevel = 0.5f;
    [Range(.01f, .5f)] public float LakeLevel = .15f;

    public float MapScale = 1.0f;

    Vector2 Scale
    {
        get 
        { 
           return new Vector2( MapScale * (xSize / 64), MapScale * (zSize / 64)); 
        }

    }

    public float Seed = 0;

    public int xSize = 20;
    public int zSize = 20;

    public float Size = 100f;


    void Start()
    {
        MeshInitializer();
    }
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

    void MeshInitializer()
    {  
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y;
                y = Mathf.PerlinNoise(x / (float)xSize * Scale.x + Seed, z / (float)zSize * Scale.y + Seed) * Size;
                if (y < GroundLevel * Size && y > LakeLevel * Size)
                {
                    y = GroundLevel * Size;
                }
                else if (y < LakeLevel * Size)
                {
                    y = y + (GroundLevel - LakeLevel )* Size;
                }
                
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
        uvs = new Vector2[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
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
