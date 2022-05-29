using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject[,] ChunkGrid;
    public GameObject ChunkPrefab;
    public int gridSize = 512;
    public Vector3 userPos = new Vector2(0, 0);

    public int Seed = 1000;

    [Range(.1f, .9f)] public float GroundLevel = 0.5f;
    [Range(.01f, .5f)] public float LakeLevel = .15f;
    public float Size = 100f;
    public int Dimentions = 64;
    public int Scale = 1;

    public int RenderDistance = 12;
    // Start is called before the first frame update
    void Start()
    {
        ChunkGrid = new GameObject[gridSize*2,gridSize*2];
    }

    // Update is called once per frame
    void Update()
    {
        userPos = Camera.main.transform.position;
        //List<List<Chunk>> chunksInRadius = new List<List<Chunk>>(RenderDistance * 16);
        for (int i = -(RenderDistance - (int)userPos.x/Dimentions);
            i < RenderDistance + (int)userPos.x/Dimentions;
            i++)
        {
            for (
                int v = -(RenderDistance - (int)userPos.z/Dimentions);
                v < RenderDistance + (int)userPos.z/Dimentions;
                v++)
            {
                if (ChunkGrid[i + gridSize, v + gridSize]?.gameObject == null)
                {
                    var gO = GameObject.Instantiate(ChunkPrefab, new Vector3(i * Dimentions, 0, v * Dimentions), Quaternion.identity);
                    gO.name = $"Chunk{i}x{v}";
                    gO.transform.parent = gameObject.transform;
                    var gOmesh = gO.GetComponent<MeshGenerator>();
                    gOmesh.xOffset = i * Scale;
                    gOmesh.zOffset = v * Scale;
                    gOmesh.MeshInitializer();
 
                    ChunkGrid[i + gridSize, v + gridSize] = gO;
                }
            }
        }
    }
}

